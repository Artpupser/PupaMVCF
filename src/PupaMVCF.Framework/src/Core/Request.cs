using System.Net;
using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace PupaMVCF.Framework.Core;

public class Request {
   private readonly HttpRequest _request;
   public PathString RawUrl { get; }
   public PathString RawUrlBase { get; }
   public string? Url => _request.GetEncodedUrl();
   public HttpMethodType HttpMethodType { get; }
   public MimeContentType MimeContentType { get; }
   public ISession? Session { get; }
   public string? SessionGUID => Session?.Id;
   public IPAddress IpAddress => _request.HttpContext.Connection.RemoteIpAddress;
   public string UserAgent => $"{_request.Headers["User-Agent"]}";
   
   public Request(HttpRequest request, ISession session) {
      _request = request;
      MimeContentType = MimeContent.GetMimeType(request.ContentType);
      HttpMethodType = HttpMethodManager.HardGetHttpMethod(request.Method);
      RawUrl = _request.Path.Value ?? string.Empty;
      RawUrlBase = _request.PathBase.Value ?? string.Empty;
      if (RawUrl == string.Empty)
         return;
      var index = RawUrl.Value.IndexOf("?", StringComparison.CurrentCultureIgnoreCase);
      if (index != -1)
         RawUrl = RawUrl.Value[..index];
      Session = session;
   }

   public string? GetCookie(string key) {
      return _request.Cookies[key];
   }

   public string GetQueryValue(string key) {
      return _request.Query[key].FirstOrDefault() ?? string.Empty;
   }

   public override string ToString() {
      var sb = new StringBuilder();
      sb.Append(
         $"RawUrl: {RawUrl}\nMethod: {HttpMethodType}\nQuery: {string.Join(',', _request.Query)}\nAgent: {UserAgent}\nMime: {MimeContentType}\n");
      return sb.ToString();
   }

   #region READ FUNCTIONS

   public async Task<byte[]> ReadContent(CancellationToken cancellationToken) {
      var bytes = new byte[4096];
      var nextBytes = 1;
      var length = 0;
      while (nextBytes > 0) {
         nextBytes = await _request.Body.ReadAsync(bytes, cancellationToken);
         length += nextBytes;
      }

      return bytes[..length];
   }

   public async Task<string> ReadContentStr(CancellationToken cancellationToken) {
      return Encoding.UTF8.GetString(await ReadContent(cancellationToken));
   }

   public async Task<T> ReadContentT<T>(CancellationToken cancellationToken) where T : class {
      return JsonSerializer.Deserialize<T>(await ReadContentStr(cancellationToken), BaseApp.JsonSerializerOptions)!;
   }

   public async Task<T?> ReadSafeContentT<T>(CancellationToken cancellationToken) where T : class {
      return JsonSerializer.Deserialize<T>(await ReadContentStr(cancellationToken), BaseApp.JsonSerializerOptions);
   }

   #endregion

   [Obsolete("Danger work")]
   public void LoadQueryToT<T>(T obj) {
      var props = obj!.GetType().GetProperties();
      foreach (var key in _request.Query) {
         var prop = props.FirstOrDefault(x => x.Name == key.Value);
         if (prop == null) continue;
         prop.SetValue(obj, _request.Query[key.Value].FirstOrDefault() ?? string.Empty);
      }
   }
}