using PupaMVCF.Framework.Components;
using PupaMVCF.Framework.Core;

namespace PupaMVCF.Web.Template.Components;

public sealed class FooterComponent : Component {
   public FooterComponent(IComponentParent? parent) : base(parent) { }

   public override Task Html(Request request, Response response, CancellationToken cancellationToken) {
      var sb = Builder;
      sb.Append($$"""
                  <footer class='bg-dark text-light py-5 mt-auto'>
                    <div class='container'>
                        <div class='row align-items-center'>
                            <div class='col-md-3 text-center text-md-start mb-3 mb-md-0'>
                                <img src='/api/public/files?name=icon.webp' alt='Логотип' class='img-fluid' style='max-height: 64px;'>
                            </div>
                            <div class='col-md-9 text-center text-md-end'>
                                <h5 class='fw-bold mb-2'>{{CurrentView.Title}}</h5>
                                <p class='mb-0 fs-6 text-light opacity-75'>
                                    Начало начал.<br>
                                    <small class='text-light'>©{{DateTime.Now.Year}} Все права защищены</small>
                                    </p>
                                </div>
                            </div>
                        </div>
                  </footer>
                  """);
      return Task.CompletedTask;
   }
}