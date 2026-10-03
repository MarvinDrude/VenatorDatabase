using Venator.Storage.Contracts.Ids;

namespace Venator.Storage.Contracts.Interfaces;

public interface IBlockStorage
{
   /// <summary>
   /// The size of each block in bytes
   /// </summary>
   public uint BlockSize { get; }

   /// <summary>
   /// Total number of blocks allocated
   /// </summary>
   public uint TotalBlocks { get; }

   /// <summary>
   /// Get block bytes by block id
   /// </summary>
   public void ReadBlock(BlockId blockId, ReadOnlySpan<byte> destination);

   /// <summary>
   /// Get block bytes of block id and blockCount amount after that
   /// </summary>
   public void ReadBlockRange(BlockId blockId, uint blockCount, ReadOnlySpan<byte> destination);

   /// <summary>
   /// Write block bytes of a given block id
   /// </summary>
   public void WriteBlock(BlockId blockId, ReadOnlySpan<byte> data);

   /// <summary>
   /// Appends new block (can be a reclaimed one, or appended at the end)
   /// </summary>
   /// <returns></returns>
   public BlockId AllocateBlock();
}
