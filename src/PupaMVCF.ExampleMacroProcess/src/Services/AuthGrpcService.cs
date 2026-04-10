using Grpc.Core;

using Protos.Auth;

using PupaMVCF.Framework;
using PupaMVCF.Framework.Macro;

namespace PupaMVCF.ExampleMacroProcess.Services;

public class AuthGrpcService : AuthService.AuthServiceBase {
   public override Task<VerifyResponse> Verify(VerifyRequest request, ServerCallContext context) {
      MacroApp.ContextInstance.Logger.LogInformation("RPC -> Code: {Code}", request.Code);
      return Task.FromResult(new VerifyResponse {
         Success = true
      });
   }
}