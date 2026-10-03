using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Venator.Storage.Contracts.Catalogs.Extensions;

public static class CatalogHeaderExtensions
{
   extension(scoped in CatalogHeader header)
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public CatalogHeader EnsureLittleEndian()
      {
         if (BitConverter.IsLittleEndian)
            return header;

         return new CatalogHeader(
            magicNumber: BinaryPrimitives.ReverseEndianness(header.MagicNumber),
            tableCount: BinaryPrimitives.ReverseEndianness(header.TableCount),
            version: BinaryPrimitives.ReverseEndianness(header.Version),
            checksum: BinaryPrimitives.ReverseEndianness(header.Checksum));
      }
   }
}
