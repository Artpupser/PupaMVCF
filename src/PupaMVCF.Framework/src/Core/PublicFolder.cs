using PupaLib.FileIO;

namespace PupaMVCF.Framework.Core;

public sealed class PublicFolder {
   public readonly VirtualFolder Virtual = VirtualIo.RootFolder.GetOrCreateFolderIn("public");
}