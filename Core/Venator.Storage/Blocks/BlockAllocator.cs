using Venator.Storage.Contracts.Ids;
using Venator.Storage.Pools;

namespace Venator.Storage.Blocks;

/// <summary>
/// TODO: lock-free?
/// </summary>
public sealed class BlockAllocator
{
   private readonly BufferPool _bufferPool;
   private readonly FileBlockStorage _storage;
   private readonly BlockId _bitmapRootBlockId;

   private readonly Lock _allocLock = new();

   public BlockAllocator(
      BufferPool bufferPool,
      FileBlockStorage storage,
      BlockId bitmapRootBlockId)
   {
      _bufferPool = bufferPool;
      _storage = storage;
      _bitmapRootBlockId = bitmapRootBlockId;
   }

   public BlockId AllocateBlock()
   {
      lock (_allocLock)
      {
         using var lease = _bufferPool.AcquireWriteLease(_bitmapRootBlockId);
         var bitmap = new BitmapBlockAccessor(lease.Span);

         return bitmap.TryAllocateSingle(out var recycledBlock)
            ? recycledBlock
            : _storage.AllocateBlock();
      }
   }

   public BlockId AllocateExtent(uint count)
   {
      lock (_allocLock)
      {
         using var lease = _bufferPool.AcquireWriteLease(_bitmapRootBlockId);
         var bitmap = new BitmapBlockAccessor(lease.Span);

         if (bitmap.TryAllocateExtent(count, out BlockId startBlock))
         {
            return startBlock;
         }

         var firstAppended = _storage.AllocateBlock();
         for (uint i = 1; i < count; i++)
         {
            _storage.AllocateBlock();
         }

         return firstAppended;
      }
   }

   public void FreeBlock(BlockId blockId)
   {
      lock (_allocLock)
      {
         using var lease = _bufferPool.AcquireWriteLease(_bitmapRootBlockId);

         var bitmap = new BitmapBlockAccessor(lease.Span);
         bitmap.Free(blockId);
      }
   }
}
