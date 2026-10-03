using System.Runtime.InteropServices;
using Beskar.Memory.Flags;
using Venator.Storage.Contracts.Enums;
using Venator.Storage.Contracts.Ids;

namespace Venator.Storage.Contracts.Catalogs;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct ColumnHeader(
   ColumnId columnId,
   ColumnKind kind,
   PackedBools32 flags,
   ushort nameByteLength,
   ushort defaultValueByteLength)
{
   public readonly ColumnId Identifier = columnId;
   public readonly ColumnKind Kind = kind;
   public readonly PackedBools32 Flags = flags;

   public readonly ushort NameByteLength = nameByteLength;
   public readonly ushort DefaultValueByteLength = defaultValueByteLength;

   public bool IsNullable => Flags.Get(0);
   public bool HasDefaultValue => Flags.Get(1);
}
