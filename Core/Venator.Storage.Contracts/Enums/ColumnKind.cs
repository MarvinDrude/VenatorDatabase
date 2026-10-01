namespace Venator.Storage.Contracts.Enums;

/// <summary>
/// The data type for a given column
/// </summary>
public enum ColumnKind : byte
{
   // Integers
   Int8,
   Int16,
   Int32,
   Int64,

   // Unsigned integers
   UInt8,
   UInt16,
   UInt32,
   UInt64,

   // Floating points
   Float32,
   Float64,

   // Other
   Boolean,
   TimestampUtc,
   String,
   Guid
}
