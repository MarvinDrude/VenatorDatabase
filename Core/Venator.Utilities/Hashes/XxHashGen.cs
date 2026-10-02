using System.IO.Hashing;
using System.Text;
using Beskar.Memory.Owners;

namespace Venator.Utilities.Hashes;

public static class XxHashGen
{
   /// <summary>
   /// Computes the 64-bit XXH3 hash of a byte buffer
   /// </summary>
   public static ulong HashToUInt64(ReadOnlySpan<byte> source, long seed = 0)
   {
      return XxHash64.HashToUInt64(source, seed);
   }

   /// <summary>
   /// Computes the 64-bit XXH§ hash of a given char buffer
   /// </summary>
   public static ulong HashToUInt64(ReadOnlySpan<char> source, long seed = 0)
   {
      if (source.Length is 0)
      {
         return 0;
      }

      var byteCount = Encoding.UTF8.GetByteCount(source);
      var bufferOwner = byteCount <= 256
         ? new SpanOwner<byte>(stackalloc byte[byteCount])
         : new SpanOwner<byte>(byteCount);
      var buffer = bufferOwner.Span;

      Encoding.UTF8.GetBytes(source, buffer);
      return XxHash3.HashToUInt64(buffer, seed);
   }
}
