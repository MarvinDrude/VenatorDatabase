using System.Runtime.CompilerServices;
using Microsoft.Win32.SafeHandles;
using Venator.Storage.Contracts.Ids;
using Venator.Storage.Contracts.Interfaces;

namespace Venator.Storage.Blocks;

/// <summary>
/// Owns and disposes its file handle by itself
/// </summary>
public sealed class FileBlockStorage : IBlockStorage
{
   public uint BlockSize { get; }

   public uint TotalBlocks => Volatile.Read(ref _totalBlocks);

   /// <summary>
   /// Let's be generous and always allocate a lot of new blocks
   /// </summary>
   private const uint AllocationChunkBlocks = 1024;

   private readonly SafeFileHandle _fileHandle;

   private uint _totalBlocks;

   private uint _physicalAllocatedBlocks;
   private readonly Lock _growLock = new();

   public FileBlockStorage(
      SafeFileHandle fileHandle,
      uint blockSize)
   {
      BlockSize = blockSize;
      _fileHandle = fileHandle;

      var fileLength = RandomAccess.GetLength(_fileHandle);
      _totalBlocks = (uint)(fileLength / blockSize);
      _physicalAllocatedBlocks = _totalBlocks;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void ReadBlock(BlockId blockId, Span<byte> destination)
   {
      var fileOffset = (long)blockId.Value * BlockSize;
      RandomAccess.Read(_fileHandle, destination, fileOffset);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteBlock(BlockId blockId, ReadOnlySpan<byte> source)
   {
      var fileOffset = (long)blockId.Value * BlockSize;
      RandomAccess.Write(_fileHandle, source, fileOffset);
   }

   public BlockId AllocateBlock()
   {
      var newBlockNum = Interlocked.Increment(ref _totalBlocks) - 1;
      if (newBlockNum < Volatile.Read(ref _physicalAllocatedBlocks))
         return new BlockId(newBlockNum);

      lock (_growLock)
      {
         if (newBlockNum >= _physicalAllocatedBlocks)
         {
            var targetBlocks = _physicalAllocatedBlocks + AllocationChunkBlocks;
            RandomAccess.SetLength(_fileHandle, (long)targetBlocks * BlockSize);

            _physicalAllocatedBlocks = targetBlocks;
         }
      }

      return new BlockId(newBlockNum);
   }

   public void Flush()
      => RandomAccess.FlushToDisk(_fileHandle);

   public void Dispose()
   {
      Flush();
      _fileHandle.Dispose();
   }
}
