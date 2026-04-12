using System.Net.Http.Json;

using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Extensions;
using PupaMVCF.Framework.Tests.Models;

using Xunit.Abstractions;

namespace PupaMVCF.Framework.Tests.Tests;

[Collection("NeedServerCollectionTest")]
public sealed class GetRequestsTest(ITestOutputHelper testOutputHelper, TestHostFixture fixture) {
   private async Task<HttpResponseMessage> SendClient() {
      var ip = fixture.Configuration.GetAny<string>("Ip");
      var port = fixture.Configuration.GetAny<int>("Port");
      return await WebApp.Context.Client.GetAsync($"http://{ip}:{port}/test_get");
   }

   [Fact]
   public async Task ControllerGetHandler_WithBasicRequest_ReturnsOk() {
      var response = await SendClient();
      var model = await response.Content.ReadFromJsonAsync<TestModel>();
      if (model != null) {
         testOutputHelper.WriteLine($"Id: {model.Id}");
         foreach (var item in model.Items) {
            testOutputHelper.WriteLine($"Name: {item.Name}");
            testOutputHelper.WriteLine($"Age: {item.Age}");
            testOutputHelper.WriteLine($"Email: {item.Email}");
         }
      }

      Assert.True(model != null);
   }
}