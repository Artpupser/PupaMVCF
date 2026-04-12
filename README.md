# 🚀 PupaMVCF

![.NET](https://img.shields.io/badge/.NET-10.0-blue)
![C#](https://img.shields.io/badge/C%23-14-purple)
![Platform](https://img.shields.io/badge/platform-cross--platform-lightgrey)
![License](https://img.shields.io/badge/license-MIT-green)
![Status](https://img.shields.io/badge/status-active-success)
![NuGet](https://img.shields.io/nuget/v/PupaMVCF.Framework)

---

## ✨ Description

**PupaMVCF** is a lightweight and flexible web framework built on top of .NET 10 and C# 14.  
It is designed to simplify the development of modern web applications using the Model-View-Controller (MVC) pattern.

### The framework focuses on:

- ⚡ simplicity
- 🧩 extensibility
- 🚀 performance

---

### v0.1.6
- Changed .NET version .net8.0 -> .net10.0

### v0.1.5

- Extended PupaMVCF.Framework.Tests with GET, POST requests and views testing
- Fixed validator: character substitution in `for` cycle (i -> y)

### v0.1.4

- Pipe-based request body reading and response writing via `System.IO.Pipelines`
- Form data reading support with typed `GetFormField<T>`
- Extended `Redirect` overloads with URI validation
- Full validation system: modules for email, string/number range, required fields, Cloudflare Captcha
- `ValidFromRequest<T>` — validates model directly from request, returns `Option<T>`
- `Option<T>` improvements: `implicit operator bool`, singleton `Fail`, `Out(out T)` pattern
- Extended session config: expiry, SameSite, custom cookie name
- Added `HttpsEnable`, `Domain`, `StaticPrefix` config options
- Middleware can now be declared directly on `[ControllerHandler]` attribute
- CI: web template now published with a separate NuGet token

## 🔥 Features

- 🌐 Cross-platform support
- 🧱 MVC architecture
- 📧 gRPC support
- 🧩 Middleware *(in progress)*
- 🎨 Views system
- 💉 Dependency Injection
- ⚙️ Configuration system
- 📊 Logging
- 📦 Models & Controllers
- 🧭 Routing
- ✅ Validations *(in progress)*
- 🚨 Error handling
- 🔐 Security:
    - ⚡ XSS protection
    - ⚡ SQL Injection protection
- 🔄 Daemon processing
- 🧩 Microservices ready
- 🚀 Kestrel integration
    - ⚡ Cookies *(Kestrel-based)*
    - ⚡ Sessions *(Kestrel-based)*
    - ⚡ Pipe writer/reader

---

## 📦 Installation and run

```bash
dotnet install new PupaMVCF.Web.Template
mkdir SolutionName
cd ./SolutionName
dotnet new sln ./
mkdir ProjectName
cd ./ProjectName
dotnet new pupamvcf-web
cd ..
dotnet sln add ./ProjectName
cd ./ProjectName
dotnet run