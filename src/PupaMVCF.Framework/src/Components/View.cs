using System.Text;

using Microsoft.Extensions.Configuration;

using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Extensions;

namespace PupaMVCF.Framework.Components;

public abstract class View : Component {
   public abstract string Title { get; }
   protected readonly StringBuilder _builder;
   public override StringBuilder Builder => _builder;
   public abstract HeadLink[] GetLinks();

   protected View() : base(null) {
      _builder = new StringBuilder();
   }

   protected virtual async Task Start(Request request, Response response, string stylesForBody,
      CancellationToken cancellationToken) {
      Builder.Append("<!DOCTYPE html><html lang='en'>");
      var head = new HeadComponent(this);
      await head.Html(request, response, cancellationToken);
      Builder.Append($"<body class='{stylesForBody}'>");
   }

   protected virtual Task End(Request request, Response response, CancellationToken cancellationToken) {
      Builder.Append(TagJs(response, "bootstrap.js"));
      Builder.Append("</body></html>");
      return Task.CompletedTask;
   }
}