using System.Text;
using Venator.Storage.Contracts.Catalogs;
using Venator.Storage.Contracts.Enums;
using Venator.Storage.Contracts.Ids;
using Venator.Storage.Enumerators.Catalogs;

namespace Venator.Storage.Definitions.Catalogs;

public sealed class ColumnDefinition
{
   public required ColumnId Id { get; init; }
   public required string Name { get; init; }

   public required ColumnKind Kind { get; init; }
   public required EncodingKind EncodingType { get; init; }
   public required CompressionKind CompressionType { get; init; }

   public required ColumnHeader Header { get; init; }

   public byte[]? DefaultValue { get; init; }

   public bool IsNullable => Header.IsNullable;
   public bool HasDefaultValue => Header.HasDefaultValue;

   public static ColumnDefinition Create(scoped in CatalogColumnEntry entry)
   {
      return new ColumnDefinition()
      {
         Id = entry.Id,
         Header = entry.Header,
         Name = Encoding.UTF8.GetString(entry.Utf8Name),
         Kind = entry.Kind,
         DefaultValue = [.. entry.DefaultValue],
         CompressionType = entry.Header.CompressionType,
         EncodingType = entry.Header.EncodingType
      };
   }
}
