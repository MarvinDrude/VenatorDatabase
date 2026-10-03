using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Venator.Storage.Contracts.Constants;
using Venator.Utilities.Files;

namespace Venator.Storage;

public static partial class StorageEngineBootstrapper
{
   public static StorageEngine OpenOrCreate(string filePath, StorageEngineOptions options)
   {
      var fileHandle = DirectFile.Open(filePath);
      var alignment = DeviceAlignment.Query(fileHandle);
      var fileLength = RandomAccess.GetLength(fileHandle);

      if (fileLength == 0)
      {
         return Create(fileHandle, alignment, options);
      }

      var alignedMemory = (byte*)NativeMemory.AlignedAlloc(
         StorageConstants.DualWayfinderSize, alignment.OptimalBlockSize);



      // TODO: real
      return null!;
   }
}
