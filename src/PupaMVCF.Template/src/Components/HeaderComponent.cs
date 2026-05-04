using PupaMVCF.Framework.Components;
using PupaMVCF.Framework.Core;

namespace PupaMVCF.Web.Template.Components;

public sealed class HeaderComponent : Component {
   public HeaderComponent(IComponentParent? parent) : base(parent) { }

   public override Task Html(Request request, Response response, CancellationToken cancellationToken) {
      var sb = Builder;
      sb.Append("""
                <nav class='navbar navbar-expand-lg navbar-dark bg-dark sticky-top'>
                  <div class='container'>
                      <a class='navbar-brand fw-bold fs-3 d-flex align-items-center' href='#'>
                          <img src='/api/public/files?name=icon.webp' alt='icon' width='40' height='40' class='me-3 rounded'>
                """);
      sb.Append($"<span>{CurrentView.Title}</span></a>");
      sb.Append("""
                <button class='navbar-toggler' type='button' data-bs-toggle='collapse' data-bs-target='#navbarNav'>
                    <span class='navbar-toggler-icon'></span>
                </button>
                <div class='collapse navbar-collapse' id='navbarNav'>
                    <ul class='navbar-nav ms-auto'>
                """);
      sb.Append($"""
                 <li class='nav-item'>
                     <a class='nav-link fs-5 fw-regular' href='/'>Дом</a>
                 </li>
                 """);
      sb.Append(
         "</ul></div></div><div aria-live='polite' aria-atomic='true' class='position-fixed top-0 end-0 p-3 z-3' id='liveAlertPlaceholder'></div></nav>");
      return Task.CompletedTask;
   }
}