using Venator.Storage.Contracts.Ids;
using Venator.Storage.Pools;

namespace Venator.Storage.Blocks;

/// <summary>
/// TODO: lock-free?
/// </summary>
public sealed class BlockAllocator
{
   private readonly BufferPool _bufferPool;
   private readonly FileBlockStorage _storage;
   private readonly BlockId _bitmapRootBlockId;

   private readonly Lock _allocLock = new();

   public BlockAllocator(
      BufferPool bufferPool,
      FileBlockStorage storage,
      BlockId bitmapRootBlockId)
   {
      _bufferPool = bufferPool;
      _storage = storage;
      _bitmapRootBlockId = bitmapRootBlockId;
   }

   public BlockId AllocateBlock()
   {
      lock (_allocLock)
      {
         using var lease = _bufferPool.AcquireWriteLease(_bitmapRootBlockId);
         var bitmap = new BitmapBlockAccessor(lease.Span);

         if (bitmap.TryAllocateSingle(out var allocatedBlock))
         {
            EnsureStorageCapacity(allocatedBlock.Value + 1);
            return allocatedBlock;
         }

         return _storage.AllocateBlock();
      }
   }

   public BlockId AllocateExtent(uint count)
   {
      lock (_allocLock)
      {
         using var lease = _bufferPool.AcquireWriteLease(_bitmapRootBlockId);
         var bitmap = new BitmapBlockAccessor(lease.Span);

         if (bitmap.TryAllocateExtent(count, out BlockId startBlock))
         {
            EnsureStorageCapacity(startBlock.Value + count);
            return startBlock;
         }

         var firstAppended = _storage.AllocateBlock();
         for (uint i = 1; i < count; i++)
         {
            _storage.AllocateBlock();
         }

         return firstAppended;
      }
   }

   private void EnsureStorageCapacity(ulong requiredTotalBlocks)
   {
      while (_storage.TotalBlocks < requiredTotalBlocks)
      {
         _storage.AllocateBlock();
      }
   }

   public void FreeBlock(BlockId blockId)
   {
      lock (_allocLock)
      {
         using var lease = _bufferPool.AcquireWriteLease(_bitmapRootBlockId);

         var bitmap = new BitmapBlockAccessor(lease.Span);
         bitmap.Free(blockId);
      }
   }
}
