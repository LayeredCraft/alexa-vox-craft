# Migrate AlexaVoxCraft Tests to Verify + Embedded JSON Fixtures

This guide describes how to create new test projects (replacing the old ones) using **Verify.XunitV3**, **Module Initializers**, and **Embedded JSON fixtures**.

---

## 🧱 New Test Projects

Create two new projects to replace the archived ones:

- `test/AlexaVoxCraft.Model.Tests.V2`
- `test/AlexaVoxCraft.Model.Apl.Tests.V2`

Both will target `.NET 8`, `.NET 9`, and `.NET 10`.

---

## 0️⃣ Project Files (`.csproj`)

### AlexaVoxCraft.Model.Tests.V2.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsTestProject>true</IsTestProject>
    <IsPackable>false</IsPackable>
    <TargetFrameworks>net8.0;net9.0;net10.0</TargetFrameworks>
    <LangVersion>default</LangVersion>
    <TestingPlatformDotnetTestSupport>true</TestingPlatformDotnetTestSupport>
    <UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\AlexaVoxCraft.Model\AlexaVoxCraft.Model.csproj" />
    <ProjectReference Include="..\..\AlexaVoxCraft.TestKit\AlexaVoxCraft.TestKit.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Verify.XunitV3" Version="24.*" />
  </ItemGroup>

  <ItemGroup>
    <!-- Embed all JSON fixtures under Examples/ -->
    <EmbeddedResource Include="Examples\**\*.json" />
  </ItemGroup>
</Project>
```

### AlexaVoxCraft.Model.Apl.Tests.V2.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsTestProject>true</IsTestProject>
    <IsPackable>false</IsPackable>
    <TargetFrameworks>net8.0;net9.0;net10.0</TargetFrameworks>
    <LangVersion>default</LangVersion>
    <TestingPlatformDotnetTestSupport>true</TestingPlatformDotnetTestSupport>
    <UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\AlexaVoxCraft.Model.Apl\AlexaVoxCraft.Model.Apl.csproj" />
    <ProjectReference Include="..\..\AlexaVoxCraft.TestKit\AlexaVoxCraft.TestKit.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Verify.XunitV3" Version="24.*" />
  </ItemGroup>

  <ItemGroup>
    <EmbeddedResource Include="Examples\**\*.json" />
  </ItemGroup>
</Project>
```

---

## 1️⃣ Global Usings

Create `GlobalUsings.g.cs` in each project:

```csharp
global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Linq;
global using System.Reflection;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Threading.Tasks;

global using VerifyXunit;
global using Xunit;
```

---

## 2️⃣ Module Initializers

### Model Tests (`TestModuleInit.cs`)

```csharp
using System.Runtime.CompilerServices;
using System.Text.Json;
using VerifyTests;

public static sealed class TestModuleInit
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifierSettings.ScrubLinesContaining(
            "apiAccessToken", "Authorization", "Signature", "SignatureCertChainUrl");
        VerifierSettings.ScrubGuid();
        VerifierSettings.ScrubDateTimes();

        VerifierSettings.ModifySerialization(s =>
        {
            s.AddExtraSettings(o =>
            {
                o.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                o.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                o.WriteIndented = true;
            });
        });
    }
}
```

### APL Tests (`TestModuleInit.cs`)

```csharp
using System.Runtime.CompilerServices;
using System.Text.Json;
using VerifyTests;

public static sealed class TestModuleInit
{
    [ModuleInitializer]
    public static void Initialize()
    {
        APLSupport.Add();

        VerifierSettings.ScrubLinesContaining(
            "apiAccessToken", "Authorization", "Signature", "SignatureCertChainUrl");
        VerifierSettings.ScrubGuid();
        VerifierSettings.ScrubDateTimes();

        VerifierSettings.ModifySerialization(s =>
        {
            s.AddExtraSettings(o =>
            {
                o.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                o.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                o.WriteIndented = true;
            });
        });
    }
}
```

---

## 3️⃣ Shared Test Helpers

### AlexaJsonTest.cs

```csharp
public static sealed class AlexaJsonTest
{
    public static JsonSerializerOptions Options => AlexaJsonOptions.DefaultOptions;
}
```

### FixtureLoader.cs

```csharp
public static sealed class FixtureLoader
{
    public static string FromResource(string resourceName)
    {
        var asm = Assembly.GetExecutingAssembly();
        using var s = asm.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Missing embedded resource: {resourceName}");
        using var sr = new StreamReader(s);
        return sr.ReadToEnd();
    }

    public static string FromExamples(string relativePath)
    {
        var asm = Assembly.GetExecutingAssembly();
        var asmName = asm.GetName().Name!;
        var tail = relativePath.Replace('\\', '/').Replace('/', '.');
        var resource = $"{asmName}.Examples.{tail}";
        return FromResource(resource);
    }
}
```

### TestBase.cs

```csharp
public abstract class TestBase
{
    protected static JsonSerializerOptions AlexaJson => AlexaJsonTest.Options;
    protected static string Fx(string relativePath) => FixtureLoader.FromExamples(relativePath);
}
```

---

## 4️⃣ Example Tests

### Requests

```csharp
namespace AlexaVoxCraft.Model.Tests.V2.Requests;

[UsesVerify]
public sealed class IntentRequestDeserializeTests() : TestBase
{
    [Fact]
    public async Task IntentRequest_Deserializes_And_Projection_Is_Stable()
    {
        var json = Fx("Requests/IntentRequest.json");
        var envelope = JsonSerializer.Deserialize<RequestEnvelope>(json, AlexaJson)!;
        var intent = envelope.Request as IntentRequest;

        var view = new
        {
            Type = envelope.Request?.Type,
            Locale = envelope.Request?.Locale,
            Intent = intent?.Intent?.Name,
            Slots = intent?.Intent?.Slots?.ToDictionary(
                k => k.Key, v => new { v.Value?.Name, v.Value?.Value })
        };

        await Verify(view);
    }
}
```

### Responses

```csharp
namespace AlexaVoxCraft.Model.Tests.V2.Responses;

[UsesVerify]
public sealed class LaunchResponseSerializeTests() : TestBase
{
    [Fact]
    public async Task LaunchResponse_Serializes_As_Expected()
    {
        var response = new ResponseEnvelope
        {
            Version = "1.0",
            Response = new ResponseBody
            {
                ShouldEndSession = false,
                OutputSpeech = new PlainTextOutputSpeech
                {
                    Text = "Welcome to Jedi Duel! Say 'start' to begin your training."
                },
                Reprompt = new Reprompt
                {
                    OutputSpeech = new PlainTextOutputSpeech
                    {
                        Text = "Ready to begin? Say 'start'."
                    }
                }
            },
            SessionAttributes = new()
            {
                ["lastIntent"] = "LaunchRequest",
                ["sessionId"] = "stub-session"
            }
        };

        var json = JsonSerializer.Serialize(response, AlexaJson);
        await VerifyJson(json);
    }
}
```

### APL Tests

```csharp
namespace AlexaVoxCraft.Model.Apl.Tests.V2.Apl;

[UsesVerify]
public sealed class RenderDocumentDeserializeTests() : TestBase
{
    [Fact]
    public async Task RenderDocument_Deserializes_And_Projection_Is_Stable()
    {
        var json = Fx("APL/RenderDocument.json");
        var payload = JsonSerializer.Deserialize<RenderDocumentDirective>(json, AlexaJson)!;

        var view = new
        {
            Type = payload.Type,
            Token = payload.Token,
            DocumentType = payload.Document?.Type,
            HasDataSources = payload.DataSources?.Count > 0
        };

        await Verify(view);
    }
}
```

---

## 5️⃣ Folder Structure

```
test/
  AlexaVoxCraft.Model.Tests.V2/
    Examples/
      Requests/
        IntentRequest.json
      Responses/
        Response.json
    Requests/
    Responses/
  AlexaVoxCraft.Model.Apl.Tests.V2/
    Examples/
      APL/
        RenderDocument.json
    Apl/
```

---

## ✅ Run & Approve

```bash
dotnet restore
dotnet test test/AlexaVoxCraft.Model.Tests.V2 -f net9.0
dotnet test test/AlexaVoxCraft.Model.Apl.Tests.V2 -f net9.0
```

Open the `.verified.*` files and approve the baseline snapshots per Verify’s workflow.
