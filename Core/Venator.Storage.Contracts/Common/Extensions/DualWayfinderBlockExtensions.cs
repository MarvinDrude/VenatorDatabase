using System.Runtime.CompilerServices;

namespace Venator.Storage.Contracts.Common.Extensions;

public static class DualWayfinderBlockExtensions
{
   extension(scoped in DualWayfinderBlock block)
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public DualWayfinderBlock EnsureLittleEndian()
      {
         if (BitConverter.IsLittleEndian)
            return block;

         return new DualWayfinderBlock(
            wayfinderAlpha: block.WayfinderAlpha.EnsureLittleEndian(),
            wayfinderBeta: block.WayfinderBeta.EnsureLittleEndian());
      }
   }
}
