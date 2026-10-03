using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Venator.Storage.Contracts.Ids.Extensions;

public static class RowGroupIdExtensions
{
   extension(scoped in RowGroupId id)
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public RowGroupId EnsureLittleEndian()
      {
         if (BitConverter.IsLittleEndian)
            return id;

         return new RowGroupId(BinaryPrimitives.ReverseEndianness(id.Value));
      }
   }
}
