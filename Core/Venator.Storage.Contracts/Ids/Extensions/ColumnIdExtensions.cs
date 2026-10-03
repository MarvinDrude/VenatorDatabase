using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Venator.Storage.Contracts.Ids.Extensions;

public static class ColumnIdExtensions
{
   extension(scoped in ColumnId id)
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public ColumnId EnsureLittleEndian()
      {
         if (BitConverter.IsLittleEndian)
            return id;

         return new ColumnId(BinaryPrimitives.ReverseEndianness(id.Value));
      }
   }
}
