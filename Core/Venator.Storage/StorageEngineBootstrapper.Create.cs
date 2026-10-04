using Microsoft.Win32.SafeHandles;
using Venator.Utilities.Files;

namespace Venator.Storage;

public static partial class StorageEngineBootstrapper
{
   private static StorageEngine Create(
      SafeFileHandle handle,
      in StorageAlignmentInfo alignmentInfo,
      StorageEngineOptions options)
   {
      // TODO: MDE
      return null!;
   }
}
