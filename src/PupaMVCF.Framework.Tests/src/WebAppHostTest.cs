using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using PupaMVCF.Framework.Controllers;
using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Database;
using PupaMVCF.Framework.Middleware;
using PupaMVCF.Framework.Routing;

using Xunit.Abstractions;

namespace PupaMVCF.Framework.Tests;

public sealed class TestWebApp : WebApp {
   public TestWebApp(IConfiguration configuration, IRouter router, ILogger<WebApp> logger) : base(configuration, router,
      logger) { }

   public override IDatabaseProcessor DatabaseProcessor => throw new NotImplementedException();
}

[Collection("Test")]
public sealed class WebAppHostTest : IAsyncLifetime {
   private readonly ITestOutputHelper _testOutputHelper;
   private IHost _host = null!;

   public WebAppHostTest(ITestOutputHelper testOutputHelper) {
      _testOutputHelper = testOutputHelper;
   }

   public Task InitializeAsync() {
      var builder = Host.CreateApplicationBuilder([]);
      builder.Configuration.AddJsonFile("secrets.json");
      builder.Services.AddSingleton<IRouter, Router>(_ => {
         var routerMapBuilder = new RouterMapBuilder();
         routerMapBuilder.AddMiddlewareRange([new LoggerMiddleware()]);
         routerMapBuilder.AddControllerRange([
            new StaticController()
         ]);
         return new Router(routerMapBuilder);
      });
      builder.Services.AddHostedService<TestWebApp>();
      _host = builder.Build();
      return Task.CompletedTask;
   }

   [Fact]
   public async Task HostedService_ShouldRunAndStop() {
      const int durationWorkInSeconds = 5;
      var cts = new CancellationTokenSource();
      cts.CancelAfter(TimeSpan.FromSeconds(durationWorkInSeconds));
      try {
         await _host.RunAsync(cts.Token);
      } catch (Exception e) {
         _testOutputHelper.WriteLine($"(CancelAfter) duration in seconds: {durationWorkInSeconds}");
         Assert.Fail(e.Message);
      }

      Assert.True(true, "Application started and worked successful!");
   }

   public Task DisposeAsync() {
      _host.Dispose();
      return Task.CompletedTask;
   }
}