using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Venator.Storage.Contracts.Catalogs;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[DebuggerDisplay("CatalogHeader(Count = {TableCount,nq})")]
public readonly struct CatalogHeader(
   uint magicNumber,
   ushort version,
   ushort tableCount,
   ulong checksum)
{
   /// <summary>
   /// Magic number: VCAT
   /// </summary>
   public readonly uint MagicNumber = magicNumber;

   /// <summary>
   /// The catalog version
   /// </summary>
   public readonly ushort Version = version;

   /// <summary>
   /// Total table count in database
   /// </summary>
   public readonly ushort TableCount = tableCount;

   /// <summary>
   /// Checksum to check
   /// </summary>
   public readonly ulong Checksum = checksum;
}
