using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Venator.Storage.Contracts.Ids.Extensions;

public static class BlockIdExtensions
{
   extension(scoped in BlockId id)
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public BlockId EnsureLittleEndian()
      {
         if (BitConverter.IsLittleEndian)
            return id;

         return new BlockId(BinaryPrimitives.ReverseEndianness(id.Value));
      }
   }
}
