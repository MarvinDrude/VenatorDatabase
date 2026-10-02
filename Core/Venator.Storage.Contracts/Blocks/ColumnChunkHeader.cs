using System.Runtime.InteropServices;
using Venator.Storage.Contracts.Enums;
using Venator.Storage.Contracts.Ids;

namespace Venator.Storage.Contracts.Blocks;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 128)]
public readonly struct ColumnChunkHeader
{
   [FieldOffset(0)]
   public readonly BlockId StartBlockId;
   [FieldOffset(8)]
   public readonly uint BlockCount;

   [FieldOffset(12)]
   public readonly uint CompressedLength;
   [FieldOffset(16)]
   public readonly uint UncompressedLength;

   [FieldOffset(20)]
   public readonly ushort ByteOffsetInBlock;

   [FieldOffset(22)]
   public readonly EncodingKind Encoding;
   [FieldOffset(23)]
   public readonly CompressionKind Compression;

   [FieldOffset(24)]
   public readonly ZoneMap ZoneMap; // 40 bytes
}
