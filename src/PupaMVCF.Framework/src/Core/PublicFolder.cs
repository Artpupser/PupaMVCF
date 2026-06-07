using PupaLib.FileIO;

namespace PupaMVCF.Framework.Core;

public sealed class PublicFolder {
   public readonly VirtualFolder Virtual; 

   public PublicFolder()
   {
      var optionFolder = VirtualIo.RootFolder.GetOrCreateFolderIn("public");
      if (!optionFolder.Out(out var folder)) 
         throw VirtualFolder.NotFoundException(VirtualIo.RootFolder.BuildPath("public"));
      Virtual = folder;
   }
}
