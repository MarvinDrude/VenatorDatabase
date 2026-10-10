using System.Diagnostics;
using System.Runtime.InteropServices;
using Venator.Storage.Contracts.Ids;

namespace Venator.Storage.Contracts.Catalogs;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[DebuggerDisplay("CatalogBlockHeader(Next = {NextBlockId,nq})")]
public readonly struct CatalogBlockHeader(
   BlockId nextBlockId)
{
   public readonly BlockId NextBlockId = nextBlockId;
}
