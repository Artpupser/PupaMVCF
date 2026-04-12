using System.Net;
using System.Net.Http.Json;

using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Extensions;
using PupaMVCF.Framework.Tests.Models;

using Xunit.Abstractions;

namespace PupaMVCF.Framework.Tests.Tests;

[Collection("NeedServerCollectionTest")]
public sealed class PostRequestsTest(ITestOutputHelper testOutputHelper, TestHostFixture fixture) {
   private async Task<HttpResponseMessage> SendClient<T>(T model) {
      var ip = fixture.Configuration.GetAny<string>("Ip");
      var port = fixture.Configuration.GetAny<int>("Port");
      return await WebApp.Context.Client.PostAsJsonAsync($"http://{ip}:{port}/test_post", model);
   }

   [Fact]
   public async Task ControllerPostHandler_WithBigTestModel_ReturnsOk() {
      var model = new TestModel {
         Id = "1",
         Items = [
            new TestModelItem {
               Name = "test1",
               Email = "test1@test.com",
               Age = 20
            },
            new TestModelItem {
               Name = "test2",
               Email = "test2@test.com",
               Age = 20
            },
            new TestModelItem {
               Name = "test3",
               Email = "test3@test.com",
               Age = 20
            }
         ]
      };
      var response = await SendClient(model);
      Assert.Equal((HttpStatusCode)200, response.StatusCode);
   }

   [Fact]
   public async Task ControllerPostHandler_WithSmallTestModel_ReturnsOk() {
      var model = new TestModel {
         Id = "1",
         Items = [
            new TestModelItem {
               Name = "test1",
               Email = "test1@test.com",
               Age = 20
            }
         ]
      };
      var response = await SendClient(model);
      Assert.Equal((HttpStatusCode)200, response.StatusCode);
   }

   [Fact]
   public async Task ControllerPostHandler_WithBrokenName_ReturnsBad() {
      var model = new TestModel {
         Id = "1",
         Items = [
            new TestModelItem {
               Name = "",
               Email = "test1@test.com",
               Age = 20
            }
         ]
      };

      var response = await SendClient(model);
      var content = await response.Content.ReadFromJsonAsync<string[]>();
      foreach (var item in content!) testOutputHelper.WriteLine(item);

      Assert.NotEqual((HttpStatusCode)200, response.StatusCode);
      Assert.Equal(2, content.Length);
   }

   [Fact]
   public async Task ControllerPostHandler_WithBrokenEmail_ReturnsBad() {
      var model = new TestModel {
         Id = "1",
         Items = [
            new TestModelItem {
               Name = "Model",
               Email = "test1@_!-@testcm",
               Age = 20
            }
         ]
      };

      var response = await SendClient(model);
      var content = await response.Content.ReadFromJsonAsync<string[]>();
      foreach (var item in content!) testOutputHelper.WriteLine(item);

      Assert.NotEqual((HttpStatusCode)200, response.StatusCode);
      Assert.Equal(2, content.Length);
   }

   [Fact]
   public async Task ControllerPostHandler_WithBrokenId_ReturnsBad() {
      var model = new TestModel {
         Id = "1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111",
         Items = [
            new TestModelItem {
               Name = "Model",
               Email = "test1@_!-@testcm",
               Age = 20
            }
         ]
      };

      var response = await SendClient(model);
      var content = await response.Content.ReadFromJsonAsync<string[]>();
      foreach (var item in content!) testOutputHelper.WriteLine(item);

      Assert.NotEqual((HttpStatusCode)200, response.StatusCode);
      Assert.Single(content);
   }

   [Fact]
   public async Task ControllerPostHandler_WithZeroItems_ReturnsBad() {
      var model = new TestModel {
         Id = "123",
         Items = []
      };

      var response = await SendClient(model);
      var content = await response.Content.ReadFromJsonAsync<string[]>();
      foreach (var item in content) testOutputHelper.WriteLine(item);

      Assert.NotEqual((HttpStatusCode)200, response.StatusCode);
      Assert.Equal(2, content.Length);
   }
}