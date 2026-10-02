using System.Runtime.InteropServices;

namespace Venator.Storage.Contracts.Common;

/// <summary>
/// Protecting us against write corruption,
/// commits are done to alpha and beta in an alternating pattern
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4096 * 2)]
public readonly struct DualWayfinderBlock
{
   public readonly WayfinderBlock WayfinderAlpha;

   public readonly WayfinderBlock WayfinderBeta;
}
