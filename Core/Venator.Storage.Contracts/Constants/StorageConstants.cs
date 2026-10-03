namespace Venator.Storage.Contracts.Constants;

/// <summary>
/// Important constants for the storage engine
/// </summary>
public static class StorageConstants
{
   /// <summary>
   /// VENATOR\0
   /// </summary>
   public const ulong MagicNumber = 0x56454E41544F5200;

   /// <summary>
   /// VCAT
   /// </summary>
   public const uint CatalogMagicNumber = 0x54414356;

   /// <summary>
   /// The Format version
   /// </summary>
   public const uint FormatVersion = 1;

   public const int WayfinderSize = 4096;
   public const int DualWayfinderSize = WayfinderSize * 2;

   public const long WayfinderAlphaOffset = 0;
   public const long WayfinderBetaOffset = WayfinderAlphaOffset + WayfinderSize;
}
