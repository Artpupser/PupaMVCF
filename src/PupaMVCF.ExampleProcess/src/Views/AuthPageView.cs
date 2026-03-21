using PupaMVCF.ExampleProcess.Components;
using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Views;

namespace PupaMVCF.ExampleProcess.Views;

public sealed class AuthPageView : View {
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
         "<section id='hero' class='bg-black text-light d-flex align-items-center min-vh-100'><div class='container'><div class='row justify-content-center align-items-center'><div class='col-md-8 col-lg-5'><div class='card bg-black border-light shadow rounded-5'><div class='card-body p-5'><h3 class='text-center text-white mb-4 fw-regular'>Вход</h3>");
      var loginForm = new FormComponent(this, "/login", "login");
      loginForm.Push("email", "email", "Электронная почта");
      loginForm.Push("captcha", "captcha", "captcha");
      await loginForm.Html(request, response, cancellationToken);
      sb.Append(
         "</div></div></div><div class='col-md-8 col-lg-5'><div class='card bg-black border-light shadow rounded-5'><div class='card-body p-5'><h3 class='text-center text-white mb-4 fw-regular'>Регистрация</h3>");
      var registrationForm = new FormComponent(this, "/registration", "registration");
      registrationForm.Push("text", "username", "Логин");
      registrationForm.Push("email", "email", "Электронная почта");
      registrationForm.Push("captcha", "captcha", "captcha");
      await registrationForm.Html(request, response, cancellationToken);
      sb.Append(TagJs(response, "verify.js"));
      sb.Append("</div></div></div></div></div></section>");
      await End(request, response, cancellationToken);
   }

   public override string Title => "Аунтентифицация 🔑";
}