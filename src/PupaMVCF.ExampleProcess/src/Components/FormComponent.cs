using PupaMVCF.Framework;
using PupaMVCF.Framework.Components;
using PupaMVCF.Framework.Core;

namespace PupaMVCF.ExampleProcess.Components;

public readonly record struct FormComponentFiled(string Type, string Name, string Label, bool Require = true);

public class FormComponent : Component {
   private readonly string _action;
   private readonly string _id;
   private readonly List<FormComponentFiled> _fields = [];

   public FormComponent(IComponentParent? parent, string action, string id) : base(parent) {
      _action = action;
      _id = id;
   }

   public void Push(string type, string name, string label, bool require = true) {
      _fields.Add(new FormComponentFiled(type, name, label, require));
   }

   public override Task Html(Request request, Response response, CancellationToken cancellationToken) {
      var sb = Builder;
      sb.Append($"<form  method='POST' action='{_action}' id='{_id}'>");
      foreach (var field in _fields) {
         if (field.Type == "captcha") {
            sb.Append($$"""
                        <div class="d-flex justify-content-center mb-4">
                           <div class="cf-turnstile" data-sitekey="{{WebApp.CaptchaSecureSite}}" data-theme="light" data-size="normal" data-callback="onSuccess"></div>
                            <script nonce={{response.Nonce}} src="https://challenges.cloudflare.com/turnstile/v0/api.js" async defer></script>
                        </div>
                        """);
            continue;
         }

         sb.Append($"""
                    <div class="form-floating mb-3">
                    <input type="{field.Type}" class="form-control bg-transparent text-light border-secondary" 
                      id="{field.Name}" 
                      name="{field.Name}" 
                      {(field.Require ? "required" : string.Empty)}>
                    <label for="email" class="text-secondary">{field.Label}</label></div>
                    """);
      }

      sb.Append(
         "<div class='d-grid mt-auto'><button type='submit' class='btn btn-outline-success btn-lg rounded-4'>Продолжить</button></div>");
      sb.Append("</form>");
      return Task.CompletedTask;
   }
}