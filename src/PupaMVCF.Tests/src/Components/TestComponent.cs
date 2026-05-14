using PupaMVCF.Framework.Components;
using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Tests.Components;

public sealed class TestComponent(IComponentParent? parent) : Component(parent) {
   public override Task Html(Request request, Response response, CancellationToken cancellationToken) {
      var sb = Builder;
      sb.Append("<h1>Test</h1>");
      return Task.CompletedTask;
   }
}