using Grpc.Core;

using Protos.Auth;

using PupaMVCF.ExampleProcess.Models;
using PupaMVCF.ExampleProcess.Extensions;
using PupaMVCF.Framework;
using PupaMVCF.Framework.Controllers;
using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Validations;

namespace PupaMVCF.ExampleProcess.Controllers;

public sealed class AuthController : Controller {
   private readonly Channel _channel;
   private readonly AuthService.AuthServiceClient _client;

   public AuthController(IConfiguration configuration) {
      _channel = new Channel(configuration["AuthServiceIpPort"], ChannelCredentials.Insecure);
      _client = new AuthService.AuthServiceClient(_channel);
   }


   [ControllerHandler("/verify", HttpMethodType.POST)]
   private async Task VerifyHandler(Request request, Response response, CancellationToken cancellationToken) {
      response.MimeContentType = MimeContentType.Json;
      var verifyCodeModel = await request.ReadSafeContentT<VerifyCodeModel>(cancellationToken);
      if (await ValidatorManager.Valid(request, response, verifyCodeModel, cancellationToken)) {
         var grpcResponse = await _client.VerifyAsync(verifyCodeModel.ToGrpc());
         if (grpcResponse.Success) {
            response.WriteEmptyJsonToCache();
            return;
         }

         response.ErrorStack.PushStack("Verify false!");
      }

      WebApp.SecureInstance.Logger.LogWarning($"Not valid");
   }

   [ControllerHandler("/login", HttpMethodType.POST)]
   private async Task LoginHandler(Request request, Response response, CancellationToken cancellationToken) {
      response.MimeContentType = MimeContentType.Json;
      var loginModel = await request.ReadSafeContentT<LoginModel>(cancellationToken);
      if (await ValidatorManager.Valid(request, response, loginModel, cancellationToken))
         response.WriteEmptyJsonToCache();
   }

   [ControllerHandler("/registration", HttpMethodType.POST)]
   private async Task RegistrationHandler(Request request, Response response, CancellationToken cancellationToken) {
      response.MimeContentType = MimeContentType.Json;
      var registrationModel = await request.ReadSafeContentT<RegistrationModel>(cancellationToken);
      if (await ValidatorManager.Valid(request, response, registrationModel, cancellationToken))
         response.WriteEmptyJsonToCache();
   }
}