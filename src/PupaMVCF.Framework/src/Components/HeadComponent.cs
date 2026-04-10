using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Components;

namespace PupaMVCF.Framework.Components;

public readonly record struct HeadLink(string Rel, string Href);

public sealed class HeadComponent : Component {
   private HeadLink[] Links { get; }
   public string Title { get; }

   public HeadComponent(IComponentParent parent, string title, HeadLink[] links) : base(parent) {
      Title = title;
      Links = links;
   }

   public HeadComponent(View view) : base(view) {
      Title = view.Title;
      Links = view.GetLinks();
   }

   public override Task Html(Request request, Response response, CancellationToken cancellationToken) {
      var sb = Builder;
      sb.Append("<head>");
      sb.Append("<meta charset='UTF-8'>");
      sb.Append(TagTitle(Title));
      sb.Append(TagLink(response, @"icon", $@"{StaticPrefix}icon.webp"));
      sb.Append(TagLink(response, @"preconnect", @"https://challenges.cloudflare.com"));
      foreach (var link in Links)
         sb.Append(TagLink(response, link.Rel, link.Href));
      sb.Append("</head>");
      return Task.CompletedTask;
   }
}