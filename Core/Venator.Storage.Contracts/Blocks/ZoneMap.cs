using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Beskar.Memory.Flags;

namespace Venator.Storage.Contracts.Blocks;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 40)]
public unsafe struct ZoneMap
{
   public readonly uint NullCount;
   public readonly uint RowCount;

   private fixed byte _minRaw[16];
   private fixed byte _maxRaw[16];

   public ZoneMap(
      ReadOnlySpan<byte> min, ReadOnlySpan<byte> max,
      uint nullCount, uint rowCount)
   {
      NullCount = nullCount;
      RowCount = rowCount;

      fixed (byte* minPtr = _minRaw, maxPtr = _maxRaw)
      {
         min[..Math.Min(min.Length, 16)].CopyTo(new Span<byte>(minPtr, 16));
         max[..Math.Min(max.Length, 16)].CopyTo(new Span<byte>(maxPtr, 16));
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public T GetMin<T>() where T : unmanaged
      => Unsafe.ReadUnaligned<T>(ref Unsafe.AsRef(ref _minRaw[0]));

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public T GetMax<T>() where T : unmanaged
      => Unsafe.ReadUnaligned<T>(ref Unsafe.AsRef(ref _maxRaw[0]));

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool CanSkipEquals<T>(T value)
      where T : unmanaged, IComparable<T>
   {
      if (NullCount == RowCount)
      {
         return true;
      }

      return value.CompareTo(GetMin<T>()) < 0
         || value.CompareTo(GetMax<T>()) > 0;
   }
}
