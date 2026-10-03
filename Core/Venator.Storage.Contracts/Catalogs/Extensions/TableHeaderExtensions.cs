using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using Beskar.Memory.Flags;
using Venator.Storage.Contracts.Ids;

namespace Venator.Storage.Contracts.Catalogs.Extensions;

public static class TableHeaderExtensions
{
   extension(scoped in TableHeader header)
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public TableHeader EnsureLittleEndian()
      {
         if (BitConverter.IsLittleEndian)
            return header;

         return new TableHeader(
            identifier: (TableId)BinaryPrimitives.ReverseEndianness(header.Identifier.Value),
            createdEpoch: BinaryPrimitives.ReverseEndianness(header.CreatedEpoch),
            lastModifiedEpoch: BinaryPrimitives.ReverseEndianness(header.LastModifiedEpoch),
            rowCount: BinaryPrimitives.ReverseEndianness(header.RowCount),
            rowGroupCount: BinaryPrimitives.ReverseEndianness(header.RowGroupCount),
            columnCount: BinaryPrimitives.ReverseEndianness(header.ColumnCount),
            sortKeyCount: BinaryPrimitives.ReverseEndianness(header.SortKeyCount),
            nameByteLength: BinaryPrimitives.ReverseEndianness(header.NameByteLength),
            flags: new PackedBools32(BinaryPrimitives.ReverseEndianness(header.Flags.RawValue)));
      }
   }
}
