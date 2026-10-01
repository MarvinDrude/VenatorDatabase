using System.Runtime.InteropServices;
using Beskar.Memory.Flags;
using Venator.Storage.Contracts.Enums;
using Venator.Storage.Contracts.Ids;

namespace Venator.Storage.Contracts.Catalogs;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct ColumnHeader
{
   public readonly ColumnId Identifier;
   public readonly ColumnKind Kind;
   public readonly PackedBools32 Flags;

   public readonly ushort NameByteLength;
   public readonly ushort DefaultValueByteLength;
}
