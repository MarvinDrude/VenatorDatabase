using System.Diagnostics;
using System.Runtime.InteropServices;
using Beskar.Memory.Flags;
using Venator.Storage.Contracts.Ids;

namespace Venator.Storage.Contracts.Catalogs;

[StructLayout(LayoutKind.Sequential, Pack = 42)]
[DebuggerDisplay("TableHeader({Identifier,nq})")]
public readonly struct TableHeader
{
   public readonly TableId Identifier;

   public readonly ulong CreatedEpoch;
   public readonly ulong LastModifiedEpoch;

   public readonly ulong RowCount;
   public readonly uint RowGroupCount;

   public readonly ushort ColumnCount;
   public readonly ushort SortKeyCount;

   public readonly ushort NameByteLength;

   public readonly PackedBools32 Flags;
}
