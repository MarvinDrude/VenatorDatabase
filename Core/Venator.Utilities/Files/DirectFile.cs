using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Venator.Utilities.Files;

public static partial class DirectFile
{
   /// <summary>
   /// Opens a file with DIRECT. No OS-Level caching on purpose.
   /// </summary>
   public static SafeFileHandle Open(string filePath)
   {
      try
      {
         if (OperatingSystem.IsWindows())
         {
            // https://learn.microsoft.com/en-us/windows/win32/fileio/file-buffering
            return OpenWindows(filePath);
         }

         if (OperatingSystem.IsLinux())
         {
            // https://man7.org/linux/man-pages/man2/open.2.html
            return OpenLinux(filePath);
         }
      }
      catch (Exception)
      {
         // ignored
      }

      return OpenStandardFallback(filePath);
   }

   // reading below this, anyone may behold: this is OS madness

   private static SafeFileHandle OpenWindows(string path)
   {
      const uint genericRead = 0x80000000;
      const uint genericWrite = 0x40000000;

      const uint fileShareRead = 1;
      const uint fileShareWrite = 2;
      const uint openAlways = 4;

      // 0x20000000 = FILE_FLAG_NO_BUFFERING (Direct DMA to our aligned slab)
      // 0x80000000 = FILE_FLAG_WRITE_THROUGH (Writes bypass OS cache)
      const uint fileFlagNoBuffering = 0x20000000;
      const uint fileFlagWriteThrough = 0x80000000;

      var handle = CreateFileW(
         path,
         genericRead | genericWrite,
         fileShareRead | fileShareWrite,
         nint.Zero,
         openAlways,
         fileFlagNoBuffering | fileFlagWriteThrough,
         nint.Zero);

      if (handle.IsInvalid)
      {
         Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
      }

      return handle;
   }

   private static SafeFileHandle OpenLinux(string path)
   {
      // Linux x86_64 / ARM64 constants from <fcntl.h>
      const int oRdwr = 0x0002;
      const int oCreat = 0x0040; // 0100 octal
      const int oDirect = 0x4000; // 040000 octal (Bypasses Linux Page Cache)
      const int oDsync = 0x1000; // 010000 octal (Data sync to physical flash)

      // Permissions: 0666 (rw-rw-rw- masked by umask)
      const int mode = 438;
      var fd = Open(path, oRdwr | oCreat | oDirect | oDsync, mode);

      if (fd < 0)
      {
         var errno = Marshal.GetLastPInvokeError();
         throw new IOException($"Linux open(O_DIRECT) failed with errno: {errno}");
      }

      return new SafeFileHandle(fd, ownsHandle: true);
   }

   private static SafeFileHandle OpenStandardFallback(string path)
   {
      return File.OpenHandle(
         path,
         FileMode.OpenOrCreate,
         FileAccess.ReadWrite,
         FileShare.ReadWrite,
         FileOptions.WriteThrough);
   }

   [LibraryImport("kernel32.dll", EntryPoint = "CreateFileW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
   private static partial SafeFileHandle CreateFileW(
      string lpFileName,
      uint dwDesiredAccess,
      uint dwShareMode,
      nint lpSecurityAttributes,
      uint dwCreationDisposition,
      uint dwFlagsAndAttributes,
      nint hTemplateFile);

   [LibraryImport("libc", EntryPoint = "open", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
   private static partial int Open(string pathname, int flags, int mode);
}
