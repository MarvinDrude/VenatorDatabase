using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Venator.Storage.Contracts.Catalogs;
using Venator.Storage.Contracts.Ids;
using Venator.Storage.Enumerators.Catalogs;
using Venator.Storage.Readers.Blocks;

namespace Venator.Storage.Definitions.Catalogs;

public sealed class TableDefinition
{
   public required TableId Id { get; init; }
   public required string Name { get; init; }

   public required TableHeader Header { get; init; }

   public IReadOnlyList<ColumnDefinition> Columns => _columnsByIndex;
   public int ColumnCount => Columns.Count;

   private readonly FrozenDictionary<string, ColumnDefinition> _columnsByName;
   private readonly ColumnDefinition[] _columnsByIndex;

   public TableDefinition(ColumnDefinition[] columns)
   {
      _columnsByIndex = columns;
      _columnsByName = columns.ToFrozenDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
   }

   public static TableDefinition Create(
      scoped ref BlockReader reader,
      scoped in CatalogTableEntry entry)
   {
      var columns = new ColumnDefinition[entry.ColumnCount];
      var columnEnumerator = entry.Columns;
      var index = 0;

      while (columnEnumerator.MoveNext(ref reader, out var column))
      {
         columns[index++] = ColumnDefinition.Create(column);
      }

      return new TableDefinition(columns)
      {
         Id = entry.Id,
         Header = entry.Header,
         Name = Encoding.UTF8.GetString(entry.Utf8Name),

      };
   }

   public bool TryGetColumn(ReadOnlySpan<char> name, [MaybeNullWhen(false)] out ColumnDefinition column)
   {
      var alternateLookup = _columnsByName.GetAlternateLookup<ReadOnlySpan<char>>();
      return alternateLookup.TryGetValue(name, out column);
   }

   public ColumnDefinition? GetColumn(int index)
   {
      return _columnsByIndex[index];
   }
}
