using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Venator.Storage.Contracts.Ids;
using Venator.Storage.Contracts.Interfaces;
using Venator.Storage.Leases;
using Venator.Storage.Pools;

namespace Venator.Storage.Readers.Blocks;

[StructLayout(LayoutKind.Auto)]
public ref struct BlockReader(BufferPool bufferPool, BlockId initialBlockId) : IDisposable
{
   private readonly BufferPool _bufferPool = bufferPool;
   private ReadBlockLease _currentLease = bufferPool.AcquireReadLease(initialBlockId);
   private int _position = 0;

   public BlockId CurrentBlockId => _currentLease.BlockId;
   public readonly int Position => _position;
   public readonly int Remaining => _currentLease.Span.Length - _position;
   public readonly ReadOnlySpan<byte> RemainingSpan => _currentLease.Span[_position..];

   public void LoadBlock(BlockId nextBlockId)
   {
      _currentLease.Dispose();

      _currentLease = _bufferPool.AcquireReadLease(nextBlockId);
      _position = 0;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public T Read<T>() where T : unmanaged
   {
      var size = Unsafe.SizeOf<T>();
      if (_position + size > _currentLease.Span.Length)
         throw new EndOfStreamException($"Cannot read {typeof(T).Name} ({size} bytes); only {Remaining} bytes remain.");

      ref readonly var source = ref Unsafe.As<byte, T>(
         ref MemoryMarshal.GetReference(_currentLease.Span.Slice(_position, size)));

      _position += size;
      return source;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ReadOnlySpan<byte> ReadBytes(int count)
   {
      if (_position + count > _currentLease.Span.Length)
         throw new EndOfStreamException($"Cannot read {count} bytes; only {Remaining} bytes remain.");

      var slice = _currentLease.Span.Slice(_position, count);
      _position += count;

      return slice;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Seek(int position)
   {
      ArgumentOutOfRangeException.ThrowIfGreaterThan(position, _currentLease.Span.Length);
      _position = position;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Skip(int count) => Seek(_position + count);

   public void Dispose()
   {
      _currentLease.Dispose();
   }
}
