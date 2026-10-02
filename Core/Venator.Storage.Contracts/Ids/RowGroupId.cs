using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Venator.Storage.Contracts.Ids;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[DebuggerDisplay("BlockId({Value})")]
public readonly record struct RowGroupId(ulong Value) : IComparable<RowGroupId>
{
   public static readonly RowGroupId Invalid = new(ulong.MaxValue);

   public bool IsValid => Value is not ulong.MaxValue;

   public int CompareTo(RowGroupId other) => Value.CompareTo(other.Value);
   public override string ToString() => $"RowGroupId#{Value}";

   public static implicit operator ulong(RowGroupId id) => id.Value;
   public static explicit operator RowGroupId(ulong id) => new(id);
}
