using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Venator.Storage.Contracts.Ids.Extensions;

public static class TableIdExtensions
{
   extension(scoped in TableId id)
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public TableId EnsureLittleEndian()
      {
         if (BitConverter.IsLittleEndian)
            return id;

         return new TableId(BinaryPrimitives.ReverseEndianness(id.Value));
      }
   }
}
