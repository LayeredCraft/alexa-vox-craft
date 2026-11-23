# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

AlexaVoxCraft is a modular C# .NET library for building Amazon Alexa skills using modern .NET practices. It provides:

- **Core Models**: Complete Alexa skill request/response models with System.Text.Json serialization
- **APL Support**: Full Alexa Presentation Language implementation for visual interfaces
- **MediatR Integration**: CQRS-style request handling with pipeline behaviors
- **Lambda Hosting**: AWS Lambda runtime integration with custom serialization
- **Structured Logging**: Serilog integration with CloudWatch-compatible JSON formatting

## Architecture

### Core Components

The solution publishes 12 NuGet packages (every `src/` project except
`AlexaVoxCraft.MediatR.Generators`, which ships inside the `AlexaVoxCraft.MediatR` package as its
analyzer):

- **AlexaVoxCraft.Model**: Base Alexa skill models (requests, responses, directives)
- **AlexaVoxCraft.Model.Apl**: Comprehensive APL components and document support
- **AlexaVoxCraft.Model.InSkillPurchasing**: In-skill purchasing request/response models
- **AlexaVoxCraft.Model.WebApiGames**: Web API for Games (`Alexa.Presentation.HTML`) directives, Message request and capability check
- **AlexaVoxCraft.MediatR**: Core MediatR integration for request handling patterns
- **AlexaVoxCraft.MediatR.Lambda**: AWS Lambda hosting with middleware
- **AlexaVoxCraft.Lambda**: Lambda hosting abstractions and serialization
- **AlexaVoxCraft.MinimalLambda**: Hosting on MinimalLambda
- **AlexaVoxCraft.Http**: Shared HTTP client base for Alexa service clients
- **AlexaVoxCraft.InSkillPurchasing**: In-skill purchasing service client
- **AlexaVoxCraft.Smapi**: Skill Management API client
- **AlexaVoxCraft.Observability**: Opt-in OpenTelemetry instrumentation package

### Request Handling Pattern

Skills use the MediatR pattern where:
1. Requests implement `IRequestHandler<T>` 
2. Handlers optionally implement `ICanHandle` for routing logic
3. Pipeline behaviors handle cross-cutting concerns (logging, exceptions)
4. Lambda functions derive from `AlexaSkillFunction<TRequest, TResponse>`

### Key Entry Point Pattern

```csharp
// Program.cs
return await LambdaHostExtensions.RunAlexaSkill<Function, SkillRequest, SkillResponse>();

// Function class
public class Function : AlexaSkillFunction<SkillRequest, SkillResponse>
{
    protected override void Init(IHostBuilder builder)
    {
        builder
            .UseHandler<LambdaHandler, SkillRequest, SkillResponse>()
            .ConfigureServices((context, services) =>
            {
                services.AddSkillMediator(context.Configuration, cfg => 
                    cfg.RegisterServicesFromAssemblyContaining<Program>());
            });
    }
}
```

### Native AOT Compatibility

All in-scope runtime packages (`AlexaVoxCraft.Model`, `.Model.Apl`, `.Model.InSkillPurchasing`, `.Model.WebApiGames`, `.MediatR`,
`.MediatR.Lambda`, `.Lambda`, `.Http`, `.InSkillPurchasing`, `.Smapi`) are `IsAotCompatible=true` and
validated end to end by a rooted, continuously-CI-run real xUnit v3 Native AOT test project
(`test/AlexaVoxCraft.NativeAot.AotTests`, using [Compono.XunitV3.Aot](https://github.com/LayeredCraft/compono)
for AOT-safe, profile-based test composition - `[Compose<TProfile>]`/`[Compose<TProfile, TConfig>]`,
gated by `.github/workflows/native-aot-validation.yaml`. Replaced the original bespoke
`AlexaVoxCraft.NativeAot.ValidationApp` console app once Compono.XunitV3.Aot's Phase 2 profile support
shipped (LayeredCraft/compono#140) - see that project's own file-level comments for the design
rationale. See ADR-0001 (`docs/adr/0001-native-aot-compatibility.md`) and plan 0003
(`docs/plans/0003-native-aot-compatibility-implementation.md`) for the full architecture and
implementation history; consumer-facing guidance is in `docs/components/native-aot.md`.

Native AOT publish succeeds with zero IL warnings originating from the AOT test project itself. The
measured baseline (`main` `d07c5af`, SDK 11.0.100, `dotnet publish -r osx-arm64 -c Release` of
`test/AlexaVoxCraft.NativeAot.AotTests`) is **72 ILC warnings** (IL2026/IL3050/IL2070/IL2090), all
originating in pre-existing `AlexaVoxCraft` production code. They are accepted historical
exceptions dating from #188 (Native AOT compatibility), not caused by the test project; the exact
location-level listing is recorded in issue #214. (Before the AOT test conversion migrated its own
JSON call sites to `JsonTypeInfo<T>`-based overloads, the old validation app produced 96; the 28
attributable to test code were removed without any suppression.) Do not describe a Native AOT
publish here as "clean" or "warning-free" without specifying which category a given warning count
belongs to. The IL2070/IL2090 pair specifically (`EnumParser.cs`,
`EnumHelper.cs` - `Type`/generic-parameter reflection calls without matching
`DynamicallyAccessedMembers` annotations) is a distinct, legitimate finding from the
resolver-chain-uncertainty IL2026/IL3050 pattern the JSON warnings represent - it was investigated and
intentionally deferred as separate follow-up work, not fixed as part of the AOT test conversion.

Key internal mechanics (not part of any public contract, useful when touching this area):
- `AlexaJsonOptions`'s resolver chain: `ModelContext` (Model) → package-owned contexts
  (`AplModelContext`, `InSkillPurchasingModelContext`, `WebApiGamesModelContext`, `SmapiModelContext`,
  `InSkillPurchasingClientModelContext`) registered via the internal
  `RegisterPackageTypeInfoResolver` → consumer resolvers via the public `RegisterTypeInfoResolver` →
  JIT reflection fallback (present only when `JsonSerializer.IsReflectionEnabledByDefault`).
- `AlexaVoxCraft.Http`'s `DelegatingModelTypeInfoResolver` forwards to
  `AlexaJsonOptions.DefaultOptions.TypeInfoResolver` on every call rather than capturing it once, so a
  `BaseClient`-derived instance constructed before a consumer's `RegisterTypeInfoResolver` call still
  picks up that registration on its next actual use (the "resolver freshness invariant").
  `AlexaVoxCraft.Smapi`/`AlexaVoxCraft.InSkillPurchasing` compose their own package context with the
  same delegating resolver at each client's construction site.
  `AlexaJsonOptions.RegisterPackageTypeInfoResolver`/`AlexaVoxCraft.Http`'s `DelegatingModelTypeInfoResolver` are `internal` -
  `InternalsVisibleTo` grants access to the specific sibling assemblies that need them.
- `SkillMediator.Send` resolves the handler wrapper via `GetKeyedService<RequestHandlerWrapper>(requestType)`
  first (populated by `InterceptorEmitter.EmitHandlerRegistrations`'s generated `AddKeyedSingleton` calls),
  falling back to the pre-existing `MakeGenericType`-based path only when the interceptor never ran
  (`ServiceRegistrar`'s reflection-based assembly-scanning carve-out - not Native-AOT-supported, by design).
- APL's `APLValueConverterFactory`/`APLValueCollectionConverterFactory` dispatch via a closed
  `Dictionary<Type, Func<JsonConverter>>` keyed by the closed `APLValue<T>`/`APLValueCollection<T>`
  instantiation - no `MakeGenericType`/`Activator.CreateInstance`. New `APLValue<T>`/`APLValueCollection<T>`
  properties must add an entry to both the dictionary and `AplModelContext`. This was already the stated
  rule before issue #198 (a real deployed-Lambda Native AOT failure) found that most of the existing
  dictionary entries had never actually gotten the `AplModelContext` half of it - both converters'
  `Write` methods erase to `object` before their inner `JsonSerializer.Serialize` call, so every T needs
  an explicit root even though it's never a property's *declared* type.
  `AlexaVoxCraft.NativeAot.AotTests/AplModelContextCompletenessTests` now enforces this by reflecting
  over both dictionaries directly and asserting `AlexaJsonOptions.DefaultOptions.GetTypeInfo(T)` resolves
  for each entry, so a future T missing its `AplModelContext` root fails CI instead of shipping.
- A source-tree `ProjectReference` to `AlexaVoxCraft.MediatR` does **not** pull in its interceptor
  generator as an analyzer (only a packaged NuGet reference does, via the `analyzers/dotnet/cs` pack
  path) - any new source-tree consumer project (samples, the validation app) needs its own direct
  `ProjectReference` to `AlexaVoxCraft.MediatR.Generators` with `OutputItemType="Analyzer"`.

## Development Commands

### Build and Test
The SDK is pinned in `global.json`, which also selects Microsoft.Testing.Platform (MTP) as the
`dotnet test` runner, so `dotnet test` takes MTP options (`--solution`, `--project`), not legacy
VSTest flags.

```bash
# Restore dependencies
dotnet restore AlexaVoxCraft.slnx

# Build the entire solution (all target frameworks)
dotnet build AlexaVoxCraft.slnx -c Release --no-incremental
dotnet build AlexaVoxCraft.slnx -c Debug --no-incremental

# Run all tests (every test project, every target framework)
dotnet test --solution AlexaVoxCraft.slnx -c Release

# Run tests for a specific project
dotnet test --project test/AlexaVoxCraft.Model.Tests/AlexaVoxCraft.Model.Tests.csproj

# Publish and run the Native AOT test suite (net11.0; CI uses linux-x64)
dotnet publish test/AlexaVoxCraft.NativeAot.AotTests/AlexaVoxCraft.NativeAot.AotTests.csproj -r <rid> -c Release
./test/AlexaVoxCraft.NativeAot.AotTests/bin/Release/net11.0/<rid>/publish/AlexaVoxCraft.NativeAot.AotTests

# Clean build artifacts
dotnet clean AlexaVoxCraft.slnx

# Pack NuGet packages
dotnet pack AlexaVoxCraft.slnx -c Release
```

### Lambda Development
```bash
# Install AWS Lambda Tools (one time setup)
dotnet new tool-manifest --force
dotnet tool install Amazon.Lambda.Tools
dotnet restore

# Create Lambda deployment package using aws-lambda-tools-defaults.json configuration
dotnet lambda package

# Test Lambda functions locally (requires AWS Lambda Mock Test Tool)
dotnet run
```

Use `aws-lambda-tools-defaults.json` in skill projects to configure Lambda packaging:
```json
{
  "Information": [
    "This file provides default values for the deployment wizard inside Visual Studio and the AWS Lambda commands added to the .NET Core CLI.",
    "To learn more about the Lambda commands with the .NET Core CLI execute the following command at the command line in the project root directory.",
    "dotnet lambda help",
    "All the command line options for the Lambda command can be specified in this file."
  ],
  "profile": "",
  "region": "",
  "configuration": "Release",
  "function-runtime": "provided.al2023",
  "function-memory-size": 512,
  "function-timeout": 30,
  "function-handler": "bootstrap",
  "msbuild-parameters": "--self-contained true"
}
```

## Release and Versioning

Publishing is automated by GitHub Actions using the LayeredCraft devops templates
(`.github/workflows/*.yaml`, reusable workflows from `LayeredCraft/devops-templates`).

### Version Source

The published version is resolved by **Release Drafter** (`.github/release-drafter.yml`) from the
labels of the pull requests merged since the last release. The PR title check enforces Conventional
Commits titles, and the autolabeler maps them to labels:

- `breaking-change` (a `!` after the type, e.g. `feat!:`) → major
- `type: feat` (`feat:`) → minor
- `type: fix`, `type: docs`, `type: refactor`, `type: test`, `type: chore`, `type: ci`,
  `type: revert` → patch (also the default)

`<VersionPrefix>` in `Directory.Build.props` is **not** used for published versions; do not edit it
to cut a release.

### Release Process

- **Preview**: every push to `main` (each merged PR), except changes only under `docs/**` or
  `README.md`, runs `publish-preview.yaml`. It builds and tests the solution and pushes **all**
  packages to NuGet.org as `<resolved version>-preview.<run number>` (for example
  `8.1.0-preview.<run>` after a `feat:` PR). Merging a PR to `main` is therefore a package
  publication.
- **Stable**: publishing a GitHub release (from the Release Drafter draft) runs
  `publish-release.yaml`, which builds, tests and pushes all packages with the release version.
- Pull requests run **PR Build** (`pr-build.yaml`, MTP runner, all SDKs 8.0–11.0),
  **Native AOT Validation** (`native-aot-validation.yaml`) and the PR title check; nothing is
  published from a pull request or feature branch.

### Package Configuration

All packages inherit settings from `Directory.Build.props`:
- **License**: Apache-2.0 (SPDX identifier); third-party attributions in `NOTICE` and `licenses/`
- **Repository**: GitHub URL with SourceLink integration
- **Symbols**: Automatic `.snupkg` symbol packages for debugging
- **Metadata**: Author, project URL, repository type, README and icon

All packages are published together with the same version.

### Breaking Changes and Semantic Versioning

- **Major**: breaking changes requiring consumer code updates
- **Minor**: new features, backward compatible
- **Patch**: fixes and maintenance
- Public API changes must be additive unless a major version is intended.

## Testing Architecture

- **xUnit v3 on Microsoft.Testing.Platform**: every JIT test project uses `xunit.v3.mtp-v2` and runs
  through `dotnet test` with the MTP runner selected in `global.json` (see Development Commands).
  Test projects target net10.0/net11.0 (`test/Directory.Build.props`).
- **Compono**: the test-composition and test-double framework (source-generated; no AutoFixture).
  All Compono packages are pinned in lockstep in `Directory.Packages.props`; never mix versions.
- **AwesomeAssertions**: Fluent assertion library (same API as FluentAssertions but different package)
- **Verify** (`Verify.XunitV3`, `Verify.SourceGenerators`): snapshot tests for serialized models and
  generator output (`Snapshots/` folders; `*.received.*` files are never committed)
- **JSON Examples**: Extensive test data files for deserialization validation
- **Serialization Tests**: Round-trip validation of System.Text.Json converters
- **APL Component Tests**: Comprehensive APL document and component validation
- **Native AOT tests**: `test/AlexaVoxCraft.NativeAot.AotTests` is a net11.0 rooted executable using the
  separate `xunit.v3.aot.mtp-v2` + `Compono.XunitV3.Aot` family with reflection-based JSON disabled.
  Never reference `xunit.v3.mtp-v2`/`Compono.XunitV3` in that project (CS0433), or the AOT family in
  the JIT test projects.

### Compono conventions
- Reusable setup lives in small, concern-named composition profiles (`ICompositionProfile`) under each
  test project's `TestKit/` folder (for example `MediatRTestProfile`, `LambdaTestProfile`,
  `SmapiHttpTestProfile`), applied with `[Compose<TProfile>]`.
- Use `Compono.TestDoubles` (`UseGeneratedTestDoubles()`) for interface doubles, `Compono.Http` for
  HTTP clients, `Compono.Logging` for observable `ILogger<T>` output and `Compono.Options` for
  `IOptions<T>`. Add a Compono integration package only when a test needs it.
- Compose incidental values; keep behavior-defining values (wire field names, discriminators,
  boundaries, fixture JSON) as visible literals.
- Custom fakes or harnesses only where Compono lacks the capability; say why in the file (for
  example `FakeDelegates.cs`: Compono.TestDoubles does not generate delegate doubles).

### Testing Notes
- Use `AwesomeAssertions` package, NOT `FluentAssertions` (same API, different package)
- Run tests with `dotnet test --solution`/`--project` (MTP runner); `dotnet run` on a test project
  also works for ad hoc runs
- Propagate `TestContext.Current.CancellationToken` into async calls in tests

## Logging Configuration

For structured logging with AWS CloudWatch compatibility, use **LayeredCraft.Logging.CompactJsonFormatter**:
- `CompactJsonFormatter`: AWS CloudWatch-compatible JSON formatter 
- Renames reserved fields: `@t` → `_t`, `@l` → `_l`, `@m` → `_m`
- For additional structured logging features, use `LayeredCraft.StructuredLogging.Extensions`

Enable request/response logging in development:
```json
"AlexaVoxCraft.MediatR.Lambda.Serialization": "Debug"
```


## Key Patterns

### Request Handlers
```csharp
public class LaunchRequestHandler : IRequestHandler<LaunchRequest>
{
    public bool CanHandle(IHandlerInput handlerInput) => 
        handlerInput.RequestEnvelope.Request is LaunchRequest;

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken)
    {
        return await input.ResponseBuilder
            .Speak("Hello world!")
            .WithShouldEndSession(true)
            .GetResponse(cancellationToken);
    }
}
```

### Exception Handling
Exception handlers implement `IExceptionHandler` and are auto-registered:
```csharp
public class MyExceptionHandler : IExceptionHandler
{
    public Task<bool> CanHandle(IHandlerInput handlerInput, Exception ex, CancellationToken cancellationToken)
    {
        return Task.FromResult(true);
    }

    public Task<SkillResponse> Handle(IHandlerInput handlerInput, Exception ex, CancellationToken cancellationToken)
    {
        var response = handlerInput.ResponseBuilder.Speak("Something went wrong.");
        return response.GetResponse(cancellationToken);
    }
}
```

## Project Structure

- `/src/`: Library source code (12 NuGet packages plus the bundled MediatR source generator)
- `/samples/`: Working example skills (basic, APL, ISP, generated and host-based)
- `/test/`: Unit tests with extensive JSON example files, plus the Native AOT test project
- **Multi-targeting**: library and test projects target .NET 10.0 and 11.0 (.NET 8.0/9.0 are no longer supported)
  (`AlexaVoxCraft.MediatR.Generators` targets netstandard2.0; the Native AOT test project net11.0)
- **Lambda Optimized**: ReadyToRun publishing, ICU bundling, self-contained deployment options

## Key Features

- **Polymorphic JSON**: Custom converters for complex Alexa object hierarchies
- **APL Extensions**: Support for BackStack, DataStore, EntitySensing, SmartMotion
- **Session Management**: Context and state management for multi-turn conversations
- **Connection Tasks**: Support for complex multi-step interactions
- **AWS Integration**: Lambda runtime support with custom serialization
- **OpenTelemetry Integration**: Comprehensive observability with opt-in telemetry package

# Custom Instructions for Claude

## Code Style and Conventions
- NEVER add code comments unless explicitly requested
- Follow existing C# coding conventions and patterns in the codebase
- Use existing libraries and utilities - check imports and package.json/csproj before adding new dependencies
- When creating new components, examine existing similar components for patterns
- Prefer sealed classes when possible for better performance and design clarity

## Engineering Rules
- **Test-first (Red → Green → Refactor) for every behavior-bearing change**: write the test first,
  run it and observe it fail for the intended behavioral reason against compiling production code
  (a compile error or missing type is not Red), then make the minimal change that passes, then
  refactor while staying green. Bug fixes start with a reproducing regression test. Documentation,
  configuration-only and warning-only changes are exempt but must say so and be validated
  structurally (build, tests, inspection).
- **Zero ordinary C# compiler (CS) warnings** for every target framework, in Release and Debug and in
  the Native AOT publish.
- **Zero new warnings of any category** compared with the accepted analyzer/AOT baseline (the IL
  trim/AOT and SYSLIB config-binding warnings from #188, listed in #214). Compare diagnostics by code,
  project, target framework, location and message, not by count.
- **No suppression shortcuts**: no `NoWarn`, `#pragma warning disable`, `[SuppressMessage]`,
  `[UnconditionalSuppressMessage]`, lowered warning levels, disabled analyzers or dropped target
  frameworks to make a build pass. Repository-wide `TreatWarningsAsErrors` is not enabled while the
  accepted analyzer warnings remain. Do not rewrite existing converters only to remove accepted
  warnings.
- **Public API compatibility**: changes to published packages are additive unless a major version
  is intended; keep every supported target framework.
- **Native AOT compatibility**: in-scope packages stay `IsAotCompatible=true`; new runtime-dispatched
  serialization types are rooted in the owning package's source-generated context and covered by the
  Native AOT test project; no reflection-only fallbacks on AOT paths.

## Testing Guidelines
- Use Compono (profiles under `TestKit/`) for test composition and doubles; never AutoFixture
- Use `AwesomeAssertions` for assertions (same API as FluentAssertions)
- Validate both object constraints AND round-trip serialization through `AlexaJsonOptions`
- Assert wire JSON (field names, discriminators, value kinds, omitted members) against literal
  fixtures, not only self-round-trips
- Use `dotnet test` (MTP runner via `global.json`)

## Random Number Generation Guidelines
- **Application Code**: Use `Random.Shared.Next()` for performance
- **Tests**: Let Compono compose incidental values; use literals for values that define the behavior
  under test, so results stay deterministic and reproducible in CI

## Git and Development Workflow
- Always use descriptive commit messages following conventional commit style
- Include `🤖 Generated with [Claude Code](https://claude.ai/code)` and `Co-Authored-By: Claude <noreply@anthropic.com>` in commits
- Create branches with descriptive names (e.g., `feat/web-api-for-games`)
- When creating PRs, include comprehensive summary of changes and test coverage improvements

## Security and Best Practices
- Never expose or log secrets and keys
- Never commit secrets to the repository
- Always follow security best practices for Alexa skill development
- Validate input constraints according to Alexa specifications

## Model Testing Patterns
- Preserve JSON deserialization tests that validate converters
- Generate realistic test data that meets Alexa constraints
- Validate both object structure AND serialization behavior
- Test edge cases and constraint violations

## OpenTelemetry Implementation TODO

### Phase 1: Core Telemetry Infrastructure ✅
- [x] Create feature branch `feature/add-opentelemetry-instrumentation`
- [x] Add `AlexaVoxCraftTelemetry` static class to MediatR package
- [x] Add telemetry constants classes (no magic strings)
- [x] Verify all code compiles and tests pass
- [x] Commit: "Add core OpenTelemetry telemetry infrastructure"

### Phase 2: Pipeline Behavior Instrumentation ✅
- [x] Replace `PerformanceLoggingBehavior` with comprehensive OpenTelemetry instrumentation
- [x] Add `alexa.request` spans with semantic attributes and activity status management
- [x] Record core metrics (requests, latency, errors, slot resolutions, cold starts)
- [x] Add TimerScope helper with tag support for histogram measurements
- [x] Implement slot resolution tracking and response content analysis
- [x] Add session correlation with privacy-safe SHA256 hashing
- [x] Add error classification with dimensional tracking
- [x] Maintain full backward compatibility (all 113 tests pass)
- [x] Address Copilot feedback: entropy comment, APL constant, authority.Status null check
- [x] Commit: "Replace PerformanceLoggingBehavior with comprehensive OpenTelemetry instrumentation"

### Phase 3: Lambda Function Instrumentation ✅
- [x] Enhance `AlexaSkillFunction.FunctionHandlerAsync()` with spans
- [x] Add cold start detection and tracking
- [x] Add Lambda-level context and error handling
- [x] Add 6 new semantic attribute constants for Lambda context
- [x] Add Lambda metrics (duration and memory) with helper methods
- [x] Add comprehensive unit tests (6 new tests covering spans, attributes, metrics)
- [x] Maintain full backward compatibility (73/73 tests passing)
- [x] Remove unused `FaasExecution` constant for code cleanliness
- [x] Commit: "Add comprehensive OpenTelemetry instrumentation to Lambda function layer"

### Phase 4: Serialization Instrumentation ✅
- [x] Instrument `AlexaLambdaSerializer` with serialization spans
- [x] Track payload sizes and serialization performance
- [x] Add semantic attributes for direction, payload size, and type information
- [x] Record payload size metrics with proper directional tags
- [x] Add direction-specific timer helper methods
- [x] Maintain full backward compatibility (491 total tests passing)
- [x] Commit: "Implement Serialization Telemetry in AlexaLambdaSerializer (#63)"

### Phase 5: Handler Wrapper Instrumentation ✅
- [x] Enhance `RequestHandlerWrapperImpl` with handler spans
- [x] Track handler resolution and execution time with separate spans
- [x] Add semantic attributes for handler identification and execution order
- [x] Add metrics for resolution attempts, executions, and durations
- [x] Handle both regular handlers and default handler scenarios
- [x] Implement comprehensive error handling with proper span status
- [x] Maintain full backward compatibility (342 total tests passing)
- [x] Commit: "Implement Handler-Level OpenTelemetry Instrumentation (#64)"

### Phase 6: Observability Package ✅
- [x] Create new `AlexaVoxCraft.Observability` package
- [x] Add OTEL registration extension methods (`AddAlexaVoxCraftInstrumentation`)
- [x] Add package dependencies on OpenTelemetry packages
- [x] Add to solution and verify compilation
- [x] Commit: "Add AlexaVoxCraft.Observability package for OpenTelemetry instrumentation"

### Phase 7: Sample Integration
- [x] Update sample projects to demonstrate OTEL integration
- [x] Add configuration examples for ADOT/CloudWatch
- [x] Test end-to-end telemetry flow
- [x] Commit: "Update samples with OpenTelemetry integration examples"

### Phase 8: Documentation & PR
- [x] Update CLAUDE.md with telemetry usage instructions
- [x] Create comprehensive pull request
- [x] Include performance impact analysis
- [x] Add migration guide for existing users

## OpenTelemetry Usage (After Implementation)

### Basic Setup
```csharp
// In your skill's Program.cs
services.AddOpenTelemetry()
    .WithTracing(b => b.AddAlexaVoxCraft())
    .WithMetrics(b => b.AddAlexaVoxCraft());
```

### Key Metrics
- `alexa.requests` - Request counter by type/intent/locale
- `alexa.latency` - End-to-end request processing time
- `alexa.handler.duration` - Handler execution time by type and default status
- `alexa.handler.resolution.duration` - Handler CanHandle() resolution time
- `alexa.handler.executions` - Counter of successful handler executions by type
- `alexa.handler.resolution.attempts` - Counter of handler resolution attempts by type
- `alexa.serialization.duration` - Serialization time by direction (request/response)
- `alexa.payload.size` - Payload sizes in bytes by direction and type
- `alexa.errors` - Error counter by type
- `alexa.cold_starts` - Lambda cold start tracking
- `alexa.response.speech.characters` - SSML response length
- `alexa.lambda.duration` - Lambda execution time
- `alexa.lambda.memory_used` - Lambda memory usage

### Key Spans
- `alexa.lambda.execution` - Overall Lambda execution
- `alexa.request` - Request processing pipeline
- `alexa.handler.resolution` - Handler CanHandle() resolution evaluation
- `alexa.handler` - Individual handler execution (Handle() method)
- `alexa.serialization.request/response` - Serialization performance
- `alexa.apl.render` - APL document processing

### Semantic Attributes
All telemetry follows OpenTelemetry semantic conventions with Alexa-specific extensions:
- `rpc.system=alexa`, `rpc.method={IntentName|RequestType}`
- `alexa.request.type`, `alexa.intent.name`, `alexa.locale`
- `alexa.session.new`, `alexa.device.has_screen`
- `alexa.dialog.state`, `alexa.slot.resolution.status`
- `alexa.handler.type` - Handler class name for identification
- `alexa.handler.can_handle` - Handler resolution result (true/false)
- `alexa.handler.execution_order` - Handler evaluation order in resolution loop
- `alexa.handler.is_default` - Whether this is the default handler
- `alexa.serialization.direction` - "request" or "response"
- `alexa.payload.size` - Payload size in bytes
- `code.namespace`, `code.function` - Type information for serialized objects