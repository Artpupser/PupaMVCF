using Microsoft.AspNetCore.Http;

using PupaMVCF.Framework.Core;
using PupaMVCF.Tests.Components;
using PupaMVCF.Tests.Views;

using Xunit.Abstractions;

namespace PupaMVCF.Tests.Tests;

[Collection("ComponentsTest")]
public sealed class ViewsTest {
   private (Request, Response, CancellationToken) ImitationServerWork() {
      var httpContext = new DefaultHttpContext {
         Request = {
            Method = "GET"
         }
      };
      var request = new Request(httpContext.Request);
      var response = new Response(httpContext.Response);
      var cts = new CancellationTokenSource();
      return (request, response, cts.Token);
   }

   [Fact]
   public async Task ViewCreate_PropertiesCheck_ReturnsOk() {
      var view = new TestView();
      Assert.Equal("Test title", view.Title);
      Assert.Equal(view, view.CurrentView);
   }

   [Fact]
   public async Task ViewCreate_WithHtmlInvoke_ReturnsOk() {
      await Task.Delay(TimeSpan.FromSeconds(1));
      var (request, response, cancellationToken) = ImitationServerWork();
      var view = new TestView();
      await view.Html(request, response, cancellationToken);
      Assert.True(view.Builder.Length > 8);
   }

   [Fact]
   public async Task ComponentCreate_WithHtmlInvoke_ReturnsOk() {
      var (request, response, cancellationToken) = ImitationServerWork();
      var view = new TestView();
      var component = new TestComponent(view);
      await component.Html(request, response, cancellationToken);
      Assert.True(component.Builder.Length > 0);
      Assert.Equal("<h1>Test</h1>", component.Builder.ToString());
   }
}