using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using PupaMVCF.Tests.Models;
using Xunit.Abstractions;

namespace PupaMVCF.Tests.Tests;

[Collection("NeedServerCollectionTest")]
public sealed class GetRequestsTest(ITestOutputHelper testOutputHelper, TestHostFixture fixture) {
   private async Task<HttpResponseMessage> SendClient() {
      var ip = fixture.Configuration.GetValue<string>("Ip");
      var port = fixture.Configuration.GetValue<int>("Port");
      return await fixture.Client.GetAsync($"http://{ip}:{port}/test/get");
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
