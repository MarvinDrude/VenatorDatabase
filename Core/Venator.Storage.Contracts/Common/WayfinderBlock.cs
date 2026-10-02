using System.Runtime.InteropServices;
using Beskar.Memory.Flags;
using Venator.Storage.Contracts.Constants;
using Venator.Storage.Contracts.Ids;
using Venator.Utilities.Dates;
using Venator.Utilities.Hashes;

namespace Venator.Storage.Contracts.Common;

/// <summary>
/// The very first byte data in the database file.
/// </summary>
[StructLayout(LayoutKind.Explicit, Pack = 1, Size = StorageConstants.WayfinderSize)]
public readonly struct WayfinderBlock
{
   /// <summary>
   /// Magic number always the same: VENATOR\0
   /// </summary>
   [FieldOffset(0)]
   public readonly ulong MagicNumber;

   /// <summary>
   /// Increments per commit
   /// </summary>
   [FieldOffset(8)]
   public readonly ulong SequenceNumber;

   /// <summary>
   /// Feature flags
   /// </summary>
   [FieldOffset(16)]
   public readonly Flags256 Flags;

   /// <summary>
   /// The configured block size (ex: 64kb)
   /// </summary>
   [FieldOffset(48)]
   public readonly uint BlockSize;

   /// <summary>
   /// Block id where the catalog block ids are stored
   /// </summary>
   [FieldOffset(52)]
   public readonly BlockId CatalogRootBlockId;

   /// <summary>
   /// Block id where the bitmap of free blocks are stored
   /// </summary>
   [FieldOffset(60)]
   public readonly BlockId FreeBitmapRootBlockId;

   /// <summary>
   /// Total blocks occupied by this db
   /// </summary>
   [FieldOffset(68)]
   public readonly uint TotalBlocks;

   /// <summary>
   /// Last write action (any kind) as timestamp
   /// </summary>
   [FieldOffset(72)]
   public readonly ulong TimestampUtc;

   /// <summary>
   /// Format version used to create the DB
   /// </summary>
   [FieldOffset(80)]
   public readonly uint FormatVersion;

   /// <summary>
   /// A checksum to validate
   /// </summary>
   [FieldOffset(StorageConstants.WayfinderSize - sizeof(ulong))]
   public readonly ulong Checksum;

   public bool IsValid =>
      MagicNumber is StorageConstants.MagicNumber
        && FormatVersion == StorageConstants.FormatVersion
        && Checksum == ComputeChecksum();

   public WayfinderBlock(
      ulong sequenceNumber,
      uint blockSize,
      Flags256 flags,
      BlockId catalogRootBlockId,
      BlockId freeBitmapRootBlockId,
      uint totalBlocks)
   {
      SequenceNumber = sequenceNumber;
      BlockSize = blockSize;

      Flags = flags;

      CatalogRootBlockId = catalogRootBlockId;
      FreeBitmapRootBlockId = freeBitmapRootBlockId;

      TotalBlocks = totalBlocks;

      MagicNumber = StorageConstants.MagicNumber;
      FormatVersion = StorageConstants.FormatVersion;
      TimestampUtc = Timing.GetTimestamp();
      Checksum = ComputeChecksum();
   }

   private ulong ComputeChecksum()
   {
      unsafe
      {
         fixed (WayfinderBlock* ptr = &this)
         {
            ReadOnlySpan<byte> span = new(ptr, StorageConstants.WayfinderSize - sizeof(ulong));
            return XxHashGen.HashToUInt64(span);
         }
      }
   }
}
