using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Venator.Storage.Contracts.Ids;
using Venator.Storage.Pools;

namespace Venator.Storage.Leases;

[StructLayout(LayoutKind.Auto)]
public readonly ref struct WriteBlockLease
{
   private readonly BufferPool _pool;

   public readonly int SlotIndex;
   public readonly BlockId BlockId;
   public readonly Span<byte> Span;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal WriteBlockLease(
      BufferPool pool,
      int slotIndex,
      BlockId blockId,
      Span<byte> span)
   {
      _pool = pool;

      SlotIndex = slotIndex;
      BlockId = blockId;
      Span = span;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Dispose() => _pool.UnpinAndMarkDirty(SlotIndex);
}
