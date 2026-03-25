# SMAPI Developer Client

The SMAPI (Skill Management API) Developer Client enables programmatic management of Alexa skills from CI/CD pipelines and external tooling. It uses Login with Amazon (LWA) OAuth refresh token authentication with credentials obtained from the Amazon Developer Console.

## Prerequisites

Before using the SMAPI Developer Client, you need:

1. **Amazon Developer Account** - Register at [developer.amazon.com](https://developer.amazon.com)
2. **Security Profile** - Create a security profile in your Amazon Developer Console
3. **ASK CLI** - Install the Alexa Skills Kit CLI for token generation

## Getting Credentials

### Step 1: Create a Security Profile

1. Go to the [Amazon Developer Console](https://developer.amazon.com/settings/console/securityprofile/overview.html)
2. Click **Create a New Security Profile**
3. Fill in the required fields:
    - **Security Profile Name**: e.g., "SMAPI CI/CD Integration"
    - **Security Profile Description**: Describe its purpose
    - **Consent Privacy Notice URL**: Your privacy policy URL
4. Save the security profile
5. Navigate to **Web Settings** and note the **Client ID** and **Client Secret**

### Step 2: Generate LWA Tokens

Use the ASK CLI to generate Login with Amazon tokens:

```bash
# Install ASK CLI if not already installed
npm install -g ask-cli

# Configure ASK CLI
ask configure

# Generate LWA tokens with required scopes
ask util generate-lwa-tokens \
  --client-id YOUR_CLIENT_ID \
  --client-confirmation YOUR_CLIENT_SECRET \
  --scopes "SPACE_SEPARATED_SCOPES"
```

This command opens a browser for authorization and returns the access token and refresh token. Save the **refresh token** securely - this is the long-lived credential you'll use for CI/CD authentication.

For available scopes, see the [SMAPI Access Token Scopes documentation](https://developer.amazon.com/en-US/docs/alexa/smapi/get-access-token-smapi.html#scopes).

## Configuration

### Using appsettings.json

Add the following to your `appsettings.json`:

```json
{
  "SmapiClient": {
    "ClientId": "amzn1.application-oa2-client.xxxxx",
    "ClientSecret": "your-client-secret",
    "RefreshToken": "Atzr|xxxxx"
  }
}
```

!!! warning "Security Notice"
    Never commit credentials to source control. Use environment variables, Azure Key Vault, AWS Secrets Manager, or other secret management solutions for production deployments.

### Using Environment Variables

```bash
export SmapiClient__ClientId="amzn1.application-oa2-client.xxxxx"
export SmapiClient__ClientSecret="your-client-secret"
export SmapiClient__RefreshToken="Atzr|xxxxx"
```

## Registration

### Configuration-Based Registration

```csharp
using AlexaVoxCraft.Smapi;

var builder = WebApplication.CreateBuilder(args);

// Register with default section name "SmapiClient"
builder.Services.AddSmapiDeveloperClient(builder.Configuration);

// Or with a custom section name
builder.Services.AddSmapiDeveloperClient(builder.Configuration, "AlexaSkillManagement");
```

### Action-Based Registration

```csharp
using AlexaVoxCraft.Smapi;

builder.Services.AddSmapiDeveloperClient(options =>
{
    options.ClientId = "amzn1.application-oa2-client.xxxxx";
    options.ClientSecret = Environment.GetEnvironmentVariable("SMAPI_CLIENT_SECRET")!;
    options.RefreshToken = Environment.GetEnvironmentVariable("SMAPI_REFRESH_TOKEN")!;
});
```

## Usage

### Interaction Model Client

The `IAlexaInteractionModelClient` provides methods for managing skill interaction models:

```csharp
public class SkillDeploymentService
{
    private readonly IAlexaInteractionModelClient _client;

    public SkillDeploymentService(IAlexaInteractionModelClient client)
    {
        _client = client;
    }

    public async Task UpdateAsync(
        string skillId,
        string locale,
        InteractionModelDefinition model,
        CancellationToken cancellationToken = default)
    {
        await _client.UpdateAsync(skillId, "development", locale, model, cancellationToken);
    }

    public async Task<InteractionModelDefinition?> GetAsync(
        string skillId,
        string locale,
        CancellationToken cancellationToken = default)
    {
        return await _client.GetAsync(skillId, "development", locale, cancellationToken);
    }
}
```

#### Deploying a Single Localized Model

Use the `LocalizedInteractionModel` overload when you already have a locale paired with its definition:

```csharp
var localizedModel = new LocalizedInteractionModel("en-US", definition);
await _client.UpdateAsync(skillId, "development", localizedModel, cancellationToken);
```

#### Deploying Multiple Locales

`UpdateAllAsync` attempts every locale and collects failures into an `AggregateException` rather than stopping at the first error, ensuring all locales are always attempted:

```csharp
var models = new[]
{
    new LocalizedInteractionModel("en-US", enUsDefinition),
    new LocalizedInteractionModel("en-GB", enGbDefinition),
    new LocalizedInteractionModel("en-CA", enCaDefinition),
};

try
{
    await _client.UpdateAllAsync(skillId, "development", models, cancellationToken);
}
catch (AggregateException ex)
{
    foreach (var inner in ex.InnerExceptions)
        Console.Error.WriteLine(inner.Message);
}
```

### Building Interaction Models

#### Single-Locale Builder

Use `InteractionModelBuilder` to construct a model for a single locale:

```csharp
using AlexaVoxCraft.Smapi.Builders.InteractionModel;
using AlexaVoxCraft.Model.Request.Type;

var model = InteractionModelBuilder.Create()
    .WithInvocationName("my skill")
    .WithVersion("1")
    .WithDescription("My skill interaction model")
    .AddIntent("OrderPizzaIntent", intent =>
        intent.WithSlot("size", "PizzaSize")
              .WithSlot("topping", "PizzaTopping")
              .WithSamples(
                  "order a {size} pizza",
                  "I want a {size} {topping} pizza",
                  "order pizza"))
    .AddIntent(BuiltInIntent.Help)
    .AddIntent(BuiltInIntent.Cancel)
    .AddIntent(BuiltInIntent.Stop)
    .AddSlotType("PizzaSize", type =>
        type.WithValue("small", v => v.WithSynonyms("little", "mini"))
            .WithValue("medium", v => v.WithSynonyms("regular", "normal"))
            .WithValue("large", v => v.WithSynonyms("big", "extra large")))
    .AddSlotType("PizzaTopping", type =>
        type.WithValue("pepperoni")
            .WithValue("mushroom", v => v.WithSynonyms("mushrooms"))
            .WithValue("cheese", v => v.WithSynonyms("plain")))
    .Build();
```

Use `WithLocale` and `BuildLocalized` to produce a `LocalizedInteractionModel` directly, ready for upload:

```csharp
var localized = InteractionModelBuilder.Create()
    .WithLocale("en-US")
    .WithInvocationName("my skill")
    .WithVersion("1")
    .WithDescription("My skill interaction model")
    .AddIntent(BuiltInIntent.Help)
    .AddIntent(BuiltInIntent.Cancel)
    .BuildLocalized(); // returns LocalizedInteractionModel("en-US", ...)

await _client.UpdateAsync(skillId, "development", localized, cancellationToken);
```

#### Multi-Locale Builder

`MultiLocaleInteractionModelBuilder` lets you define the shared interaction model schema once (intent names, slot structure, slot type names) and specify locale-specific text per locale. Any text not overridden in an additional locale falls back to the default locale — similar to how `.resx` resource files work.

**Schema-level** elements (defined once on the builder):
- Intent names and slot structure
- Slot type names

**Locale-level** elements (defined per locale via `LocaleOverrideBuilder`):
- Invocation name
- Intent sample utterances
- Slot sample utterances
- Slot type values

```csharp
using AlexaVoxCraft.Smapi.Builders.InteractionModel;
using AlexaVoxCraft.Model.Request.Type;

var models = MultiLocaleInteractionModelBuilder.Create()
    .WithVersion("1")
    .WithDescription("My multi-locale skill")
    // Define the schema once — slot structure shared across all locales
    .AddIntent("OrderIntent", i => i.WithSlot("drink", "DrinkType"))
    .AddIntent(BuiltInIntent.Cancel)
    .AddIntent(BuiltInIntent.Stop)
    .AddSlotType("DrinkType")
    // Default locale provides the base text all other locales fall back to
    .WithDefaultLocale("en-US", locale => locale
        .WithInvocationName("my skill")
        .WithIntentSamples("OrderIntent", "order {drink}", "get me {drink}")
        .WithSlotValues("DrinkType", v => v
            .WithValue("coffee")
            .WithValue("tea")))
    // en-CA inherits everything from en-US
    .ForLocale("en-CA")
    // en-GB overrides only the samples that differ
    .ForLocale("en-GB", locale => locale
        .WithInvocationName("my british skill")
        .WithIntentSamples("OrderIntent", "order {drink}", "I'd like {drink}")
        .WithSlotValues("DrinkType", v => v
            .WithValue("coffee")
            .WithValue("tea")
            .WithValue("biscuit")))
    .BuildAll(); // IReadOnlyList<LocalizedInteractionModel>, default locale first

await _client.UpdateAllAsync(skillId, "development", models, cancellationToken);
```

`BuildAll` always includes the default locale first, followed by additional locales in registration order. Calling `ForLocale` with no lambda inherits everything from the default locale. Calling `ForLocale` multiple times for the same locale merges the overrides.

### Name-Free Interactions

Name-Free Interaction (NFI) allows users to interact with your skill without explicitly invoking its name. Configure NFI using the fluent builder API:

```csharp
using AlexaVoxCraft.Smapi.Builders.InteractionModel;

var model = InteractionModelBuilder.Create()
    .WithInvocationName("coffee shop")
    .WithVersion("1")
    .WithDescription("Coffee shop skill with name-free interactions")
    .AddIntent("OrderIntent", intent =>
        intent.WithSlot("drink", "DrinkType")
              .WithSamples("order {drink}", "buy {drink}"))
    .AddSlotType("DrinkType", type =>
        type.WithValue("coffee")
            .WithValue("tea")
            .WithValue("latte"))
    .WithNameFreeInteraction(nfi => nfi
        .WithLaunchIngressPoint(launch => launch
            .WithUtterances("what's available", "show menu", "what can I order"))
        .WithIntentIngressPoint("OrderIntent", intent => intent
            .WithUtterances("order coffee", "get tea", "buy a latte")))
    .Build();
```

#### NFI Builder Methods

**Launch Ingress Points** - Utterances that invoke the skill without the skill name:
```csharp
.WithLaunchIngressPoint(launch => launch
    .WithUtterance("what's new")
    .WithUtterances("latest updates", "recent changes"))
```

**Intent Ingress Points** - Utterances that map directly to intents:
```csharp
.WithIntentIngressPoint("OrderIntent", intent => intent
    .WithUtterances("order coffee", "buy tea"))
```

**Custom Utterance Formats** - Specify format for special utterance processing:
```csharp
.WithUtterance("special phrase", utterance => utterance
    .WithFormat("CUSTOM_FORMAT"))
```

**Multiple Ingress Points** - Combine launch and multiple intent ingress points:
```csharp
.WithNameFreeInteraction(nfi => nfi
    .WithLaunchIngressPoint(launch => launch
        .WithUtterances("start", "begin"))
    .WithIntentIngressPoint("OrderIntent", order => order
        .WithUtterances("order", "buy"))
    .WithIntentIngressPoint("StatusIntent", status => status
        .WithUtterance("check status")))
```

### JSON Serialization

Export the interaction model as JSON for debugging or manual deployment:

```csharp
var json = InteractionModelBuilder.Create()
    .WithInvocationName("my skill")
    // ... configure model
    .ToJson();

Console.WriteLine(json);
```

## CI/CD Integration

### GitHub Actions Example

```yaml
name: Deploy Interaction Model

on:
  push:
    branches: [main]
    paths:
      - 'interaction-models/**'

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '9.0.x'

      - name: Deploy Interaction Model
        env:
          SmapiClient__ClientId: ${{ secrets.SMAPI_CLIENT_ID }}
          SmapiClient__ClientSecret: ${{ secrets.SMAPI_CLIENT_SECRET }}
          SmapiClient__RefreshToken: ${{ secrets.SMAPI_REFRESH_TOKEN }}
        run: dotnet run --project tools/DeployInteractionModel
```

### Azure DevOps Pipeline Example

```yaml
trigger:
  branches:
    include:
      - main
  paths:
    include:
      - interaction-models/*

pool:
  vmImage: 'ubuntu-latest'

steps:
  - task: UseDotNet@2
    inputs:
      version: '9.0.x'

  - task: DotNetCoreCLI@2
    displayName: 'Deploy Interaction Model'
    inputs:
      command: 'run'
      projects: 'tools/DeployInteractionModel/DeployInteractionModel.csproj'
    env:
      SmapiClient__ClientId: $(SMAPI_CLIENT_ID)
      SmapiClient__ClientSecret: $(SMAPI_CLIENT_SECRET)
      SmapiClient__RefreshToken: $(SMAPI_REFRESH_TOKEN)
```

## Token Management

The SMAPI Developer Client automatically handles token refresh. Access tokens are cached and refreshed when they expire (with a small buffer to prevent edge cases). You don't need to manage tokens manually.

## Error Handling

The client throws standard HTTP exceptions for API errors:

```csharp
try
{
    await client.UpdateAsync(skillId, "development", locale, model, cancellationToken);
}
catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
{
    // Skill or locale not found
}
catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
{
    // Authentication failed - check credentials
}
catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
{
    // Rate limited - implement retry with backoff
}
```

When deploying multiple locales via `UpdateAllAsync`, individual locale failures are collected and thrown together as an `AggregateException` after all locales have been attempted. A `CancellationToken` cancellation propagates immediately:

```csharp
try
{
    await client.UpdateAllAsync(skillId, "development", models, cancellationToken);
}
catch (AggregateException ex)
{
    Console.Error.WriteLine($"Failed to update {ex.InnerExceptions.Count} locale(s):");
    foreach (var inner in ex.InnerExceptions)
        Console.Error.WriteLine($"  {inner.Message}");
}
```

## See Also

- [Amazon SMAPI Documentation](https://developer.amazon.com/docs/smapi/smapi-overview.html)
- [Login with Amazon Documentation](https://developer.amazon.com/docs/login-with-amazon/web-docs.html)
- [ASK CLI Documentation](https://developer.amazon.com/docs/smapi/quick-start-alexa-skills-kit-command-line-interface.html)