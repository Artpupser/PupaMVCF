using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Views;

namespace PupaMVCF.Framework.Components;

public sealed class HeadComponent : Component {
   public IReadOnlyList<string> CssFiles { get; }
   public string Title { get; }

   public HeadComponent(IComponentParent parent, string title, IReadOnlyList<string> cssFiles) : base(parent) {
      Title = title;
      CssFiles = cssFiles;
   }

   public HeadComponent(View view) : base(view) {
      Title = view.Title;
      CssFiles = View.CssFiles;
   }

   public override Task Html(Request request, Response response, CancellationToken cancellationToken) {
      var sb = Builder;
      sb.Append("<head>");
      sb.Append("<meta charset='UTF-8'>");
      sb.Append(TagTitle(Title));
      sb.Append(TagLink(response, @"icon", @"/api/public/files?name=icon.webp"));
      sb.Append(TagLink(response, @"preconnect", @"https://challenges.cloudflare.com"));
      foreach (var cssFile in CssFiles)
         sb.Append(TagLink(response, @"stylesheet", cssFile));
      sb.Append("</head>");
      return Task.CompletedTask;
   }
}