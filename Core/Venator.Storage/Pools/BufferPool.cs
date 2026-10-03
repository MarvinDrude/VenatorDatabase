using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Venator.Storage.Contracts.Blocks;
using Venator.Storage.Contracts.Ids;
using Venator.Storage.Contracts.Interfaces;
using Venator.Storage.Contracts.Leases;

namespace Venator.Storage.Pools;

public sealed unsafe class BufferPool : IDisposable
{
   private readonly IBlockStorage _storage;
   private readonly byte* _slab;
   private readonly BufferSlot[] _slots;

   public BufferPool(IBlockStorage storage, int slotCount)
   {
      _storage = storage;
      _slots = new BufferSlot[slotCount];

      _slab = (byte*)NativeMemory.AlignedAlloc(
         (nuint)(slotCount * storage.BlockSize), 4096);
   }

   public void Dispose()
   {

   }
}
