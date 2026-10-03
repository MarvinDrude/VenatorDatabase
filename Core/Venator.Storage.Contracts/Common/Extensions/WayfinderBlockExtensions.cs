using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using Beskar.Memory.Flags;
using Venator.Storage.Contracts.Ids;

namespace Venator.Storage.Contracts.Common.Extensions;

public static class WayfinderBlockExtensions
{
   extension(scoped in WayfinderBlock block)
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public WayfinderBlock EnsureLittleEndian()
      {
         if (BitConverter.IsLittleEndian)
            return block;

         Flags256 flags = default;
         flags.SetRawValues(
            BinaryPrimitives.ReverseEndianness(block.Flags[0].RawValue),
            BinaryPrimitives.ReverseEndianness(block.Flags[1].RawValue),
            BinaryPrimitives.ReverseEndianness(block.Flags[2].RawValue),
            BinaryPrimitives.ReverseEndianness(block.Flags[3].RawValue));

         return new WayfinderBlock(
            magicNumber: BinaryPrimitives.ReverseEndianness(block.MagicNumber),
            sequenceNumber: BinaryPrimitives.ReverseEndianness(block.SequenceNumber),
            flags: flags,
            blockSize: BinaryPrimitives.ReverseEndianness(block.BlockSize),
            catalogRootBlockId: (BlockId)BinaryPrimitives.ReverseEndianness(block.CatalogRootBlockId.Value),
            freeBitmapRootBlockId: (BlockId)BinaryPrimitives.ReverseEndianness(block.FreeBitmapRootBlockId.Value),
            totalBlocks: BinaryPrimitives.ReverseEndianness(block.TotalBlocks),
            timestampUtc: BinaryPrimitives.ReverseEndianness(block.TimestampUtc),
            formatVersion: BinaryPrimitives.ReverseEndianness(block.FormatVersion),
            checksum: BinaryPrimitives.ReverseEndianness(block.Checksum));
      }
   }
}
