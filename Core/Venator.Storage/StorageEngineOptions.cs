namespace Venator.Storage;

public sealed class StorageEngineOptions
{
   /// <summary>
   /// The preferred block size. Defaults to optimal size if 0.
   /// </summary>
   public uint PreferredBlockSize { get; init; } = 65_536;

   /// <summary>
   /// How many blocks can be active in memory.
   /// </summary>
   public int BufferPoolSlots { get; init; } = 8_192;
}
