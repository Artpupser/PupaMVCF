<div align="center">

# 🚀 PupaMVCF

![Dotnet](https://img.shields.io/badge/.NET-black?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-black.svg?style=for-the-badge&logo=csharp&logoColor=white)
![License](https://img.shields.io/badge/MIT-black?style=for-the-badge)
![Nuget](https://custom-icon-badges.demolab.com/badge/Nuget-black?style=for-the-badge&logo=nuget&logoColor=white)

![Platform](https://img.shields.io/badge/platform-cross--platform-lightgrey?style=for-the-badge)
![C#](https://img.shields.io/badge/C%23-14-purple?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-10.0-blue?style=for-the-badge)
![NuGet](https://img.shields.io/nuget/v/PupaMVCF.Framework?style=for-the-badge)

#### [PupaMVCF](https://github.com/Artpupser/PupaMVCF) is a lightweight and flexible web framework using the MVC pattern. 🎯

<img src="https://github.com/Artpupser/PupaMVCF/blob/main/assets/banner.jpg" style="border-radius: 20px; max-height: 500px">

</div>

---

## 📎 Navigation

- [✨ Features](#-features)
- [🧵 Usage](#-usage)
- [👀 Preview](#-usage)
- [🦾️ Installation](#-installation)
    - [Linux 🐧](#linux-)
    - [Windows 🖥️](#windows-)
- [📦 Dependencies](#-dependencies)
- [🗃️ Devlog](#devlog)
- [⚖️️ License](#-license)

## ✨️ Features

<div align="center">

| 🏆 Feature                | 📝 Description                                                                                                            |
|---------------------------|---------------------------------------------------------------------------------------------------------------------------|
| 🌐 Cross-platform support | Supports Windows, Linux, and macOS using the modern .NET runtime                                                          |
| 🚥 Simplify               | Simplifies backend development with built-in routing, dependency injection, validation, authentication, and configuration |
| 💪 Flexibility            | Easily extensible with middleware, repositories, custom services, microservices support, and modular architecture         |
| 🧱 MVC architecture       | Structured around the MVC pattern with controllers, models, views, and routing                                            |
| 📧 gRPC support           | (not supported) Supports high-performance gRPC communication for services and APIs                                        |
| 🧩 Middleware             | Extensible middleware pipeline for request and response processing                                                        |
| 🎨 Views system           | Built-in view rendering system for dynamic page generation                                                                |
| 💉 Dependency Injection   | Integrated dependency injection container for service management                                                          |
| ⚙️ Configuration system   | Flexible configuration system with environment and application settings                                                   |
| 📊 Logging                | Logging system for application events, debugging, and monitoring                                                          |
| 📦 Models & Controllers   | Simplified architecture for organizing business logic and request handling                                                |
| 🧭 Routing                | Attribute and route-based request routing system                                                                          |
| ✅ Validations             | Built-in request and model validation utilities                                                                           |
| 🚨 Error handling         | Centralized exception and error handling mechanisms                                                                       |
| 🔐 Security               | Includes XSS and SQL Injection protection mechanisms                                                                      |
| 🔄 Daemon processing      | Supports long-running background and daemon tasks                                                                         |
| 🧩 Microservices ready    | Designed for scalable and distributed microservice architectures                                                          |
| 🚀 Kestrel integration    | Deep integration with Kestrel including cookies, authentication, and pipe-based I/O                                       |

</div>

## 🦾 Full installation and run

```bash
dotnet new install PupaMVCF.Template
dotnet new list

mkdir SolutionName
mkdir SolutionName/src
cd ./SolutionName
dotnet new sln ./
mkdir ./src/ProjectName
cd ./src/ProjectName
dotnet new pupamvcf-app
cd ../../
dotnet sln add ./src/ProjectName
cd ./src/ProjectName
dotnet run
```

## 📦 Dependencies

### PupaMVCF.Framework

- [Dapper](https://github.com/DapperLib/Dapper)
- [Microsoft.AspNetCore.Authentication.JwtBearer](https://github.com/dotnet/aspnetcore)
- [Microsoft.AspNetCore.Server.Kestrel](https://github.com/dotnet/aspnetcore)
- [Microsoft.Extensions.Caching.Memory](https://github.com/dotnet/runtime)
- [Microsoft.Extensions.Configuration](https://github.com/dotnet/runtime)
- [Microsoft.Extensions.Hosting](https://github.com/dotnet/runtime)
- [Microsoft.Extensions.Logging.Abstractions](https://github.com/dotnet/runtime)
- [PupaLib.Core](https://github.com/Artpupser/PupaLib.Core)
- [PupaLib.FileIO](https://github.com/Artpupser/PupaLib.FileIO)

### PupaMVCF.Template

- [dotenv.net](https://github.com/bolorundurowb/dotenv.net)
- [Npgsql](https://www.nuget.org/packages/Npgsql)
- [PupaMVCF.Framework](https://github.com/Artpupser/PupaMVCF)

## 🗃️ Devlog

### v0.2.2

- add: PupaMVCF.Template currently initializing database PgSQL
- add: json validator module
- add: ServiceCollectionExtensions
- add: Authentication jwt bearer
- add: PublicFolder how separate class [VirtualFolder PublicFolder]
- add: delete functions to database Repository
- add: ControllerScheme attribute, PrefixPattern for all handlers in controller
- fix: all yellow wrongs in build
- fix: ErrorMiddleware add ILogger to constructor
- fix: Components/Views not used StaticPrefix, currently used in all projects
- fix: StaticController fix path prefix /api/
- deleted: configuration extensions
- deleted: full removed ISession
- deleted: IValidatorModule -> ValidatorModule
- changed: validator initialization
- changed: template structure, PupaMVCF.Template,
- changed: update initialization scheme

### v0.2.1

- fix: session commit async conflict, next()

### v0.2.0

- deleted: protos, ExampleMacroProcess (need base)
- deleted: ReadExactlyAsync - optimization on Request.cs
- changed: router system on DI (Dependency Injection) base
- changed: big rework controller & middleware system
- changed: PupaMVCF.Web.Template -> PupaMVCF.Template
- changed: initialization steps
- add: database factory, repository
- add: .env files loading in PupaMVCF.Template
- add: banner.jpg
- changed: Readme.md file design
- changed: github workflows, optimized, package updated

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

