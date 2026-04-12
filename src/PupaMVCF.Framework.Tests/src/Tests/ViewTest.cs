using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Internal;

using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Tests.Components;
using PupaMVCF.Framework.Tests.Views;

using Xunit.Abstractions;

namespace PupaMVCF.Framework.Tests.Tests;

[Collection("ComponentsTest")]
public sealed class ViewsTest(ITestOutputHelper testOutputHelper) {
   private (Request, Response, CancellationToken) ImitationServerWork() {
      var sessionFeature = new DefaultSessionFeature();
      var httpRequest = new DefaultHttpRequest(new DefaultHttpContext()) {
         Method = "POST"
      };
      var request = new Request(httpRequest, sessionFeature.Session);
      var response = new Response(new DefaultHttpResponse(new DefaultHttpContext()));
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
      var (request, response, cancellationToken) = ImitationServerWork();
      var view = new TestView();
      view.Html(request, response, cancellationToken);
      Assert.True(view.Builder.Length > 8);
   }

   [Fact]
   public async Task ComponentCreate_WithHtmlInvoke_ReturnsOk() {
      var (request, response, cancellationToken) = ImitationServerWork();
      var view = new TestView();
      var component = new TestComponent(view);
      component.Html(request, response, cancellationToken);
      Assert.True(component.Builder.Length > 0);
      Assert.Equal("<h1>Test</h1>", component.Builder.ToString());
   }
}