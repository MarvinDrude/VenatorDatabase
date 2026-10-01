using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Venator.Storage.Contracts.Catalogs.Descriptors;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[DebuggerDisplay("CatalogHeader(Count = {TableCount,nq})")]
public readonly struct CatalogHeader
{
   /// <summary>
   /// Magic number: VCAT
   /// </summary>
   public readonly uint MagicNumber;

   /// <summary>
   /// The catalog version
   /// </summary>
   public readonly ushort Version;

   /// <summary>
   /// Total table count in database
   /// </summary>
   public readonly ushort TableCount;

   /// <summary>
   /// Checksum to check
   /// </summary>
   public readonly ulong Checksum;
}
