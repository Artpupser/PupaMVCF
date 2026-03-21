using System.Net;
using System.Text;
using System.Text.Encodings.Web;

using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Views;

namespace PupaMVCF.Framework.Components;

public abstract class Component : IComponentParent {
   protected readonly IComponentParent? _parent;
   protected readonly View _currentView;

   protected Component(IComponentParent? parent) {
      _parent = parent;
      _currentView = (_parent == null ? this as View : _parent.CurrentView) ??
                     throw new ArgumentNullException(nameof(parent));
   }

   public abstract Task Html(Request request, Response response, CancellationToken cancellationToken);
   public virtual StringBuilder Builder => _parent!.Builder;
   public View CurrentView => _currentView;

   public static string XssHtml(string? content) {
      return string.IsNullOrWhiteSpace(content) ? string.Empty : WebUtility.HtmlEncode(content);
   }

   public static string XssUrl(string? content) {
      return string.IsNullOrWhiteSpace(content) ? string.Empty : WebUtility.UrlEncode(content);
   }

   public static string XssJs(string? content) {
      return string.IsNullOrWhiteSpace(content) ? string.Empty : JavaScriptEncoder.Default.Encode(content);
   }

   public override string ToString() {
      return Builder.ToString();
   }

   protected static string TagTitle(string? value) {
      return $"<title>{value}</title>";
   }

   protected static string TagLink(Response response, string? rel, string? href) {
      return $"<link nonce={response.Nonce} rel='{rel}' href='{href}'/>";
   }

   protected static string TagH1(string? value) {
      return $"<h1>{value}</h1>";
   }

   protected static string TagJs(Response response, string? value) {
      return $"<script nonce={response.Nonce} src='/api/public/files?name={value}'></script>";
   }

   protected static string TagJsModule(Response response, string? value) {
      return $"<script nonce={response.Nonce} src='/api/public/files?name={value}' type='module'></script>";
   }

   protected static string TagP(string? value) {
      return $"<p>{value}</p>";
   }

   protected static string TagImg(string? src, string? alt) {
      return $"<img alt='{alt}' src='{src}'/>";
   }
}