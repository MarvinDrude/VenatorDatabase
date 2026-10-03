using System.Runtime.InteropServices;
using Venator.Storage.Contracts.Ids;

namespace Venator.Storage.Contracts.Blocks;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 20)]
public readonly struct SquadronRecordHeader(
   RowGroupId id,
   ulong createdEpoch,
   uint rowCount)
{
   public readonly RowGroupId Id = id;
   public readonly ulong CreatedEpoch = createdEpoch;
   public readonly uint RowCount = rowCount;
}
