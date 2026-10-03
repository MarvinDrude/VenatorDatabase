using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using Venator.Storage.Contracts.Enums;
using Venator.Storage.Contracts.Ids;

namespace Venator.Storage.Contracts.Blocks.Extensions;

public static class ColumnChunkHeaderExtensions
{
   extension(scoped in ColumnChunkHeader header)
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public ColumnChunkHeader EnsureLittleEndian()
      {
         if (BitConverter.IsLittleEndian)
            return header;

         return new ColumnChunkHeader(
            startBlockId: (BlockId)BinaryPrimitives.ReverseEndianness(header.StartBlockId.Value),
            blockCount: BinaryPrimitives.ReverseEndianness(header.BlockCount),
            compressedLength: BinaryPrimitives.ReverseEndianness(header.CompressedLength),
            uncompressedLength: BinaryPrimitives.ReverseEndianness(header.UncompressedLength),
            byteOffsetInBlock: BinaryPrimitives.ReverseEndianness(header.ByteOffsetInBlock),
            encoding: (EncodingKind)BinaryPrimitives.ReverseEndianness((byte)header.Encoding),
            compression: (CompressionKind)BinaryPrimitives.ReverseEndianness((byte)header.Compression),
            zoneMap: header.ZoneMap.EnsureLittleEndian());
      }
   }
}
