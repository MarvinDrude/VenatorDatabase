using System.Runtime.InteropServices;
using Venator.Storage.Contracts.Ids;

namespace Venator.Storage.Contracts.Blocks;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct BitmapBlockHeader(
    uint magicNumber,
    uint version,
    BlockId nextBitmapBlockId,
    uint firstTrackedBlockId,
    uint totalTrackedBlocks,
    uint freeBlockCount,
    uint searchHintWordIndex)
{

}
