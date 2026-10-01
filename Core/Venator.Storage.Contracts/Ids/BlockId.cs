using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Venator.Storage.Contracts.Ids;

/// <summary>
/// A type safe block id
/// </summary>
/// <param name="Value">The real inner uint value</param>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
[DebuggerDisplay("BlockId({Value})")]
public readonly record struct BlockId(uint Value);
