using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Venator.Storage.Blocks;
using Venator.Storage.Contracts.Common;
using Venator.Storage.Contracts.Common.Extensions;
using Venator.Storage.Contracts.Constants;
using Venator.Storage.Pools;
using Venator.Utilities.Files;

namespace Venator.Storage;

public static partial class StorageEngineBootstrapper
{
   public static StorageEngine OpenOrCreate(string filePath, StorageEngineOptions options)
   {
      var fileHandle = DirectFile.Open(filePath);
      var alignment = DeviceAlignment.Query(fileHandle);
      var fileLength = RandomAccess.GetLength(fileHandle);

      if (fileLength == 0)
      {
         return Create(fileHandle, alignment, options);
      }

      // If DualWayfinderSize is 8 KB and sector size is 4 KB -> reads 8 KB.
      // If DualWayfinderSize is 8 KB and sector size is 16 KB -> reads 16 KB.
      var sectorSize = Math.Max(alignment.SectorSize, 4096);
      var readBytes = Math.Max(StorageConstants.DualWayfinderSize,
         ((StorageConstants.DualWayfinderSize + sectorSize - 1) / sectorSize) * sectorSize);

      if (fileLength < readBytes)
      {
         throw new InvalidDataException(
            $"Database file size ({fileLength} bytes) is smaller than required sector read size ({readBytes} bytes).");
      }

      var memoryAlignment = (nuint)Math.Max(alignment.MemoryAlignment, sectorSize);
      var alignedMemory = (byte*)NativeMemory.AlignedAlloc(readBytes, memoryAlignment);

      try
      {
         Span<byte> readBuffer = new(alignedMemory, (int)readBytes);

         var bytesRead = RandomAccess.Read(fileHandle, readBuffer, fileOffset: 0);
         if (bytesRead < readBytes)
         {
            throw new InvalidDataException(
               $"Failed to read dual Wayfinder sectors. Expected {readBytes} bytes, got {bytesRead}.");
         }

         ReadOnlySpan<byte> alphaSpan = readBuffer[..StorageConstants.WayfinderSize];
         ReadOnlySpan<byte> betaSpan = readBuffer.Slice(StorageConstants.WayfinderSize, StorageConstants.WayfinderSize);

         ref readonly var rawAlpha = ref Unsafe.As<byte, WayfinderBlock>(ref MemoryMarshal.GetReference(alphaSpan));
         ref readonly var rawBeta = ref Unsafe.As<byte, WayfinderBlock>(ref MemoryMarshal.GetReference(betaSpan));

         var alpha = rawAlpha.EnsureLittleEndian();
         var beta = rawBeta.EnsureLittleEndian();

         var dual = new DualWayfinderBlock(alpha, beta);
         var activeWayfinder = dual.GetActiveWayfinder();

         var discoveredBlockSize = activeWayfinder.BlockSize;
         BlockSizeValidator.ValidateOrThrow(discoveredBlockSize, alignment);

         var storage = new FileBlockStorage(fileHandle, discoveredBlockSize);
         var bufferPool = new BufferPool(storage, slotCount: options.BufferPoolSlots);

         return new StorageEngine(storage, bufferPool, activeWayfinder);
      }
      finally
      {
         NativeMemory.AlignedFree(alignedMemory);
      }
   }
}
