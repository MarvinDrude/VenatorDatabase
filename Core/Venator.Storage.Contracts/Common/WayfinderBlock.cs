using System.Runtime.InteropServices;
using Venator.Storage.Contracts.Ids;

namespace Venator.Storage.Contracts.Common;

/// <summary>
/// The very first byte data in the database file.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4096)]
public readonly struct WayfinderBlock
{
   /// <summary>
   /// Magic number always the same: VENATOR\0
   /// </summary>
   public readonly ulong MagicNumber;

   /// <summary>
   /// Increments per commit
   /// </summary>
   public readonly ulong SequenceNumber;

   /// <summary>
   /// The configured block size (ex: 64kb)
   /// </summary>
   public readonly uint BlockSize;

   /// <summary>
   /// Block id where the catalog block ids are stored
   /// </summary>
   public readonly BlockId CatalogRootBlockId;

   /// <summary>
   /// Block id where the bitmap of free blocks are stored
   /// </summary>
   public readonly BlockId FreeBitmapRootBlockId;

   /// <summary>
   /// Total blocks occupied by this db
   /// </summary>
   public readonly uint TotalBlocks;

   /// <summary>
   /// Last write action (any kind) as timestamp
   /// </summary>
   public readonly ulong TimestampUtc;

   /// <summary>
   /// A checksum to validate
   /// </summary>
   public readonly ulong Checksum;
}
