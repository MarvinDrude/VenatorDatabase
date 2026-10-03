using System.Buffers.Binary;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Beskar.Memory.Flags;
using Venator.Storage.Contracts.Enums;
using Venator.Storage.Contracts.Ids;

namespace Venator.Storage.Contracts.Catalogs.Extensions;

public static class ColumnHeaderExtensions
{
   extension(scoped in ColumnHeader header)
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public ColumnHeader EnsureLittleEndian()
      {
         if (BitConverter.IsLittleEndian)
            return header;

         return new ColumnHeader(
            columnId: (ColumnId)BinaryPrimitives.ReverseEndianness(header.Identifier.Value),
            kind: (ColumnKind)BinaryPrimitives.ReverseEndianness((byte)header.Kind),
            flags: new PackedBools32(BinaryPrimitives.ReverseEndianness(header.Flags.RawValue)),
            nameByteLength: BinaryPrimitives.ReverseEndianness(header.NameByteLength),
            defaultValueByteLength: BinaryPrimitives.ReverseEndianness(header.DefaultValueByteLength));
      }
   }
}
