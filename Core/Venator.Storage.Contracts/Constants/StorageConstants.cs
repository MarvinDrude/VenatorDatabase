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

   public const int SectorSize = 4096;
   public const int WayfinderSize = SectorSize;

   public const long WayfinderAlphaOffset = 0;
   public const long WayfinderBetaOffset = WayfinderAlphaOffset + SectorSize;
}
