using System.Net;
using System.Security.Cryptography;

namespace PupaMVCF.Framework.Core;

public sealed class Session {
   public const string SESSION_NAME = "__session";
   private const byte SESSION_LENGTH = 64;
   public string Guid { get; init; }

   private Session(string guid) {
      Guid = guid;
   }

   public static Session? CreateSession(Cookie? cookie) {
      var guid = cookie?.Value;
      return guid?.Length == SESSION_LENGTH ? new Session(guid) : null;
   }

   public static string GenerateSessionGUID() {
      var rng = RandomNumberGenerator.Create();
      Span<byte> randomBytes = stackalloc byte[32];
      Span<byte> hashBytes = stackalloc byte[32];
      rng.GetBytes(randomBytes);
      SHA256.TryHashData(randomBytes, hashBytes, out _);
      return Convert.ToHexString(hashBytes);
   }

   public static void GenerateSessionGUIDCookie(Response response) {
      response.SetCookie(SESSION_NAME, GenerateSessionGUID(), TimeSpan.FromDays(1), true);
   }
}