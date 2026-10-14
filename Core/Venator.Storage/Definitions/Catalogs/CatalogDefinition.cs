using System.Collections.Frozen;
using Venator.Storage.Contracts.Ids;
using Venator.Storage.Readers.Catalogs;

namespace Venator.Storage.Definitions.Catalogs;

public sealed class CatalogDefinition
{
   private readonly FrozenDictionary<string, TableDefinition> _tablesByName;
   private readonly FrozenDictionary<TableId, TableDefinition> _tablesById;

   public int TableCount => _tablesByName.Count;
   public IEnumerable<TableDefinition> Tables => _tablesByName.Values;

   public CatalogDefinition(IEnumerable<TableDefinition> tables)
   {
      var tableArray = tables as TableDefinition[] ?? [.. tables];

      _tablesByName = tableArray.ToFrozenDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
      _tablesById = tableArray.ToFrozenDictionary(x => x.Id);
   }

   public static CatalogDefinition Create(scoped ref CatalogBlockReader catalogReader)
   {
      var tables = new TableDefinition[catalogReader.RootBlockHeader.TableCount];
      var enumerator = catalogReader.Tables;
      var index = 0;

      while (enumerator.MoveNext(ref catalogReader.Reader, out var tableEntry))
      {
         tables[index++] = TableDefinition.Create(ref catalogReader.Reader, in tableEntry);
      }

      return new CatalogDefinition(tables);
   }

   public bool TryGetTable(string name, out TableDefinition? table)
      => _tablesByName.TryGetValue(name, out table);

   public bool TryGetTable(TableId id, out TableDefinition? table)
      => _tablesById.TryGetValue(id, out table);
}
