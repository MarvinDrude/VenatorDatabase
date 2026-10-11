using System.Collections.Frozen;
using Venator.Storage.Contracts.Ids;

namespace Venator.Storage.Definitions.Catalogs;

public sealed class CatalogDefinition
{
   private readonly FrozenDictionary<string, TableDefinition> _tablesByName;
   private readonly FrozenDictionary<TableId, TableDefinition> _tablesById;

   public int TableCount => _tablesByName.Count;
   public IEnumerable<TableDefinition> Tables => _tablesByName.Values;

   public bool TryGetTable(string name, out TableDefinition? table)
      => _tablesByName.TryGetValue(name, out table);

   public bool TryGetTable(TableId id, out TableDefinition? table)
      => _tablesById.TryGetValue(id, out table);
}
