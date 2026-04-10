using System.IO.Pipelines;
using System.Net;
using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;

using PupaMVCF.Framework.Routing;

namespace PupaMVCF.Framework.Core;

public class Request {
   private readonly HttpRequest _request;
   public string RawUrl { get; }
   public string RawUrlBase { get; }
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
      var index = RawUrl.IndexOf("?", StringComparison.CurrentCultureIgnoreCase);
      if (index != -1)
         RawUrl = RawUrl[..index];
      Session = session;
   }

   public Option<string> GetCookie(string key) {
      return _request.Cookies.TryGetValue(key, out var value) ? Option<string>.Ok(value) : Option<string>.Fail();
   }

   public Option<string> GetFormField(string key) {
      var result = _request.Form[key].FirstOrDefault();
      return result == null ? Option<string>.Fail() : Option<string>.Ok(result);
   }

   public Option<T> GetFormField<T>(string key) {
      var result = _request.Form[key].FirstOrDefault();
      return string.IsNullOrWhiteSpace(result)
         ? Option<T>.Fail()
         : Option<T>.Ok((T)Convert.ChangeType(result, typeof(T)));
   }

   public Option<string> GetQueryValue(string key) {
      var result = _request.Query[key].FirstOrDefault();
      return result == null ? Option<string>.Fail() : Option<string>.Ok(result);
   }

   public RouteKey ToRouteKey() {
      return new RouteKey(RawUrl, HttpMethodType);
   }

   public override string ToString() {
      return $"[{HttpMethodType}] {Url}";
   }

   #region READ FUNCTIONS

   public async Task<Option<byte[]>> ReadContent(CancellationToken cancellationToken) {
      try {
         if (_request.ContentLength is > 0) {
            var buffer = new byte[_request.ContentLength.Value];
            await _request.Body.ReadExactlyAsync(buffer, cancellationToken);
            return Option<byte[]>.Ok(buffer);
         }

         var ms = new MemoryStream();
         await _request.Body.CopyToAsync(ms, cancellationToken);
         return Option<byte[]>.Ok(ms.ToArray());
      } catch {
         return Option<byte[]>.FailLog(WebApp.Context.Logger, "Read bytes from stream exception");
      }
   }

   public async Task<Option<string>> ReadContentStr(CancellationToken cancellationToken) {
      try {
         if (!(await ReadContent(cancellationToken)).Out(out var bytes)) return Option<string>.Fail();
         var result = Encoding.UTF8.GetString(bytes);
         return Option<string>.Ok(result);
      } catch {
         return Option<string>.FailLog(WebApp.Context.Logger, $"Encoding to string exception");
      }
   }

   public async Task<Option<T>> ReadContentT<T>(CancellationToken cancellationToken) where T : class {
      try {
         var result = await JsonSerializer.DeserializeAsync<T>(_request.Body,
            WebApp.JsonSerializerOptions,
            cancellationToken);
         return Option<T>.Ok(result!);
      } catch {
         return Option<T>.Fail();
      }
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