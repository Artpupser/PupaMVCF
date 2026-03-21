using PupaLib.FileIO;

using PupaMVCF.Framework.Core.Extra;
using PupaMVCF.Framework.Extensions;

namespace PupaMVCF.Framework.Core;

public enum MimeContentType : byte {
   Html = 0,
   Css = 1,
   Js = 2,
   Json = 3,
   FontOTF = 4,
   FontWOFF2 = 5,
   Text = 6,
   Webp = 7,
   Jpeg = 8,
   Png = 9,
   Xml = 10,
   Undefined = byte.MaxValue
}

public static class MimeContent {
   private static readonly BiFrozenDictionary<string, MimeContentType> _mimeContentType =
      new Dictionary<string, MimeContentType> {
         ["text/html"] = MimeContentType.Html,
         ["text/css"] = MimeContentType.Css,
         ["text/javascript"] = MimeContentType.Js,
         ["application/json"] = MimeContentType.Json,
         ["font/otf"] = MimeContentType.FontOTF,
         ["font/woff2"] = MimeContentType.FontWOFF2,
         ["text/plain"] = MimeContentType.Text,
         ["image/webp"] = MimeContentType.Webp,
         ["image/jpeg"] = MimeContentType.Jpeg,
         ["image/png"] = MimeContentType.Png,
         ["application/xml"] = MimeContentType.Xml,
         [string.Empty] = MimeContentType.Undefined
      }.ToBiFrozenDictionary();

   private static readonly BiFrozenDictionary<string, MimeContentType> _fileExtensionMimeType =
      new Dictionary<string, MimeContentType> {
         ["html"] = MimeContentType.Html,
         ["css"] = MimeContentType.Css,
         ["js"] = MimeContentType.Js,
         ["json"] = MimeContentType.Json,
         ["otf"] = MimeContentType.FontOTF,
         ["woff2"] = MimeContentType.FontWOFF2,
         ["txt"] = MimeContentType.Text,
         ["webp"] = MimeContentType.Webp,
         ["jpeg"] = MimeContentType.Jpeg,
         ["png"] = MimeContentType.Png,
         ["xml"] = MimeContentType.Xml,
         [string.Empty] = MimeContentType.Undefined
      }.ToBiFrozenDictionary();

   public static string GetExtensionFile(MimeContentType value) {
      return _fileExtensionMimeType.HardGetKey(value);
   }

   public static string GetExtensionFile(string? mimeContentType) {
      return _fileExtensionMimeType.HardGetKey(
         _mimeContentType.TryGetValue(mimeContentType ?? string.Empty, out var type)
            ? type
            : MimeContentType.Undefined);
   }

   public static MimeContentType GetMimeType(VirtualFile value) {
      return _fileExtensionMimeType.TryGetValue(value.ExtensionWithoutDot, out var type)
         ? type
         : MimeContentType.Undefined;
   }

   public static MimeContentType GetMimeType(string? mimeType) {
      return _mimeContentType.TryGetValue(mimeType ?? string.Empty, out var type) ? type : MimeContentType.Undefined;
   }

   public static string GetMimeTypeStr(VirtualFile value) {
      return _mimeContentType.HardGetKey(_fileExtensionMimeType.HardGetValue(value.ExtensionWithoutDot));
   }

   public static string GetMimeTypeStr(MimeContentType value) {
      return _mimeContentType.HardGetKey(value);
   }
}