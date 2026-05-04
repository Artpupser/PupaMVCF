using PupaMVCF.ExampleProcess.Components;
using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Components;
using PupaMVCF.Framework.Extensions;

namespace PupaMVCF.ExampleProcess.Views;

public sealed class VerifyPageView : View {
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
      sb.Append(
         "<section id='hero' class='bg-black text-light d-flex align-items-center min-vh-100'><div class='container'><div class='row justify-content-center align-items-center'><div class='col-md-8 col-lg-5'><div class='card bg-black border-light shadow rounded-5'><div class='card-body p-5'><h3 class='text-center text-white mb-4 fw-regular'>Верификация</h3>");
      var codeForm = new FormComponent(this, "/verify", "verify");
      codeForm.Push("text", "code", "Код подтверждения");
      await codeForm.Html(request, response, cancellationToken);
      sb.Append(TagJs(response, "index.js"));
      sb.Append("</div></div></div></div></div></section>");
      await End(request, response, cancellationToken);
   }

   public override string Title => "Верификация ✔️";
}