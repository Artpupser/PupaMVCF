using System.Text;

using Microsoft.Extensions.Configuration;

using PupaMVCF.Framework.Components;
using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Views;

public abstract class View : Component {
   public abstract string Title { get; }
   protected static string[] _cssFiles = [];
   protected readonly StringBuilder _builder;
   public override StringBuilder Builder => _builder;
   public static IReadOnlyList<string> CssFiles => _cssFiles;

   internal static void LoadCssFiles(IConfiguration configuration) {
      var cssFilesConfigurationSection = configuration.GetSection("Css");
      _cssFiles = cssFilesConfigurationSection.GetChildren().Select(x => $"/api/public/files?name={x.Value}").ToArray();
   }

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