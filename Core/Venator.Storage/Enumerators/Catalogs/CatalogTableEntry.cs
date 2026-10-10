using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Venator.Storage.Contracts.Catalogs;
using Venator.Storage.Contracts.Ids;

namespace Venator.Storage.Enumerators.Catalogs;

[StructLayout(LayoutKind.Auto)]
[DebuggerDisplay("TableEntry: {ToString(),nq}")]
public readonly ref struct CatalogTableEntry(
   TableHeader header,
   ReadOnlySpan<byte> utf8Name,
   CatalogColumnEnumerator columns)
{
   public readonly TableHeader Header = header;
   public readonly ReadOnlySpan<byte> Utf8Name = utf8Name;
   public readonly CatalogColumnEnumerator Columns = columns;

   public TableId Id => Header.Identifier;
   public ushort ColumnCount => Header.ColumnCount;
   public ulong RowCount => Header.RowCount;

   public override string ToString()
      => Encoding.UTF8.GetString(Utf8Name);
}
