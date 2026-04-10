using PupaMVCF.Framework.Components;
using PupaMVCF.Framework.Core;
using PupaMVCF.Web.Template.Components;

namespace PupaMVCF.Web.Template.Views;

public class TemplateView : View {
   public override HeadLink[] GetLinks() {
      return [new HeadLink("stylesheet", $"{StaticPrefix}bootstrap.css")];
   }

   protected override async Task Start(Request request, Response response, string stylesForBody,
      CancellationToken cancellationToken) {
      await base.Start(request, response, stylesForBody, cancellationToken);
      await new HeaderComponent(this).Html(request, response, cancellationToken);
   }

   protected override async Task End(Request request, Response response, CancellationToken cancellationToken) {
      await base.End(request, response, cancellationToken);
      await new FooterComponent(this).Html(request, response, cancellationToken);
   }

   public override async Task Html(Request request, Response response, CancellationToken cancellationToken) {
      await Start(request, response, string.Empty, cancellationToken);
      var sb = Builder;
      sb.Append("""
                <section id='hero' class='bg-black text-light d-flex align-items-center py-5 min-vh-100'>
                    <div class='container'>
                        <div class='row justify-content-center text-center'>
                            <div class='col-lg-8 col-xl-8 p-5'>
                                <h1 class='fade show display-3 fw-bold mb-4'>Привет мир! 🌍</h1>
                                <p class='lead fs-2'>Начало начал!</p>
                            </div>
                        </div>
                    </div>
                </section>
                """);

      await End(request, response, cancellationToken);
   }

   public override string Title => "Привет мир! 🌍";
}