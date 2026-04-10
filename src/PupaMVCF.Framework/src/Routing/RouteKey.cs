using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Routing;

public record RouteKey(string Pattern, HttpMethodType Method);