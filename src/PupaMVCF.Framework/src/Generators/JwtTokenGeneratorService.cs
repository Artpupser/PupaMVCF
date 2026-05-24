using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace PupaMVCF.Framework.Generators;

public sealed class JwtTokenGeneratorService(IConfiguration configuration) {
   public byte[] JwtSecretBytes { get; } = Encoding.UTF8.GetBytes(configuration.GetValue<string>("JWT_SECRET") ??
                                                                  throw new InvalidOperationException(
                                                                     "Undefined JWT_SECRET."));

   public ValueTask<string> GenerateJwt(DateTimeOffset expire, Claim[]? claims) {
      return ValueTask.FromResult(new JwtSecurityTokenHandler()
         .WriteToken(new JwtSecurityToken(
            claims: claims ?? [],
            expires: expire.UtcDateTime,
            signingCredentials: new SigningCredentials(
               new SymmetricSecurityKey(JwtSecretBytes),
               SecurityAlgorithms.HmacSha256)
         )));
   }
}