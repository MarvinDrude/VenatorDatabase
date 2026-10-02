using System.Runtime.CompilerServices;

namespace Venator.Utilities.Dates;

public static class Timing
{
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ulong GetTimestamp(DateTimeOffset date)
   {
      return (ulong)date.ToUnixTimeMilliseconds();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ulong GetTimestamp()
   {
      return GetTimestamp(DateTimeOffset.UtcNow);
   }
}
