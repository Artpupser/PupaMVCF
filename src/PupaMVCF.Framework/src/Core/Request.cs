using System.IO.Pipelines;
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
      _request.Cookies.TryGetValue(key, out var value);
      return value;
   }

   public string? GetFormField(string key) {
      return _request.Form[key].FirstOrDefault();
   }

   public T? GetFormField<T>(string key) {
      var value = _request.Form[key].FirstOrDefault() ?? string.Empty;
      if (string.IsNullOrWhiteSpace(value)) return default;
      return (T)Convert.ChangeType(value, typeof(T));
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
      var reader = PipeReader.Create(_request.Body);
      using var ms = new MemoryStream();

      while (true) {
         var result = await reader.ReadAsync(cancellationToken);
         var buffer = result.Buffer;
         foreach (var segment in buffer)
            ms.Write(segment.Span);
         reader.AdvanceTo(buffer.End);
         if (result.IsCompleted)
            break;
      }

      return ms.ToArray();
   }

   public async Task<string> ReadContentStr(CancellationToken cancellationToken) {
      return Encoding.UTF8.GetString(await ReadContent(cancellationToken));
   }

   public async Task<T> ReadContentT<T>(CancellationToken cancellationToken) where T : class {
      return JsonSerializer.Deserialize<T>(await ReadContentStr(cancellationToken), WebApp.JsonSerializerOptions)!;
   }

   public async Task<T?> ReadSafeContentT<T>(CancellationToken cancellationToken) where T : class {
      return JsonSerializer.Deserialize<T>(await ReadContentStr(cancellationToken), WebApp.JsonSerializerOptions);
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