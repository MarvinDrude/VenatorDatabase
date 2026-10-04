using Venator.Storage.Blocks;
using Venator.Storage.Contracts.Common;
using Venator.Storage.Contracts.Interfaces;
using Venator.Storage.Pools;

namespace Venator.Storage;

public sealed class StorageEngine
{
   private readonly IBlockStorage _blockStorage;
   private readonly BufferPool _bufferPool;
   private readonly BlockAllocator _blockAllocator;

   private WayfinderBlock _wayfinder;

   public StorageEngine(
      IBlockStorage blockStorage,
      BufferPool bufferPool,
      BlockAllocator blockAllocator,
      in WayfinderBlock activeWayfinder)
   {
      _blockStorage = blockStorage;
      _bufferPool = bufferPool;
      _blockAllocator = blockAllocator;

      _wayfinder = activeWayfinder;
   }
}
