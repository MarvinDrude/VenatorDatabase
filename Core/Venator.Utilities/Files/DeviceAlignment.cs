using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Venator.Utilities.Files;

public readonly record struct StorageAlignmentInfo(
   uint MemoryAlignment, // for aligned alloc
   uint SectorSize, // read write multiple for direct io
   uint OptimalBlockSize // size optimal for nvme ssd controller
);

public static unsafe partial class DeviceAlignment
{
   /// <summary>
   /// Discovers the exact hardware sector size and optimal memory alignment for a given file handle.
   /// </summary>
   public static StorageAlignmentInfo Query(SafeFileHandle handle)
   {
      var cpuPageSize = (uint)Environment.SystemPageSize;
      uint sectorSize = 4096;
      uint optimalBlock = 4096;
      var memAlign = cpuPageSize;

      try
      {
         if (OperatingSystem.IsWindows())
         {
            if (QueryWindows(handle, out var info))
            {
               sectorSize   = info.LogicalBytesPerSector > 0 ? info.LogicalBytesPerSector : 4096;
               optimalBlock = info.PhysicalBytesPerSectorForPerformance > 0
                  ? info.PhysicalBytesPerSectorForPerformance
                  : info.PhysicalBytesPerSectorForAtomicity;

               // direct io requires memory address to be aligned to sector boundary
               memAlign = Math.Max(sectorSize, cpuPageSize);
            }
         }
         else if (OperatingSystem.IsLinux())
         {
            if (QueryLinux(handle, out var dioMemAlign, out var dioOffsetAlign))
            {
               sectorSize = dioOffsetAlign > 0 ? dioOffsetAlign : 4096;
               memAlign   = Math.Max(dioMemAlign, cpuPageSize);
               optimalBlock = Math.Max(sectorSize, 4096);
            }
         }
      }
      catch
      {
         // fallback to a safe default
      }

      memAlign = Math.Max(memAlign, 4096);
      if ((memAlign & (memAlign - 1)) != 0)
      {
         memAlign = RoundUpToPowerOfTwo(memAlign);
      }

      return new StorageAlignmentInfo(memAlign, sectorSize, Math.Max(optimalBlock, 4096));
   }

   [StructLayout(LayoutKind.Sequential)]
   private struct FileStorageInfo
   {
      public uint LogicalBytesPerSector;
      public uint PhysicalBytesPerSectorForAtomicity;
      public uint PhysicalBytesPerSectorForPerformance;
      public uint FileSystemEffectivePhysicalBytesPerSectorForAtomicity;
      public uint Flags;
      public uint ByteOffsetForSectorAlignment;
      public uint ByteOffsetForPartitionAlignment;
   }

   private static bool QueryWindows(SafeFileHandle handle, out FileStorageInfo info)
   {
      info = default;
      const int fileStorageInfoClass = 16;

      fixed (FileStorageInfo* ptr = &info)
      {
         return GetFileInformationByHandleEx(
            handle,
            fileStorageInfoClass,
            (nint)ptr,
            (uint)sizeof(FileStorageInfo));
      }
   }

   [StructLayout(LayoutKind.Explicit, Size = 256)]
   private struct StatxBuffer
   {
      [FieldOffset(0x00)] public uint stx_mask;
      [FieldOffset(0x08)] public uint stx_blksize;
      [FieldOffset(0xA0)] public uint stx_dio_mem_align;
      [FieldOffset(0xA4)] public uint stx_dio_offset_align;
   }

   private static bool QueryLinux(SafeFileHandle handle, out uint memAlign, out uint offsetAlign)
   {
      memAlign = 4096;
      offsetAlign = 4096;

      const int atEmptyPath = 0x1000;
      const uint statxDioalign = 0x00002000;

      StatxBuffer buf = default;
      var res = Statx((int)handle.DangerousGetHandle(), "", atEmptyPath, statxDioalign, &buf);

      if (res == 0)
      {
         if ((buf.stx_mask & statxDioalign) != 0)
         {
            memAlign    = buf.stx_dio_mem_align;
            offsetAlign = buf.stx_dio_offset_align;
            return true;
         }
         if (buf.stx_blksize > 0)
         {
            offsetAlign = buf.stx_blksize;
            memAlign    = buf.stx_blksize;
            return true;
         }
      }

      return false;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static uint RoundUpToPowerOfTwo(uint value)
   {
      value--;
      value |= value >> 1;
      value |= value >> 2;
      value |= value >> 4;
      value |= value >> 8;
      value |= value >> 16;
      return value + 1;
   }

   [LibraryImport("kernel32.dll", SetLastError = true)]
   [return: MarshalAs(UnmanagedType.Bool)]
   private static partial bool GetFileInformationByHandleEx(
      SafeFileHandle hFile,
      int fileInformationClass,
      nint lpFileInformation,
      uint dwBufferSize);

   [LibraryImport("libc", EntryPoint = "statx", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
   private static partial int Statx(
      int dirfd,
      string pathname,
      int flags,
      uint mask,
      StatxBuffer* statxbuf);
}
