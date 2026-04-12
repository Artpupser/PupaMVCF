using PupaMVCF.Framework.Components;
using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Tests.Components;


namespace PupaMVCF.Framework.Tests.Views;

public sealed class TestView : View {
   public override HeadLink[] GetLinks() {
      return [new HeadLink("stylesheet", $"{StaticPrefix}bootstrap.css")];
   }

   public override async Task Html(Request request, Response response, CancellationToken cancellationToken) {
      await Start(request, response, string.Empty, cancellationToken);
      var sb = Builder;
      sb.Append("""
                <section id='test'>
                  <h1>Hello<h1>
                </section>
                """);
      await End(request, response, cancellationToken);
   }

   public override string Title => "Test title";
}