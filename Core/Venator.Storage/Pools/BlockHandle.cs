using System.Runtime.CompilerServices;
using Venator.Storage.Contracts.Ids;

namespace Venator.Storage.Pools;

public readonly struct BlockHandle : IDisposable
{
   private readonly BufferPool? _pool;

   public readonly int SlotIndex;
   public readonly BlockId BlockId;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal BlockHandle(
      BufferPool pool,
      int slotIndex,
      BlockId blockId)
   {
      _pool = pool;

      SlotIndex = slotIndex;
      BlockId = blockId;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ReadOnlySpan<byte> GetSpan() => _pool!.GetSlotSpan(SlotIndex);

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Dispose() => _pool?.Unpin(SlotIndex);
}

