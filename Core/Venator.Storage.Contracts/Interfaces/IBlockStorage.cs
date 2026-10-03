using System.Runtime.CompilerServices;
using Venator.Storage.Contracts.Ids;

namespace Venator.Storage.Contracts.Interfaces;

public interface IBlockStorage : IDisposable
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
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void ReadBlock(BlockId blockId, Span<byte> destination);

   /// <summary>
   /// Write block bytes of a given block id
   /// </summary>
   public void WriteBlock(BlockId blockId, ReadOnlySpan<byte> data);

   /// <summary>
   /// Appends new block (can be a reclaimed one, or appended at the end)
   /// </summary>
   /// <returns></returns>
   public BlockId AllocateBlock();

   /// <summary>
   /// Flush to disk
   /// </summary>
   public void Flush();
}
