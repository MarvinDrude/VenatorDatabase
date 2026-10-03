using Venator.Storage.Contracts.Interfaces;

namespace Venator.Storage.Contracts.Blocks;

/// <summary>
/// Heap-safe handle that keeps a block pinned.
/// </summary>
public sealed class BlockHandle
{
   private readonly IBlockStorage _blockStorage;


   public BlockHandle()
   {

   }
}
