using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Venator.Storage.Contracts.Catalogs.Descriptors;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
[DebuggerDisplay("TableHeader({Identifier,nq})")]
public readonly struct TableHeader
{
   public readonly ulong Identifier;

   public readonly ulong CreatedEpoch;
   public readonly ulong LastModifiedEpoch;

   public readonly ulong RowCount;
   public readonly uint RowGroupCount;

   public readonly ushort ColumnCount;
   public readonly ushort SortKeyCount;

   public readonly ushort NameByteLength;
}
