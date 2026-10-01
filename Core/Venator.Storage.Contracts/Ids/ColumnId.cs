using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Venator.Storage.Contracts.Ids;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
[DebuggerDisplay("ColumnId({Value})")]
public readonly record struct ColumnId(ulong Value);
