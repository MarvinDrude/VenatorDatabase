using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Venator.Storage.Contracts.Ids;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
[DebuggerDisplay("ColumnId({Value})")]
public readonly record struct ColumnId(ulong Value) : IComparable<ColumnId>
{
   public static readonly ColumnId Invalid = new(ulong.MaxValue);

   public bool IsValid => Value is not ulong.MaxValue;

   public int CompareTo(ColumnId other) => Value.CompareTo(other.Value);
   public override string ToString() => $"ColumnId#{Value}";

   public static implicit operator ulong(ColumnId id) => id.Value;
   public static explicit operator ColumnId(ulong id) => new(id);
}
