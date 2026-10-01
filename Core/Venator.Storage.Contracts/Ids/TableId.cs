using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Venator.Storage.Contracts.Ids;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
[DebuggerDisplay("TableId({Value})")]
public readonly record struct TableId(ulong Value);
