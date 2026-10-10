using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Venator.Storage.Contracts.Catalogs;
using Venator.Storage.Contracts.Enums;
using Venator.Storage.Contracts.Ids;

namespace Venator.Storage.Enumerators.Catalogs;

[StructLayout(LayoutKind.Auto)]
[DebuggerDisplay("ColumnEntry: {ToString(),nq}")]
public readonly ref struct CatalogColumnEntry(
   ColumnHeader header,
   ReadOnlySpan<byte> utf8Name,
   ReadOnlySpan<byte> defaultValue)
{
   public readonly ColumnHeader Header = header;
   public readonly ReadOnlySpan<byte> Utf8Name = utf8Name;
   public readonly ReadOnlySpan<byte> DefaultValue = defaultValue;

   public ColumnId Id => Header.Identifier;
   public ColumnKind Kind => Header.Kind;

   public bool IsNullable => Header.IsNullable;
   public bool HasDefaultValue => Header.HasDefaultValue;

   public override string ToString() => Encoding.UTF8.GetString(Utf8Name);
}
