using Venator.Storage.Contracts.Catalogs;
using Venator.Storage.Contracts.Catalogs.Extensions;
using Venator.Storage.Readers.Blocks;

namespace Venator.Storage.Enumerators.Catalogs;

public ref struct CatalogColumnEnumerator(ushort columnCount)
{
   private readonly ushort _columnCount = columnCount;
   private ushort _currentIndex;

   public readonly ushort RemainingCount => (ushort)(_columnCount - _currentIndex);
   public readonly bool IsCompleted => _currentIndex >= _columnCount;

   public bool MoveNext(ref BlockReader reader, out CatalogColumnEntry column)
   {
      if (_currentIndex >= _columnCount)
      {
         column = default;
         return false;
      }

      var header = reader.Read<ColumnHeader>().EnsureLittleEndian();
      var name = reader.ReadBytes(header.NameByteLength);

      var defaultValue = header is { HasDefaultValue: true, DefaultValueByteLength: > 0 }
         ? reader.ReadBytes(header.DefaultValueByteLength)
         : default;

      column = new CatalogColumnEntry(header, name, defaultValue);
      _currentIndex++;

      return true;
   }

   public void SkipRemaining(ref BlockReader reader)
   {
      while (_currentIndex < _columnCount)
      {
         var header = reader.Read<ColumnHeader>().EnsureLittleEndian();

         var bytesToSkip = header.NameByteLength;
         if (header.HasDefaultValue)
         {
            bytesToSkip += header.DefaultValueByteLength;
         }

         reader.Skip(bytesToSkip);
         _currentIndex++;
      }
   }
}
