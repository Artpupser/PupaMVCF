using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Http;

using PupaLib.FileIO;

using PupaMVCF.Framework.Core.Extra;
using PupaMVCF.Framework.Views;

namespace PupaMVCF.Framework.Core;

public sealed class Response : IHaveStack<string> {
   private Memory<byte> _cache;
   private readonly HttpResponse _response;
   private readonly Stack<string> _errorStack;
   private MimeContentType _mimeContentType;
   private const string EmptyJsonContent = "{}";
   public string Nonce { get; }

   public int StatusCode {
      get => _response.StatusCode;
      set => _response.StatusCode = value;
   }

   public MimeContentType MimeContentType {
      get => _mimeContentType;
      set {
         _mimeContentType = value;
         _response.ContentType = MimeContent.GetMimeTypeStr(_mimeContentType);
      }
   }

   public Response(HttpResponse response) {
      Nonce = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(8));
      _errorStack = [];
      _response = response;
      _response.StatusCode = 200;
      _cache = Memory<byte>.Empty;
      _response.Headers["X-Content-Type-Options"] = "nosniff";
      _response.Headers["X-Frame-Options"] = "DENY";
      _response.Headers["X-XSS-Protection"] = "1; mode=block";
      _response.Headers["Content-Security-Policy"] =
         $"script-src 'self' 'nonce-{Nonce}' https://challenges.cloudflare.com https://*.cloudflare.com https://cdn-cgi.challenge-platform.com  https://cdn.jsdelivr.net; connect-src 'self' https:; frame-src https://challenges.cloudflare.com https://*.cloudflare.com; object-src 'none';";
   }

   #region ErrorStack

   public IHaveStack<string> ErrorStack => this;
   public IReadOnlyList<string> StackContentList => _errorStack.ToList();
   int IHaveStack<string>.LengthStack => _errorStack.Count;

   void IHaveStack<string>.PushStack(string v) {
      _errorStack.Push(v);
   }

   string IHaveStack<string>.PopStack() {
      return _errorStack.Pop();
   }

   #endregion

   public void SetHeader(string key, string value) {
      _response.Headers[key] = value;
   }

   public void Reopen(Request request) {
      _response.Redirect(request.Url);
   }

   public void SetCookie(string key, string value, TimeSpan expiresAfter, bool secure = true) {
      _response.Cookies.Append(key, value, new CookieOptions {
         Expires = DateTime.Now + expiresAfter,
         Secure = secure
      });
   }

   public void SetCache(TimeSpan maxAge) {
      _response.Headers["Cache-Control"] = $@"max-age={maxAge.TotalSeconds}";
   }

   public async Task SendAsync(CancellationToken cancellationToken) {
      _response.ContentLength = _cache.Length;
      await _response.Body.WriteAsync(_cache, cancellationToken);
   }

   #region WRITE FUNCTIONS

   public void WriteBytesToCache(byte[] content) {
      _cache = content;
   }

   public void WriteEmptyJsonToCache() {
      MimeContentType = MimeContentType.Json;
      WriteStrToCache(EmptyJsonContent);
   }

   public void WriteTJsonToCache<T>(T content) where T : class {
      MimeContentType = MimeContentType.Json;
      WriteStrToCache(JsonSerializer.Serialize(content, WebApp.JsonSerializerOptions));
   }

   public async Task WriteVirtualFileToCache(VirtualFile file, CancellationToken cancellationToken) {
      MimeContentType = MimeContent.GetMimeType(file);
      WriteBytesToCache(await file.ReadBytesAsync(cancellationToken));
   }

   public void WriteStrToCache(string content) {
      WriteBytesToCache(Encoding.UTF8.GetBytes(content));
   }

   public void WriteViewToCache(View view) {
      MimeContentType = MimeContentType.Html;
      WriteBytesToCache(Encoding.UTF8.GetBytes(view.ToString()));
   }

   #endregion
}