using System.Runtime.InteropServices;
using Venator.Storage.Contracts.Catalogs;
using Venator.Storage.Contracts.Catalogs.Extensions;
using Venator.Storage.Contracts.Ids;
using Venator.Storage.Enumerators.Catalogs;
using Venator.Storage.Pools;
using Venator.Storage.Readers.Blocks;

namespace Venator.Storage.Readers.Catalogs;

[StructLayout(LayoutKind.Auto)]
public ref struct CatalogBlockReader : IDisposable
{
   private BlockReader _reader;
   private readonly ReadOnlySpan<byte> _span;
   private int _position;

   public CatalogBlockHeader BlockHeader { get; }
   public CatalogHeader RootBlockHeader { get; }

   public CatalogBlockReader(BufferPool bufferPool, BlockId rootCatalogBlockId)
   {
      _reader = new BlockReader(bufferPool, rootCatalogBlockId);

      BlockHeader = _reader.Read<CatalogBlockHeader>().EnsureLittleEndian();
      RootBlockHeader = _reader.Read<CatalogHeader>().EnsureLittleEndian();
   }

   public CatalogTableEnumerator Tables => new(BlockHeader, RootBlockHeader);

   public void Dispose()
   {
      _reader.Dispose();
   }
}
