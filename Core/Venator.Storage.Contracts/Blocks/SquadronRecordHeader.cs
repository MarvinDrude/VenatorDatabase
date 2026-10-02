using System.Runtime.InteropServices;
using Venator.Storage.Contracts.Ids;

namespace Venator.Storage.Contracts.Blocks;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 20)]
public readonly struct SquadronRecordHeader
{
   public readonly RowGroupId Id;
   public readonly ulong CreatedEpoch;
   public readonly uint RowCount;
}
