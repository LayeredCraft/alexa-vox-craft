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

    public async Task<string> UpdateInteractionModelAsync(
        string skillId,
        string locale,
        InteractionModelDefinition model,
        CancellationToken cancellationToken = default)
    {
        return await _client.UpdateInteractionModelAsync(
            skillId,
            "development",
            locale,
            model,
            cancellationToken);
    }

    public async Task<InteractionModelDefinition?> GetInteractionModelAsync(
        string skillId,
        string locale,
        CancellationToken cancellationToken = default)
    {
        return await _client.GetInteractionModelAsync(
            skillId,
            "development",
            locale,
            cancellationToken);
    }
}
```

### Building Interaction Models

Use the fluent builder API to construct interaction models:

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
    await client.UpdateInteractionModelAsync(skillId, stage, locale, model);
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

## See Also

- [Amazon SMAPI Documentation](https://developer.amazon.com/docs/smapi/smapi-overview.html)
- [Login with Amazon Documentation](https://developer.amazon.com/docs/login-with-amazon/web-docs.html)
- [ASK CLI Documentation](https://developer.amazon.com/docs/smapi/quick-start-alexa-skills-kit-command-line-interface.html)