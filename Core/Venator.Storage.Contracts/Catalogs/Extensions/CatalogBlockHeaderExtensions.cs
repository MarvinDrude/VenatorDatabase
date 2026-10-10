using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using Venator.Storage.Contracts.Ids;

namespace Venator.Storage.Contracts.Catalogs.Extensions;

public static class CatalogBlockHeaderExtensions
{
   extension(scoped in CatalogBlockHeader header)
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public CatalogBlockHeader EnsureLittleEndian()
      {
         if (BitConverter.IsLittleEndian)
            return header;

         return new CatalogBlockHeader(
            nextBlockId: (BlockId)BinaryPrimitives.ReverseEndianness(header.NextBlockId.Value));
      }
   }
}
