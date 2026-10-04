using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Venator.Storage.Contracts.Catalogs;
using Venator.Storage.Contracts.Catalogs.Extensions;
using Venator.Storage.Contracts.Common;
using Venator.Storage.Contracts.Common.Extensions;
using Venator.Storage.Contracts.Constants;
using Venator.Storage.Contracts.Ids;
using Venator.Utilities.Files;
using Venator.Utilities.Hashes;

namespace Venator.Storage;

public static partial class StorageEngineBootstrapper
{
   private static StorageEngine Create(
      SafeFileHandle handle,
      in StorageAlignmentInfo alignmentInfo,
      StorageEngineOptions options)
   {
      var blockSize = options.PreferredBlockSize;

      if (blockSize < alignmentInfo.SectorSize)
         blockSize = alignmentInfo.SectorSize;

      var blockMemory = (byte*)NativeMemory.AlignedAlloc(blockSize, alignmentInfo.MemoryAlignment);
      try
      {
         // lets do the wayfinder init
         // TODO: refactor to own method

         Span<byte> blockSpan = new(blockMemory, (int)blockSize);
         NativeMemory.Clear(blockMemory, blockSize);

         var wayfinderAlpha = new WayfinderBlock(
            sequenceNumber: 1,
            blockSize: blockSize,
            flags: default,
            catalogRootBlockId: new BlockId(1),
            freeBitmapRootBlockId: new BlockId(2),
            totalBlocks: 3);

         var alphaSector = blockSpan[..StorageConstants.WayfinderSize];
         Unsafe.WriteUnaligned(
            ref MemoryMarshal.GetReference(alphaSector),
            wayfinderAlpha.EnsureLittleEndian());

         RandomAccess.Write(handle, blockSpan, 0);
         NativeMemory.Clear(blockMemory, blockSize);

         // lets do the catalog init
         // TODO: refactor to own method

         const uint catMagic = StorageConstants.CatalogMagicNumber; // "VCAT"
         const ushort catVersion = (ushort)StorageConstants.FormatVersion;
         const ushort initialTableCount = 0;

         Span<byte> catChecksumInput = stackalloc byte[8];
         BinaryPrimitives.WriteUInt32LittleEndian(catChecksumInput[..4], catMagic);
         BinaryPrimitives.WriteUInt16LittleEndian(catChecksumInput[4..6], catVersion);
         BinaryPrimitives.WriteUInt16LittleEndian(catChecksumInput[6..8], initialTableCount);
         var catChecksum = XxHashGen.HashToUInt64(catChecksumInput);

         var catalogHeader = new CatalogHeader(
            magicNumber: catMagic,
            version: catVersion,
            tableCount: initialTableCount,
            checksum: catChecksum);
         Unsafe.WriteUnaligned(
            ref MemoryMarshal.GetReference(blockSpan),
            catalogHeader.EnsureLittleEndian());

         var block1Offset = (long)blockSize * 1;
         RandomAccess.Write(handle, blockSpan, block1Offset);

         // lets do the bitmap
         // TODO: refactor to own method


         return null!;
      }
      finally
      {
         NativeMemory.Free(blockMemory);
      }
   }
}
