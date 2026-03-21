using PupaMVCF.ExampleProcess.Components;
using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Views;

namespace PupaMVCF.ExampleProcess.Views;

public sealed class ErrorView : View {
   public ErrorView(Response response) {
      Title = $"{response.StatusCode}";
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
      await Start(request, response, "bg-black text-white min-vh-100", cancellationToken);
      var sb = Builder;
      var titleLabel = response.StatusCode == 404 ? "Страница не найдена" : "Произошла ошибка";
      sb.Append($"""
                 <div id='hero' class='container d-flex min-vh-100 justify-content-center align-items-center p-3'>
                     <div class='row justify-content-center'>
                         <div class='col-lg-8 col-xl-8 text-center p-5'>
                             <div class='display-1 fw-bold text-danger mb-4'>{response.StatusCode}</div>
                             <h1 class='display-4 fw-bold mb-4'>{titleLabel}</h1><div class='mb-5'>
                 """);
      if (response.ErrorStack.LengthStack > 0) {
         sb.Append(
            "<h5 class='text-danger mb-4'><i class='bi bi-exclamation-triangle-fill me-2'></i>Возможные ошибки:</h5><div class='list-group list-group-flush mx-auto' style='max-width: 500px;'>");
         while (response.ErrorStack.LengthStack > 0)
            sb.Append(
               "<div class='list-group-item bg-dark border-start border-danger border-4 p-3'><div class='d-flex align-items-center'><i class='bi bi-link-45deg text-info fs-5 me-3'></i><div>{response.ErrorStack.PopStack()}</div></div></div>");
         sb.Append("</div>");
      }

      sb.Append(
         "</div><a href='/' class='btn btn-danger btn-lg px-5 py-3 fw-semibold'><i class='bi bi-house-door me-2'></i>Вернуться на главную</a></div></div></div>");
      await End(request, response, cancellationToken);
   }

   public override string Title { get; }
}