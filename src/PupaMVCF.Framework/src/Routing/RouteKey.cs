using PupaMVCF.Framework.Core;

namespace PupaMVCF.Framework.Routing;

public readonly record struct RouteKey(string Pattern, HttpMethodType Method);