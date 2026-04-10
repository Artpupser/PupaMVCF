using System.Buffers;
using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Http;

using PupaLib.FileIO;

using PupaMVCF.Framework.Components;

namespace PupaMVCF.Framework.Core;

public sealed class Response : IErrorController {
   private Memory<byte> _cache;
   private readonly HttpResponse _response;
   private readonly Stack<string> _errorsStack;
   private MimeContentType _mimeContentType;
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
      _errorsStack = [];
      _response = response;
      _response.StatusCode = 200;
      _cache = Memory<byte>.Empty;
      _response.Headers["X-Content-Type-Options"] = "nosniff";
      _response.Headers["X-Frame-Options"] = "DENY";
      _response.Headers["X-XSS-Protection"] = "1; mode=block";
      _response.Headers["Content-Security-Policy"] =
         $"script-src 'self' 'nonce-{Nonce}' https://challenges.cloudflare.com https://*.cloudflare.com https://cdn-cgi.challenge-platform.com  https://cdn.jsdelivr.net; connect-src 'self' https:; frame-src https://challenges.cloudflare.com https://*.cloudflare.com; object-src 'none';";
   }

   #region IErrorController

   public IEnumerable<string> Errors => _errorsStack;

   public void PushError(string message) {
      _response.StatusCode = 400;
      _errorsStack.Push(message);
   }

   public void PushError(int status) {
      _response.StatusCode = status;
      _errorsStack.Push("Error description, in status");
   }

   public void PushError(string message, int status) {
      _response.StatusCode = status;
      _errorsStack.Push(message);
   }

   #endregion

   public void SetHeader(string key, string value) {
      _response.Headers[key] = value;
   }

   public void Redirect(string url) {
      _response.Redirect(url, false);
   }

   public void Redirect(Uri uri) {
      _response.Redirect(uri.AbsoluteUri, false);
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

   public void WriteBytesToCache(Memory<byte> content) {
      _cache = content;
   }

   public void WriteTJsonToCache<T>(T content) where T : class {
      MimeContentType = MimeContentType.Json;
      var buffer = new ArrayBufferWriter<byte>();
      using var writer = new Utf8JsonWriter(buffer);
      JsonSerializer.Serialize(writer, content, WebApp.JsonSerializerOptions);
      _cache = buffer.WrittenMemory.ToArray();
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