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

      public WayfinderBlock GetActiveWayfinder()
      {
         var alpha = block.WayfinderAlpha;
         var beta = block.WayfinderBeta;

         var alphaValid = alpha.IsValid;
         var betaValid  = beta.IsValid;

         return alphaValid switch
         {
            true when betaValid => beta.SequenceNumber > alpha.SequenceNumber ? beta : alpha,
            true => alpha,
            _ => betaValid
               ? beta
               : throw new InvalidDataException(
                  "Database corruption: Neither Wayfinder Alpha nor Beta has a valid checksum.")
         };
      }
   }
}
