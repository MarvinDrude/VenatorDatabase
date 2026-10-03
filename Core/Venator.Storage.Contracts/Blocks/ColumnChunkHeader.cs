using System.Runtime.InteropServices;
using Venator.Storage.Contracts.Enums;
using Venator.Storage.Contracts.Ids;

namespace Venator.Storage.Contracts.Blocks;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 128)]
public readonly struct ColumnChunkHeader(
   BlockId startBlockId,
   uint blockCount,
   uint compressedLength,
   uint uncompressedLength,
   ushort byteOffsetInBlock,
   EncodingKind encoding,
   CompressionKind compression,
   ZoneMap zoneMap)
{
   [FieldOffset(0)]
   public readonly BlockId StartBlockId = startBlockId;
   [FieldOffset(8)]
   public readonly uint BlockCount = blockCount;

   [FieldOffset(12)]
   public readonly uint CompressedLength = compressedLength;
   [FieldOffset(16)]
   public readonly uint UncompressedLength = uncompressedLength;

   [FieldOffset(20)]
   public readonly ushort ByteOffsetInBlock = byteOffsetInBlock;

   [FieldOffset(22)]
   public readonly EncodingKind Encoding = encoding;
   [FieldOffset(23)]
   public readonly CompressionKind Compression = compression;

   [FieldOffset(24)]
   public readonly ZoneMap ZoneMap = zoneMap; // 40 bytes
}
