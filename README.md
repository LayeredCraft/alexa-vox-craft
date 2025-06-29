# 🔣 Alexa Vox Craft

**Alexa Vox Craft** is a modular, opinionated library for building Alexa skills in C# using .NET. It leverages `System.Text.Json`, MediatR patterns, and extensible components for building and maintaining robust Alexa skills with support for:

- ✅ Clean separation of concerns using MediatR
- ✅ JSON (de)serialization with full control via `System.Text.Json`
- ✅ APL (Alexa Presentation Language) model support and utilities
- ✅ Custom converters for object, enum, and polymorphic types
- ✅ Lambda-ready with logging, tracing, and testability in mind

### What does "AlexaVoxCraft" mean?

- **Alexa**: Represents the integration with Amazon Alexa, the voice service powering devices like Amazon Echo.
- **VoxCraft**: Signifies the focus on voice-driven interactions, leveraging the VoxCraft framework.

> 📦 **Credits:**
>
> - The core Alexa skill models (`AlexaVoxCraft.Model`) are based on the excellent work in [timheuer/alexa-skills-dotnet](https://github.com/timheuer/alexa-skills-dotnet).
> - APL support (`AlexaVoxCraft.Model.Apl`) is based on [stoiveyp/Alexa.NET.APL](https://github.com/stoiveyp/Alexa.NET.APL).

---

## 🏎️ Packages

| Package                          | Build Status                                                                                                                                                                  | NuGet                                                                                                                                       | GitHub                                       | Downloads                                                                                                                                            |
|----------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------|----------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------|
| **AlexaVoxCraft.Model**          | [![Build](https://github.com/LayeredCraft/alexa-vox-craft/actions/workflows/build.yaml/badge.svg)](https://github.com/LayeredCraft/alexa-vox-craft/actions/workflows/build.yaml)  | [![NuGet](https://img.shields.io/nuget/vpre/AlexaVoxCraft.Model.svg)](https://www.nuget.org/packages/AlexaVoxCraft.Model)                   | [📁 Source](src/AlexaVoxCraft.Model)         | [![NuGet Downloads](https://img.shields.io/nuget/dt/AlexaVoxCraft.Model.svg)](https://www.nuget.org/packages/AlexaVoxCraft.Model/)                   |
| **AlexaVoxCraft.Model.Apl**      | [![Build](https://github.com/LayeredCraft/alexa-vox-craft/actions/workflows/build.yaml/badge.svg)](https://github.com/LayeredCraft/alexa-vox-craft/actions/workflows/build.yaml)  | [![NuGet](https://img.shields.io/nuget/vpre/AlexaVoxCraft.Model.Apl.svg)](https://www.nuget.org/packages/AlexaVoxCraft.Model.Apl)           | [📁 Source](src/AlexaVoxCraft.Model.Apl)     | [![NuGet Downloads](https://img.shields.io/nuget/dt/AlexaVoxCraft.Model.Apl.svg)](https://www.nuget.org/packages/AlexaVoxCraft.Model.Apl/)           |
| **AlexaVoxCraft.MediatR.Lambda** | [![Build](https://github.com/LayeredCraft/alexa-vox-craft/actions/workflows/build.yaml/badge.svg)](https://github.com/LayeredCraft/alexa-vox-craft/actions/workflows/build.yaml)  | [![NuGet](https://img.shields.io/nuget/vpre/AlexaVoxCraft.MediatR.Lambda.svg)](https://www.nuget.org/packages/AlexaVoxCraft.MediatR.Lambda) | [📁 Source](src/AlexaVoxCraft.MediatR.Lambda) | [![NuGet Downloads](https://img.shields.io/nuget/dt/AlexaVoxCraft.MediatR.Lambda.svg)](https://www.nuget.org/packages/AlexaVoxCraft.MediatR.Lambda/) |
| **AlexaVoxCraft.MediatR**        | [![Build](https://github.com/LayeredCraft/alexa-vox-craft/actions/workflows/build.yaml/badge.svg)](https://github.com/LayeredCraft/alexa-vox-craft/actions/workflows/build.yaml)  | [![NuGet](https://img.shields.io/nuget/vpre/AlexaVoxCraft.MediatR.svg)](https://www.nuget.org/packages/AlexaVoxCraft.MediatR)               | [📁 Source](src/AlexaVoxCraft.MediatR) | [![NuGet Downloads](https://img.shields.io/nuget/dt/AlexaVoxCraft.MediatR.svg)](https://www.nuget.org/packages/AlexaVoxCraft.MediatR/)               |
| **AlexaVoxCraft.Logging**        | [![Build](https://github.com/LayeredCraft/alexa-vox-craft/actions/workflows/build.yaml/badge.svg)](https://github.com/LayeredCraft/alexa-vox-craft/actions/workflows/build.yaml)  | [![NuGet](https://img.shields.io/nuget/vpre/AlexaVoxCraft.Logging.svg)](https://www.nuget.org/packages/AlexaVoxCraft.Logging)               | [📁 Source](src/AlexaVoxCraft.Logging)       | [![NuGet Downloads](https://img.shields.io/nuget/dt/AlexaVoxCraft.Logging.svg)](https://www.nuget.org/packages/AlexaVoxCraft.Logging/)               |

---

## 📁 Project Structure

```
AlexaVoxCraft/
├── 📂 src/                              # Core library packages
│   ├── 📦 AlexaVoxCraft.Model/          # Base Alexa skill models & serialization
│   │   ├── Cards/                       # Card types (Simple, Standard, LinkAccount)
│   │   ├── Converters/                  # JSON converters & polymorphic handling
│   │   ├── Directives/                  # Audio, video, dialog directives
│   │   ├── Helpers/                     # Enum utilities & response builders
│   │   ├── Interfaces/                  # Core abstractions
│   │   ├── Request/                     # Request types (Launch, Intent, Session, etc.)
│   │   ├── Response/                    # Response models & builders
│   │   ├── Ssml/                        # Complete SSML element support
│   │   └── UI/                          # Display templates & visual cards
│   │
│   ├── 📦 AlexaVoxCraft.Model.Apl/      # APL (Alexa Presentation Language) support
│   │   ├── AlexaComponents/             # 40+ pre-built Alexa components
│   │   ├── Audio/                       # Audio documents & sound effects
│   │   ├── Commands/                    # 25+ APL commands (Animate, Scroll, etc.)
│   │   ├── Components/                  # Core APL components (Container, Text, etc.)
│   │   ├── Converters/                  # APL-specific JSON converters
│   │   ├── DataSources/                 # Dynamic data binding
│   │   ├── Extensions/                  # BackStack, DataStore, SmartMotion
│   │   ├── Filters/                     # Image & vector graphic filters
│   │   └── VectorGraphics/              # AVG (Alexa Vector Graphics) support
│   │
│   ├── 📦 AlexaVoxCraft.MediatR/        # MediatR integration & request handling
│   │   ├── Attributes/                  # Session & persistent attributes
│   │   ├── Behaviors/                   # Pipeline behaviors & interceptors
│   │   ├── Handlers/                    # Request handler abstractions
│   │   ├── Interfaces/                  # Handler & routing interfaces
│   │   └── Services/                    # DI registration & service discovery
│   │
│   ├── 📦 AlexaVoxCraft.MediatR.Lambda/ # AWS Lambda hosting & runtime
│   │   ├── Context/                     # Skill context management
│   │   ├── Extensions/                  # Host builder extensions
│   │   ├── Functions/                   # Lambda function base classes
│   │   ├── Handlers/                    # Lambda-specific handlers
│   │   └── Serialization/               # Custom Lambda serializers
│   │
│   └── 📦 AlexaVoxCraft.Logging/        # Alexa-specific logging for AWS
│       └── Serialization/               # CloudWatch-compatible JSON formatter
│
├── 📂 samples/                          # Working example projects
│   ├── 📱 Sample.Skill.Function/        # Basic Alexa skill demonstration
│   │   ├── Handlers/                    # Request handlers (Launch, Intent)
│   │   ├── Function.cs                  # Main Lambda function
│   │   ├── Program.cs                   # Entry point
│   │   └── appsettings.json             # Configuration
│   │
│   └── 📱 Sample.Apl.Function/          # APL skill with visual interfaces
│       ├── Handlers/                    # APL-enabled request handlers
│       ├── Function.cs                  # APL skill function
│       └── Program.cs                   # Entry point
│
├── 📂 test/                             # Comprehensive test coverage
│   ├── 🧪 AlexaVoxCraft.Model.Tests/    # Core model & serialization tests
│   │   ├── Examples/                    # 50+ JSON test files
│   │   ├── Cards/                       # Card serialization tests
│   │   ├── Converters/                  # JSON converter validation
│   │   ├── Directives/                  # Directive handling tests
│   │   ├── Request/                     # Request parsing tests
│   │   ├── Response/                    # Response building tests
│   │   └── Ssml/                        # SSML element tests
│   │
│   └── 🧪 AlexaVoxCraft.Model.Apl.Tests/ # APL functionality tests
│       ├── Examples/                    # 80+ APL JSON examples
│       ├── AlexaComponents/             # Alexa component tests
│       ├── Audio/                       # Audio document tests
│       ├── Commands/                    # APL command tests
│       ├── Components/                  # Core component tests
│       ├── Extensions/                  # Extension tests (DataStore, etc.)
│       └── VectorGraphics/              # AVG parsing tests
│
├── 📂 .github/                          # DevOps & automation
│   ├── workflows/                       # CI/CD pipelines
│   │   ├── build.yaml                   # Main build & release pipeline
│   │   └── pr-build.yaml                # PR validation pipeline
│   └── dependabot.yml                   # Dependency management
│
├── 📂 licenses/                         # Third-party license files
├── 📄 AlexaVoxCraft.sln                 # Visual Studio solution
├── 📄 Directory.Build.props             # Shared MSBuild properties
├── 📄 README.md                         # This file
├── 📄 LICENSE                           # Apache 2.0 license
├── 📄 NOTICE                            # Legal attributions
└── 🖼️ icon.png                          # Package icon
```

### Package Breakdown

| Package | Purpose | Key Features |
|---------|---------|--------------|
| **AlexaVoxCraft.Model** | Core Alexa models | Request/response types, SSML, cards, directives, System.Text.Json serialization |
| **AlexaVoxCraft.Model.Apl** | APL support | 40+ components, commands, audio, vector graphics, extensions (DataStore, SmartMotion) |
| **AlexaVoxCraft.MediatR** | Request handling | Handler routing, pipeline behaviors, attributes management, DI integration |
| **AlexaVoxCraft.MediatR.Lambda** | Lambda hosting | AWS Lambda functions, context management, custom serialization, hosting extensions |
| **AlexaVoxCraft.Logging** | Alexa-specific logging | AWS CloudWatch-compatible JSON formatter, built on LayeredCraft.StructuredLogging |

---

## 🚀 Getting Started

### 1. Install Required Packages

Install only the packages you need! If you're not using MediatR or APL features, you can omit those dependencies.

```bash
dotnet add package AlexaVoxCraft.Model
# Optional:
dotnet add package AlexaVoxCraft.MediatR.Lambda
```

### 2. Create a Lambda Entry Point

```csharp
await LambdaHostExtensions.RunAlexaSkill<YourAlexaSkillFunction>();
```

### 3. Create Your Function Class

```csharp
public sealed class YourAlexaSkillFunction : AlexaSkillFunction<SkillRequest, SkillResponse>
{
    protected override void Init(IHostBuilder builder)
    {
        builder
            .UseHandler<LambdaHandler, SkillRequest, SkillResponse>()
            .ConfigureServices((context, services) =>
            {
                services.AddSkillMediator(context.Configuration,
                    cfg => { cfg.RegisterServicesFromAssemblyContaining<Program>(); });
            });
    }
}
```

### 4. Handle Requests with MediatR

Handlers in AlexaVoxCraft are expected to implement `IRequestHandler<T>` and optionally implement `ICanHandle` to provide routing logic.

```csharp
public sealed class LaunchRequestHandler : IRequestHandler<LaunchRequest>
{
    public bool CanHandle(IHandlerInput handlerInput) =>
        handlerInput.RequestEnvelope.Request is LaunchRequest;

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        return await input.ResponseBuilder.Speak("Hello world!").WithShouldEndSession(true)
            .GetResponse(cancellationToken);
    }
}
```
---

# 📋 Logging & Diagnostics

AlexaVoxCraft provides comprehensive structured logging through [LayeredCraft.StructuredLogging](https://www.nuget.org/packages/LayeredCraft.StructuredLogging/), which includes performance timing, scoped logging, and structured data capture. The framework uses [Serilog](https://serilog.net/) and automatically logs request processing, handler execution, and performance metrics.

## Logging Configuration

Configure logging in your `appsettings.json`:

```json
{
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console" ],
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "formatter": "AlexaVoxCraft.Logging.Serialization.AlexaCompactJsonFormatter, AlexaVoxCraft.Logging"
        }
      }
    ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.AspNetCore": "Warning",
        "System": "Warning"
      }
    }
  }
}
```

## Debugging and Development

### Enable AlexaVoxCraft Debug Logging

To see detailed request processing, handler resolution, and performance metrics during development, add AlexaVoxCraft namespaces to the Override section:

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.AspNetCore": "Warning",
        "System": "Warning",
        "AlexaVoxCraft": "Debug"
      }
    }
  }
}
```

### Enable Raw Request/Response Logging

To log raw Alexa JSON payloads (useful for debugging serialization issues):

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Override": {
        "AlexaVoxCraft.MediatR.Lambda.Serialization": "Debug"
      }
    }
  }
}
```

> ⚠️ **Security Warning:** Raw request/response logging may include sensitive user data. Only enable this in development environments.

## What Gets Logged

AlexaVoxCraft automatically logs:

- **Request Processing**: Request type, intent names, session IDs, application IDs
- **Performance Metrics**: Handler execution time, serialization/deserialization duration
- **Lambda Context**: Function name, request ID, remaining execution time
- **Handler Resolution**: Which handlers are selected for requests
- **Error Handling**: Exceptions with full context and stack traces
- **APL Support**: Whether APL is supported and visual directives added

## Production Logging Levels

For production environments, the framework uses appropriate log levels:

- **Information**: High-level milestones (Lambda execution started/completed)
- **Debug**: Detailed operational information (request processing, handler resolution, performance metrics)
- **Warning/Error**: Issues that need attention

This ensures production logs remain clean while providing rich debugging information when needed.
### 🧾 Formatter Attribution

> 🔧 The `AlexaCompactJsonFormatter` included in this library is adapted from [`Serilog.Formatting.Compact.CompactJsonFormatter`](https://github.com/serilog/serilog-formatting-compact).  
> This customized formatter renames reserved field names (e.g., `@t`, `@l`, `@m`) to AWS-safe equivalents (`_t`, `_l`, `_m`) to improve compatibility with CloudWatch Logs and metric filters.
>
> Original work © [Serilog Contributors](https://github.com/serilog/serilog-formatting-compact), licensed under the [MIT License](https://github.com/serilog/serilog-formatting-compact/blob/dev/LICENSE).
---

## 🧪 Unit Testing

Sample projects show how to load Alexa requests from JSON and assert deserialized structure. Tests include validation of:

- Correct parsing of SkillRequest types
- APL component deserialization
- Proper usage of custom converters

---

## 🛠 Utilities and Helpers

- `EnumHelper`: Convert to/from `[EnumMember]`-decorated enums
- `ObjectConverter`: Deserialize polymorphic object values
- `BasePolymorphicConverter<T>`: Handle APL and directive subtypes
- `AlexaLambdaSerializer`: Custom `ILambdaSerializer` with logging support

---

## ⚠️ Error Handling

To intercept and respond to exceptions in your MediatR pipeline, implement the `IExceptionHandler` interface:

```csharp
public sealed class MyExceptionHandler : IExceptionHandler
{
    public Task<bool> CanHandle(IHandlerInput handlerInput, Exception ex, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true); // Catch all
    }

    public Task<SkillResponse> Handle(IHandlerInput handlerInput, Exception ex, CancellationToken cancellationToken = default)
    {
        var response = handlerInput.ResponseBuilder.Speak("Something went wrong. Please try again.");
        return response.GetResponse(cancellationToken);
    }
}
```

No manual registration is required. Exception handlers are picked up automatically via `AddSkillMediator(...)`.

---

## 🛁 Sample Projects

| Sample Project                                              | Description                                        |
|-------------------------------------------------------------|----------------------------------------------------|
| [`Sample.Skill.Function`](samples/Sample.Skill.Function)   | A minimal Alexa skill using this library           |
| [`Sample.Apl.Function`](samples/Sample.Apl.Function)       | A sample APL skill to demonstrate working with APL |

Each sample demonstrates MediatR integration, serialization support, custom directives, and Lambda bootstrapping.

---

## 🤭 Roadmap

- ✅ Full widget lifecycle support
- ✅ Advanced directive handling
- ⚖️ OpenTelemetry & logging enrichment
- 🔄 Documentation site

---

## 🤝 Contributing

PRs are welcome! Please submit issues and ideas to help make this toolkit even better.

---

## Contributors ✨

Thanks goes to these wonderful people ([emoji key](https://allcontributors.org/docs/en/emoji-key)):

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->
<table>
  <tbody>
    <tr>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/LayeredCraft"><img src="https://avatars.githubusercontent.com/u/1405469?v=4?s=100" width="100px;" alt="Nick Cipollina"/><br /><sub><b>Nick Cipollina</b></sub></a><br /><a href="#content-LayeredCraft" title="Content">🔓</a></td>
    </tr>
  </tbody>
</table>

<!-- markdownlint-restore -->
<!-- prettier-ignore-end -->

<!-- ALL-CONTRIBUTORS-LIST:END -->

This project follows the [all-contributors](https://github.com/all-contributors/all-contributors) specification. Contributions of any kind welcome!

---

## 📜 License

MIT

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
## Stargazers over time

[![Stargazers over time](https://starchart.cc/LayeredCraft/alexa-vox-craft.svg)](https://starchart.cc/LayeredCraft/alexa-vox-craft)

