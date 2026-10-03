using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Venator.Storage.Contracts.Interfaces;

namespace Venator.Storage.Readers.Blocks;

[StructLayout(LayoutKind.Auto)]
public ref struct BlockReader(IBlockStorage blockStorage)
{

}
