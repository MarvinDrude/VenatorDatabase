using System.Numerics;
using Venator.Storage.Contracts.Constants;
using Venator.Utilities.Files;

namespace Venator.Storage.Blocks;

public static class BlockSizeValidator
{
   public const uint MinimumBlockSize = 4096; // 4 KB
   public const uint MaximumBlockSize = 1048576 * 2; // 2 MB

   public static uint DetermineBlockSize(StorageEngineOptions options, in StorageAlignmentInfo alignment)
   {
      if (options.PreferredBlockSize is not 0)
      {
         return ValidateOrThrow(options.PreferredBlockSize, alignment);
      }

      var target = Math.Max(MaximumBlockSize, alignment.OptimalBlockSize);
      return BitOperations.RoundUpToPowerOf2(target);
   }

   public static uint ValidateOrThrow(uint requestedBlockSize, in StorageAlignmentInfo alignment)
   {
      if (!BitOperations.IsPow2(requestedBlockSize))
      {
         throw new ArgumentException(
            $"Requested block size ({requestedBlockSize} bytes) must be a power of two.",
            nameof(requestedBlockSize));
      }

      if (requestedBlockSize is < MinimumBlockSize or > MaximumBlockSize)
      {
         throw new ArgumentOutOfRangeException(
            nameof(requestedBlockSize),
            $"Requested block size ({requestedBlockSize} bytes) must be between {MinimumBlockSize} and {MaximumBlockSize} bytes.");
      }

      var requiredAlignment = Math.Max(alignment.SectorSize, 512);
      if (requestedBlockSize % requiredAlignment != 0)
      {
         throw new InvalidOperationException(
            $"Requested block size ({requestedBlockSize} bytes) is incompatible with the underlying storage device. " +
            $"It must be an exact multiple of the sector size ({alignment.SectorSize} bytes).");
      }

      if (requestedBlockSize < StorageConstants.DualWayfinderSize)
      {
         throw new InvalidOperationException(
            $"Requested block size ({requestedBlockSize} bytes) is smaller than 2x Wayfinder size ({StorageConstants.WayfinderSize} bytes).");
      }

      return requestedBlockSize;
   }
}
