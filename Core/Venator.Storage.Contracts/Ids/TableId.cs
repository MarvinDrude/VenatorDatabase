using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Venator.Storage.Contracts.Ids;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
[DebuggerDisplay("TableId({Value})")]
public readonly record struct TableId(ulong Value)
{
   public static readonly TableId Invalid = new(ulong.MaxValue);

   public bool IsValid => Value is not ulong.MaxValue;

   public int CompareTo(TableId other) => Value.CompareTo(other.Value);
   public override string ToString() => $"TableId#{Value}";

   public static implicit operator ulong(TableId id) => id.Value;
   public static explicit operator TableId(ulong id) => new(id);
}
