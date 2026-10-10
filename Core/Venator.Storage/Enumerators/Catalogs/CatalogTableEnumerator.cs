using System.Runtime.CompilerServices;
using Venator.Storage.Contracts.Catalogs;
using Venator.Storage.Contracts.Catalogs.Extensions;
using Venator.Storage.Readers.Blocks;

namespace Venator.Storage.Enumerators.Catalogs;

public ref struct CatalogTableEnumerator(
   CatalogBlockHeader catalogHeader,
   CatalogHeader rootBlockHeader)
{
   private CatalogBlockHeader _catalogHeader = catalogHeader;
   private readonly CatalogHeader _rootBlockHeader = rootBlockHeader;

   public bool MoveNext(ref BlockReader reader, out CatalogTableEntry table)
   {
      if (reader.Remaining < Unsafe.SizeOf<TableHeader>())
      {
         if (!_catalogHeader.NextBlockId.IsValid)
         {
            table = default;
            return false;
         }

         reader.LoadBlock(_catalogHeader.NextBlockId);
         _catalogHeader = reader.Read<CatalogBlockHeader>().EnsureLittleEndian();
      }

      var tableHeader = reader.Read<TableHeader>().EnsureLittleEndian();
      var name = reader.ReadBytes(tableHeader.NameByteLength);

      var columns = new CatalogColumnEnumerator(tableHeader.ColumnCount);
      table = new CatalogTableEntry(tableHeader, name, columns);

      return true;
   }
}
