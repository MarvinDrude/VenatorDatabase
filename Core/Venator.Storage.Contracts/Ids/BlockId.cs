using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Venator.Storage.Contracts.Ids;

/// <summary>
/// A type safe block id
/// </summary>
/// <param name="Value">The real inner ulong value</param>
[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[DebuggerDisplay("{ToString(),nq}")]
public readonly record struct BlockId(ulong Value) : IComparable<BlockId>
{
   public static readonly BlockId Invalid = new(ulong.MaxValue);

   public long FileOffset => (long)Value * StorageConstants.BlockSize;
   public bool IsValid => Value is not ulong.MaxValue;

   public int CompareTo(BlockId other) => Value.CompareTo(other.Value);
   public override string ToString() => $"Block#{Value} (@{FileOffset:N0}B)";
}
