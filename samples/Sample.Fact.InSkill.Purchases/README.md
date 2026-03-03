# Build An Alexa Skill with In-Skill Purchases - Premium Fact (.NET)

This sample is a .NET port of the [Amazon Node.js Premium Fact sample](https://github.com/alexa-samples/skill-sample-nodejs-fact-in-skill-purchases), built with the [AlexaVoxCraft](https://github.com/LayeredCraft/alexa-vox-craft) library.

Adding premium content to your skill is a way to monetize it. This sample extends a basic fact skill by adding premium content categories accessible through either a subscription or a one-time purchase. This guide walks through the structure of the skill and how to deploy it to AWS Lambda.

## Skill Architecture

Each skill consists of two basic parts: a front end and a back end.

1. The **front end** is the voice interface (VUI), configured through the Alexa interaction model.
2. The **back end** is where the skill logic resides — in this case, an AWS Lambda function built with .NET.

## Project Structure

```
Sample.Fact.InSkill.Purchases/
├── Data/
│   └── FactData.cs                  # Static fact list across free, science, history, and space categories
├── Handlers/
│   ├── LaunchHandler.cs             # Welcome message, adapts based on owned products
│   ├── HelpHandler.cs               # AMAZON.HelpIntent
│   ├── YesHandler.cs                # AMAZON.YesIntent / GetRandomFactIntent
│   ├── NoHandler.cs                 # AMAZON.NoIntent
│   ├── GetCategoryFactHandler.cs    # GetCategoryFactIntent — serves facts or triggers upsell
│   ├── WhatCanIBuyHandler.cs        # WhatCanIBuyIntent — lists purchasable products
│   ├── ProductDetailHandler.cs      # ProductDetailIntent — describes a specific product
│   ├── BuyHandler.cs                # BuyIntent — initiates buy flow
│   ├── CancelSubscriptionHandler.cs # CancelSubscriptionIntent — initiates cancel flow
│   ├── BuyResponseHandler.cs        # Connections.Response for Buy / Upsell
│   ├── CancelResponseHandler.cs     # Connections.Response for Cancel
│   ├── StopCancelHandler.cs         # AMAZON.StopIntent / AMAZON.CancelIntent
│   ├── SessionEndedHandler.cs       # SessionEndedRequest
│   ├── FallbackHandler.cs           # AMAZON.FallbackIntent
│   └── ErrorHandler.cs              # Global exception handler
├── Helpers/
│   └── FactHelpers.cs               # Shared helpers: random fact, slot resolution, product list
├── Interceptors/
│   └── EntitledProductsInterceptor.cs  # Loads entitled products into session on new sessions
├── Program.cs                       # Lambda entry point and DI configuration
├── LambdaHandler.cs                 # ILambdaHandler implementation — routes to ISkillMediator
└── Constants.cs                     # Skill name and other constants
```

## In-Skill Purchasing Flow

This skill demonstrates three purchasing flows:

| Flow | Directive | Response Handler |
|---|---|---|
| Buy | `BuyDirective` | `BuyResponseHandler` |
| Upsell | `UpsellDirective` | `BuyResponseHandler` |
| Cancel | `CancelDirective` | `CancelResponseHandler` |

The `EntitledProductsInterceptor` runs on every new session and caches the customer's entitled products in session attributes, so handlers can gate content without making an API call on each request.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [AWS CLI](https://aws.amazon.com/cli/) configured with appropriate credentials
- [Amazon Developer Account](https://developer.amazon.com/)
- An Alexa skill with in-skill products configured in the [Alexa Developer Console](https://developer.amazon.com/alexa/console/ask)

## Getting Started

### 1. Install Lambda tools

```bash
dotnet new tool-manifest --force
dotnet tool install Amazon.Lambda.Tools
dotnet restore
```

### 2. Build and package for Lambda

```bash
dotnet lambda package
```

This uses `aws-lambda-tools-defaults.json` to produce a self-contained Lambda deployment package.

### 3. Deploy to AWS Lambda

Upload the generated `.zip` to your Lambda function, or use the AWS CLI:

```bash
aws lambda update-function-code \
  --function-name <your-function-name> \
  --zip-file fileb://bin/Release/net10.0/publish/Sample.Fact.InSkill.Purchases.zip
```

### 4. Update appsettings.json

Replace the placeholder skill ID with your actual skill ID from the Alexa Developer Console:

```json
{
  "SkillConfiguration": {
    "SkillId": "amzn1.ask.skill.<your-skill-id>"
  }
}
```

### 5. Configure in the Alexa Developer Console

- Set the Lambda ARN as your skill endpoint
- Create and link in-skill products (science pack, history pack, space pack, all-access subscription) via the developer console
- Ensure the interaction model includes the intents: `GetCategoryFactIntent`, `GetRandomFactIntent`, `WhatCanIBuyIntent`, `ProductDetailIntent`, `BuyIntent`, `CancelSubscriptionIntent`

## Key Differences from the Node.js Sample

| Concept | Node.js | .NET / AlexaVoxCraft |
|---|---|---|
| Request routing | `canHandle` / `handle` functions | `IRequestHandler<T>` with `CanHandle` / `Handle` |
| ISP API | `serviceClientFactory.getMonetizationServiceClient()` | `IInSkillPurchasingClient` (injected via DI) |
| Session attributes | Plain object property bag | `TryGetAttribute<T>` / `SetAttribute<T>` with type-discriminated serialization |
| Request interceptors | `addRequestInterceptors` | `IRequestInterceptor` registered via DI |
| Error handling | `addErrorHandlers` | `IExceptionHandler` registered via DI |
| Hosting | Alexa Hosted / Node.js runtime | AWS Lambda with provided.al2023 runtime (.NET NativeAOT-ready) |

## Additional Resources

### Community
- [Amazon Developer Forums](https://forums.developer.amazon.com/spaces/165/index.html) — Join the conversation
- [AlexaVoxCraft GitHub](https://github.com/LayeredCraft/alexa-vox-craft) — Library source and issues

### Documentation
- [Alexa In-Skill Purchasing Overview](https://developer.amazon.com/docs/in-skill-purchase/isp-overview.html)
- [Official Alexa Skills Kit Documentation](https://developer.amazon.com/docs/ask-overviews/build-skills-with-the-alexa-skills-kit.html)
- [AWS Lambda .NET Runtime](https://docs.aws.amazon.com/lambda/latest/dg/lambda-csharp.html)

## License

This sample code is made available under the Apache 2.0 license. See the [LICENSE](../../LICENSE) file.