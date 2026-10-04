using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Venator.Storage.Contracts.Blocks;
using Venator.Storage.Contracts.Enums.Internal;
using Venator.Storage.Contracts.Ids;
using Venator.Storage.Contracts.Interfaces;
using Venator.Storage.Leases;

namespace Venator.Storage.Pools;

public sealed unsafe class BufferPool : IDisposable
{
   private readonly IBlockStorage _storage;

   private readonly byte* _slab;
   private readonly BufferSlot[] _slots;

   private const int StripeCount = 64;
   private readonly Lock[] _stripeLocks;

   private readonly ConcurrentDictionary<BlockId, int> _pageTable = new();

   private int _clockHand;
   private bool _isDisposed;

   public uint BlockSize { get; }
   public int SlotCount { get; }

   public BufferPool(IBlockStorage storage, int slotCount)
   {
      _storage = storage ?? throw new ArgumentNullException(nameof(storage));

      SlotCount = slotCount;
      BlockSize = storage.BlockSize;

      _slots = new BufferSlot[slotCount];

      for (var i = 0; i < slotCount; i++)
      {
         _slots[i].BlockId = BlockId.Invalid;
         _slots[i].State = SlotState.Empty;
      }

      _stripeLocks = new Lock[StripeCount];

      for (var i = 0; i < StripeCount; i++)
      {
         _stripeLocks[i] = new Lock();
      }

      var totalBytes = slotCount * BlockSize;

      _slab = (byte*)NativeMemory.AlignedAlloc((nuint)totalBytes, 4096);
      NativeMemory.Clear(_slab, (nuint)totalBytes);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ReadBlockLease AcquireReadLease(BlockId blockId)
   {
      var slotIndex = GetOrLoadSlot(blockId);
      return new ReadBlockLease(this, slotIndex, blockId, GetSlotSpan(slotIndex));
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public WriteBlockLease AcquireWriteLease(BlockId blockId)
   {
      var slotIndex = GetOrLoadSlot(blockId);
      return new WriteBlockLease(this, slotIndex, blockId, GetSlotSpan(slotIndex));
   }

   public BlockHandle AcquireHandle(BlockId blockId)
   {
      var slotIndex = GetOrLoadSlot(blockId);
      return new BlockHandle(this, slotIndex, blockId);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal void Unpin(int slotIndex)
   {
      ref var slot = ref _slots[slotIndex];
      _ = Interlocked.Decrement(ref slot.PinCount);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal void UnpinAndMarkDirty(int slotIndex)
   {
      ref var slot = ref _slots[slotIndex];
      slot.IsDirty = true;
      _ = Interlocked.Decrement(ref slot.PinCount);
   }

   private int GetOrLoadSlot(BlockId blockId)
   {
      while (_pageTable.TryGetValue(blockId, out var slotIndex))
      {
         ref var slot = ref _slots[slotIndex];
         Interlocked.Increment(ref slot.PinCount);

         if (slot.BlockId == blockId
             && slot.State is SlotState.Loaded)
         {
            slot.UsageMarker = 1; // mark recently used
            return slotIndex;
         }

         // block was evicted just now, lets retry
         Interlocked.Decrement(ref slot.PinCount);
      }

      // slow cache miss
      var stripe = (int)(blockId.Value % StripeCount);
      lock (_stripeLocks[stripe])
      {
         if (_pageTable.TryGetValue(blockId, out var existingSlot))
         {
            // was loaded since we last checked -> fast get
            Interlocked.Increment(ref _slots[existingSlot].PinCount);
            _slots[existingSlot].UsageMarker = 1;

            return existingSlot;
         }

         // find one to unload
         var targetIndex = EvictVictimSlot();
         ref var target = ref _slots[targetIndex];

         var targetSpan = GetSlotSpan(targetIndex);
         _storage.ReadBlock(blockId, targetSpan);

         target.BlockId = blockId;
         target.PinCount = 1;
         target.IsDirty = false;
         target.UsageMarker = 1;
         target.State = SlotState.Loaded;

         _pageTable[blockId] = targetIndex;
         return targetIndex;
      }
   }

   private int EvictVictimSlot()
   {
      var scans = 0;
      var maxScans = SlotCount * 2;

      while (scans < maxScans)
      {
         var index = Interlocked.Increment(ref _clockHand) % SlotCount;
         scans++;

         ref var slot = ref _slots[index];

         if (Volatile.Read(ref slot.PinCount) > 0
             || slot.State is SlotState.Loading)
            continue;

         if (slot.UsageMarker == 1)
         {
            slot.UsageMarker = 0;
            continue;
         }

         // claim slot for eviction
         lock (_stripeLocks[index % StripeCount])
         {
            if (slot.PinCount > 0) continue;
            slot.State = SlotState.Evicting;

            if (slot is { IsDirty: true, BlockId.IsValid: true })
            {
               _storage.WriteBlock(slot.BlockId, GetSlotSpan(index));
               slot.IsDirty = false;
            }

            if (slot.BlockId.IsValid)
            {
               _pageTable.TryRemove(slot.BlockId, out _);
            }

            slot.State = SlotState.Loading;
            return index;
         }
      }

      // TODO: should we throw here? or just block until available? -> open question
      throw new InvalidOperationException("Buffer pool exhausted");
   }

   public void Prefetch(BlockId blockId)
   {
      if (_pageTable.ContainsKey(blockId))
         return;

      ThreadPool.UnsafeQueueUserWorkItem(_ =>
      {
         try
         {
            using var lease = AcquireReadLease(blockId);
         }
         catch
         {
            // TODO: prefetch is best-effort for now, is that ok?
         }
      }, null);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public Span<byte> GetSlotSpan(int slotIndex)
   {
      var ptr = _slab + (slotIndex * BlockSize);
      return new Span<byte>(ptr, (int)BlockSize);
   }

   public void Dispose()
   {
      if (_isDisposed) return;
      _isDisposed = true;

      // TODO: should we flush all dirty blocks?

      NativeMemory.AlignedFree(_slab);
   }
}
