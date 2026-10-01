using Venator.Storage.Contracts.Enums;

namespace Venator.Storage.Contracts.Catalogs;

public sealed class ColumnDefinition
{
   public ushort ColumnId { get; init; }

   public required string Name { get; init; }

   public required ColumnKind Kind { get; init; }

   public bool IsNullable { get; init; }

   public byte[]? DefaultValueRawBytes { get; init; }
}
