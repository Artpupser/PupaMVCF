using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;

using PupaLib.Core;

using PupaMVCF.Framework.Routing;

namespace PupaMVCF.Framework.Core;

public class Request(HttpRequest request) {
   private readonly HttpRequest _request = request;
   public string Path => _request.Path;
   public string PathBase => _request.PathBase;

   public Option<string> QueryString => _request.QueryString.HasValue
      ? Option<string>.Ok(_request.QueryString.Value)
      : Option<string>.Fail();

   public string Url => _request.GetEncodedUrl();
   public HttpMethodType HttpMethodType { get; } = HttpMethodManager.HardGetHttpMethod(request.Method);
   public MimeContentType MimeContentType { get; } = MimeContent.GetMimeType(request.ContentType);
   public ClaimsPrincipal User { get; } = request.HttpContext.User;
   public IFeatureCollection FeatureCollection { get; } = request.HttpContext.Features;

   public Option<IPAddress> IpAddress => _request.HttpContext.Connection.RemoteIpAddress is not null
      ? Option<IPAddress>.Ok(_request.HttpContext.Connection.RemoteIpAddress)
      : Option<IPAddress>.Fail();

   public IRequestHeader RequestHeader { get; } = new HeaderDictionaryProvider(request.Headers);

   public Option<string> GetBearerToken() {
      var authHeader = _request.Headers.Authorization.FirstOrDefault();
      if (string.IsNullOrWhiteSpace(authHeader) ||
          !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
         return Option<string>.Fail();

      var token = authHeader["Bearer ".Length..].Trim();
      return string.IsNullOrWhiteSpace(token)
         ? Option<string>.Fail()
         : Option<string>.Ok(token);
   }

   public bool IsAuth() {
      return User.Identity?.IsAuthenticated ?? false;
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
      return !result.Any() ? Option<IEnumerable<IFormFile>>.Fail() : Option<IEnumerable<IFormFile>>.Ok(result);
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
