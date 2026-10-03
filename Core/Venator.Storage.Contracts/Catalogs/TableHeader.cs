using System.Diagnostics;
using System.Runtime.InteropServices;
using Beskar.Memory.Flags;
using Venator.Storage.Contracts.Ids;

namespace Venator.Storage.Contracts.Catalogs;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 46)]
[DebuggerDisplay("TableHeader({Identifier,nq})")]
public readonly struct TableHeader(
   TableId identifier,
   ulong createdEpoch,
   ulong lastModifiedEpoch,
   ulong rowCount,
   uint rowGroupCount,
   ushort columnCount,
   ushort sortKeyCount,
   ushort nameByteLength,
   PackedBools32 flags)
{
   public readonly TableId Identifier = identifier;

   public readonly ulong CreatedEpoch = createdEpoch;
   public readonly ulong LastModifiedEpoch = lastModifiedEpoch;

   public readonly ulong RowCount = rowCount;
   public readonly uint RowGroupCount = rowGroupCount;

   public readonly ushort ColumnCount = columnCount;
   public readonly ushort SortKeyCount = sortKeyCount;

   public readonly ushort NameByteLength = nameByteLength;

   public readonly PackedBools32 Flags = flags;
}
