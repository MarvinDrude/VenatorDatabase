using System.Runtime.InteropServices;
using Beskar.Memory.Flags;
using Venator.Storage.Contracts.Ids;

namespace Venator.Storage.Contracts.Blocks;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 16)]
public struct BufferSlot
{
   [FieldOffset(0)]
   public BlockId BlockId;

   [FieldOffset(8)]
   public int PinCount;

   [FieldOffset(12)]
   public PackedBools16 Flags;

   public bool IsDirty
   {
      get => Flags.Get(0);
      set => Flags.Set(0, value);
   }
}
