using Grpc.Core;

using Protos.Auth;

using PupaMVCF.Framework;

namespace PupaMVCF.ExampleAuthProcess.Services;

public class AuthGrpcService : AuthService.AuthServiceBase {
   public override Task<VerifyResponse> Verify(VerifyRequest request, ServerCallContext context) {
      BaseApp.BaseInstance.Logger.LogInformation("RPC -> Code: {Code}", (object?)request.Code);
      return Task.FromResult(new VerifyResponse {
         Success = true
      });
   }
}