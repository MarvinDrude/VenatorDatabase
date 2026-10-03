using System.Runtime.InteropServices;
using Venator.Storage.Contracts.Constants;

namespace Venator.Storage.Contracts.Common;

/// <summary>
/// Protecting us against write corruption,
/// commits are done to alpha and beta in an alternating pattern
/// </summary>
[StructLayout(LayoutKind.Explicit, Pack = 1, Size = StorageConstants.WayfinderSize * 2)]
public readonly struct DualWayfinderBlock
{
   /// <summary>
   /// Alpha channel version of the wayfinder
   /// </summary>
   [FieldOffset(0)]
   public readonly WayfinderBlock WayfinderAlpha;

   /// <summary>
   /// Beta channel version of the wayfinder
   /// </summary>
   [FieldOffset(StorageConstants.WayfinderSize)]
   public readonly WayfinderBlock WayfinderBeta;

   public DualWayfinderBlock(WayfinderBlock wayfinderAlpha, WayfinderBlock wayfinderBeta)
   {
      WayfinderAlpha = wayfinderAlpha;
      WayfinderBeta = wayfinderBeta;
   }
}
