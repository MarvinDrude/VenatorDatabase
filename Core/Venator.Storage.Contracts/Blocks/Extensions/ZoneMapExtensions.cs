using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Venator.Storage.Contracts.Blocks.Extensions;

public static class ZoneMapExtensions
{
   extension(scoped in ZoneMap zoneMap)
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public ZoneMap EnsureLittleEndian()
      {
         if (BitConverter.IsLittleEndian)
            return zoneMap;

         return new ZoneMap(
            other: in zoneMap,
            nullCount: BinaryPrimitives.ReverseEndianness(zoneMap.NullCount),
            rowCount: BinaryPrimitives.ReverseEndianness(zoneMap.RowCount));
      }
   }
}
