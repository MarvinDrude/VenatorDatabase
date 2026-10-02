using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Venator.Storage.Contracts.Ids;

/// <summary>
/// A type safe block id
/// </summary>
/// <param name="Value">The real inner ulong value</param>
[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[DebuggerDisplay("BlockId({Value})")]
public readonly record struct BlockId(ulong Value) : IComparable<BlockId>
{
   public static readonly BlockId Invalid = new(ulong.MaxValue);

   public bool IsValid => Value is not ulong.MaxValue;

   public int CompareTo(BlockId other) => Value.CompareTo(other.Value);
   public override string ToString() => $"Block#{Value}";

   public static implicit operator ulong(BlockId id) => id.Value;
   public static explicit operator BlockId(ulong id) => new(id);
}
