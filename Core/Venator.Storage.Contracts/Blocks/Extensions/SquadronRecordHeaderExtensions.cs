using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using Venator.Storage.Contracts.Ids;

namespace Venator.Storage.Contracts.Blocks.Extensions;

public static class SquadronRecordHeaderExtensions
{
   extension(scoped in SquadronRecordHeader header)
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public SquadronRecordHeader EnsureLittleEndian()
      {
         if (BitConverter.IsLittleEndian)
            return header;

         return new SquadronRecordHeader(
            id: (RowGroupId)BinaryPrimitives.ReverseEndianness(header.Id.Value),
            createdEpoch: BinaryPrimitives.ReverseEndianness(header.CreatedEpoch),
            rowCount: BinaryPrimitives.ReverseEndianness(header.RowCount));
      }
   }
}
