using System.Net;
using System.Text;
using System.Text.Json;

namespace PupaMVCF.Framework.Core;

public class Request {
   private readonly HttpListenerRequest _request;

   public string RawUrl { get; }
   public Uri? Url => _request.Url;
   public HttpMethodType HttpMethodType { get; }
   public MimeContentType MimeContentType { get; }
   public Session? Session { get; }

   public string IpV4 => _request.RemoteEndPoint.Address.ToString();
   public string UserAgent => _request.UserAgent;

   public Request(HttpListenerRequest request) {
      _request = request;
      MimeContentType = MimeContent.GetMimeType(request.ContentType);
      HttpMethodType = HttpMethodManager.HardGetHttpMethod(request.HttpMethod);
      RawUrl = _request.RawUrl ?? string.Empty;
      if (RawUrl == string.Empty)
         return;
      var index = RawUrl.IndexOf("?", StringComparison.CurrentCultureIgnoreCase);
      if (index != -1)
         RawUrl = RawUrl[..index];
      Session = Session.CreateSession(GetCookie(Session.SESSION_NAME));
   }

   public Cookie? GetCookie(string key) {
      return _request.Cookies[key];
   }

   public string GetQueryValue(string key) {
      return _request.QueryString[key] ?? string.Empty;
   }

   public override string ToString() {
      var sb = new StringBuilder();
      sb.Append(
         $"RawUrl: {RawUrl}\nMethod: {HttpMethodType}\nQuery: {string.Join(',', _request.QueryString.AllKeys)}\nAgent: {_request.UserAgent}\nMime: {MimeContentType}\n");
      return sb.ToString();
   }

   #region READ FUNCTIONS

   public async Task<byte[]> ReadContent(CancellationToken cancellationToken) {
      var bytes = new byte[4096];
      var nextBytes = 1;
      var length = 0;
      while (nextBytes > 0) {
         nextBytes = await _request.InputStream.ReadAsync(bytes, cancellationToken);
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
      foreach (var key in _request.QueryString.AllKeys) {
         var prop = props.FirstOrDefault(x => x.Name == key);
         if (prop == null) continue;
         prop.SetValue(obj, _request.QueryString[key] ?? string.Empty);
      }
   }
}