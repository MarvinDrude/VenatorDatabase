using Microsoft.Win32.SafeHandles;
using Venator.Utilities.Files;

namespace Venator.Storage;

public static class StorageEngineBootstrapper
{
   public static StorageEngine Open(string filePath)
   {
      var fileHandle = DirectFile.Open(filePath);


      // TODO: real
      return null!;
   }
}
