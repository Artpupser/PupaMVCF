using Protos.Auth;

using PupaMVCF.ExampleProcess.Models;

namespace PupaMVCF.ExampleProcess.Extensions;

public static class ModelExtensions {
   public static VerifyRequest ToGrpc(this VerifyCodeModel value) {
      return new VerifyRequest {
         Code = value.Code
      };
   }

   public static VerifyCodeModel ToRecord(this VerifyRequest value) {
      return new VerifyCodeModel {
         Code = value.Code
      };
   }
}