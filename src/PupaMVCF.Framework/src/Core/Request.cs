using System.Net;
using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;

using PupaLib.Core;

using PupaMVCF.Framework.Routing;

namespace PupaMVCF.Framework.Core;

public class Request {
   private readonly HttpRequest _request;
   public string Path => _request.Path;
   public string PathBase => _request.PathBase;
   public string QueryString => _request.QueryString.Value;
   public string Url => _request.GetEncodedUrl();
   public HttpMethodType HttpMethodType { get; }
   public MimeContentType MimeContentType { get; }
   public IFeatureCollection FeatureCollection { get; }
   public ISession Session { get; }
   public IPAddress IpAddress => _request.HttpContext.Connection.RemoteIpAddress;
   public IRequestHeader RequestHeader { get; }

   public Request(HttpRequest request, ISession session) {
      _request = request;
      MimeContentType = MimeContent.GetMimeType(request.ContentType);
      HttpMethodType = HttpMethodManager.HardGetHttpMethod(request.Method);
      FeatureCollection = request.HttpContext.Features;
      RequestHeader = new HeaderDictionaryProvider(_request.Headers);
      Session = session;
   }

   public Option<string> GetCookie(string key) {
      return _request.Cookies.TryGetValue(key, out var value) ? Option<string>.Ok(value) : Option<string>.Fail();
   }

   public Option<string> GetFormField(string key) {
      if (_request.Form.TryGetValue(key, out var value) &&
          !StringValues.IsNullOrEmpty(value))
         return Option<string>.Ok(value.ToString());

      return Option<string>.Fail();
   }

   public Option<IEnumerable<IFormFile>> GetFormFiles(string key) {
      var result = _request.Form.Files.GetFiles(key);
      return result == null ? Option<IEnumerable<IFormFile>>.Fail() : Option<IEnumerable<IFormFile>>.Ok(result);
   }

   public Option<IFormFile> GetFormFile(string key) {
      var result = _request.Form.Files.GetFile(key);
      return result == null ? Option<IFormFile>.Fail() : Option<IFormFile>.Ok(result);
   }

   public Option<string> GetQuery(string key) {
      if (_request.Query.TryGetValue(key, out var value) &&
          !StringValues.IsNullOrEmpty(value))
         return Option<string>.Ok(value.ToString());

      return Option<string>.Fail();
   }

   public RouteKey ToRouteKey() {
      return new RouteKey(Path, HttpMethodType);
   }

   public override string ToString() {
      return $"[{HttpMethodType}] {Url}";
   }

   #region READ FUNCTIONS

   public async Task<Option<byte[]>> ReadContent(CancellationToken cancellationToken) {
      try {
         _request.EnableBuffering();
         _request.Body.Position = 0;
         using var ms = new MemoryStream();
         await _request.Body.CopyToAsync(ms, cancellationToken);
         var data = ms.ToArray();
         _request.Body.Position = 0;
         return data.Length == 0
            ? Option<byte[]>.Fail()
            : Option<byte[]>.Ok(data);
      } catch {
         return Option<byte[]>.Fail();
      }
   }

   public async Task<Option<string>> ReadContentStr(CancellationToken cancellationToken) {
      try {
         if (!(await ReadContent(cancellationToken)).Out(out var bytes)) return Option<string>.Fail();
         var result = Encoding.UTF8.GetString(bytes);
         return Option<string>.Ok(result);
      } catch {
         return Option<string>.Fail();
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
}