# In-Skill Purchasing

AlexaVoxCraft provides complete support for Alexa In-Skill Purchasing (ISP), allowing you to monetize your skill with one-time product purchases, subscriptions, and contextual upsell offers — all integrated with the same `IRequestHandler<T>` pattern used for every other request type.

> 🎯 **Premium Fact Skill Examples**: All code examples are drawn from the **Premium Fact** sample skill, which gates science, history, and space fact categories behind purchasable content packs and an all-access subscription.

## :rocket: Features

- **:dollar: Buy Flow**: Initiate a direct purchase of an in-skill product with `BuyDirective`
- **:speech_balloon: Upsell Flow**: Present a contextual offer mid-conversation with `UpsellDirective`
- **:x: Cancel Flow**: Cancel an active subscription or entitlement with `CancelDirective`
- **:inbox_tray: Response Handling**: Handle `Connections.Response` callbacks with typed `ConnectionResponseRequest<ConnectionResponsePayload>`
- **:mag: Product Catalog API**: Query available products and entitlements via `IInSkillPurchasingClient`
- **:shield: Entitlement Gating**: Check owned products and gate content behind entitlement checks
- **:arrows_counterclockwise: Interceptor Pattern**: Cache entitled products in session to avoid redundant API calls

## Packages

ISP support is split across two packages:

| Package | Purpose |
|---------|---------|
| `AlexaVoxCraft.Model.InSkillPurchasing` | Directives, response types, and payment type constants |
| `AlexaVoxCraft.InSkillPurchasing` | `IInSkillPurchasingClient` HTTP client with DI registration |

```bash
dotnet add package AlexaVoxCraft.Model.InSkillPurchasing
dotnet add package AlexaVoxCraft.InSkillPurchasing
```

## Registration

### Model Support

Call `InSkillPurchasingSupport.Add()` once at startup to register the ISP directive and response types with the AlexaVoxCraft serialization infrastructure:

```csharp
// Program.cs
InSkillPurchasingSupport.Add();

var builder = LambdaApplication.CreateBuilder();
builder.Services.AddSkillMediator(builder.Configuration);
builder.Services.AddAlexaSkillHost<LambdaHandler, SkillRequest, SkillResponse>();
builder.Services.AddInSkillPurchasing();

await using var app = builder.Build();
app.MapHandler(AlexaHandler.Invoke<SkillRequest, SkillResponse>);
await app.RunAsync();
```

`InSkillPurchasingSupport.Add()` registers:

- `PaymentDirective` with the directive JSON converter
- `BuyConnectionRequestHandler`, `UpsellConnectionRequestHandler`, and `CancelConnectionRequestHandler` with the `ConnectionSendRequestFactory`
- `ConnectionResponseHandler` with the `ConnectionResponseTypeResolver`

### HTTP Client

`AddInSkillPurchasing()` registers `IInSkillPurchasingClient` as an `HttpClient`-backed service. It automatically attaches the Alexa `apiAccessToken` as a bearer token and forwards the skill locale on every request:

```csharp
builder.Services.AddInSkillPurchasing();
```

The client targets `https://api.amazonalexa.com/` and requires no additional base URL configuration.

## Product Model

### Product

`IInSkillPurchasingClient` returns `Product` records from the Alexa ISP API:

```csharp
public sealed record Product(
    string ProductId,
    string Name,
    ProductType Type,
    string Summary,
    Purchasable Purchasable,
    Entitled Entitled,
    EntitledReason EntitledReason,
    string ReferenceName,
    int ActiveEntitlementCount,
    PurchaseMode PurchaseMode);
```

| Property | Description |
|----------|-------------|
| `ProductId` | Unique Alexa product identifier used in directives |
| `ReferenceName` | Developer-defined name (e.g., `science_pack`, `all_access`) |
| `Type` | `CONSUMABLE`, `ENTITLEMENT`, or `SUBSCRIPTION` |
| `Entitled` | `ENTITLED` or `NOT_ENTITLED` — whether the customer owns it |
| `Purchasable` | `PURCHASABLE` or `NOT_PURCHASABLE` |
| `Summary` | Short description shown in upsell offers |

### Enums

```csharp
public enum ProductType    { CONSUMABLE, ENTITLEMENT, SUBSCRIPTION }
public enum Purchasable    { NOT_PURCHASABLE, PURCHASABLE }
public enum Entitled       { NOT_ENTITLED, ENTITLED }
public enum EntitledReason { PURCHASED, NOT_PURCHASED, AUTO_ENTITLED }
public enum PurchaseMode   { LIVE, TEST }
```

## Querying Products

### Get All Products

```csharp
var result = await _ispClient.GetProductsAsync(cancellationToken: cancellationToken);
var products = result?.Products ?? [];
```

### Filter Products

Use `ProductFilter` to narrow results by entitlement, purchasability, type, or count:

```csharp
// Only entitled products
var result = await _ispClient.GetProductsAsync(
    new ProductFilter(Entitled: Entitled.ENTITLED),
    cancellationToken);

// Available packs (not yet purchased, purchasable)
var result = await _ispClient.GetProductsAsync(
    new ProductFilter(Entitled: Entitled.NOT_ENTITLED, Purchasable: Purchasable.PURCHASABLE),
    cancellationToken);
```

### Get a Single Product

```csharp
var product = await _ispClient.GetProductAsync(productId, cancellationToken);
```

### Check Voice Purchasing Status

```csharp
var purchasing = await _ispClient.GetPurchasingEnabledAsync(cancellationToken);
var isEnabled = purchasing?.PurchasingEnabled ?? false;
```

## Purchasing Flows

All three ISP flows follow the same pattern: add a directive to the response, Alexa presents the native purchasing UI, then sends a `Connections.Response` back to your skill. The session always ends when a payment directive is returned.

### :dollar: Buy Flow

Use `BuyDirective` when a customer explicitly requests to purchase a product:

```csharp
public class BuyHandler : IRequestHandler<IntentRequest>
{
    private readonly IInSkillPurchasingClient _ispClient;

    public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default) =>
        Task.FromResult(input.RequestEnvelope.Request is IntentRequest { Intent.Name: "BuyIntent" });

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        var request = (IntentRequest)input.RequestEnvelope.Request;
        var productCategory = FactHelpers.GetResolvedSlotValue(request, "productCategory");

        var referenceName = productCategory is null ? "all_access"
            : productCategory != "all_access" ? productCategory + "_pack"
            : productCategory;

        var result = await _ispClient.GetProductsAsync(cancellationToken: cancellationToken);
        var product = result?.Products.FirstOrDefault(p => p.ReferenceName == referenceName);

        if (product is not null)
        {
            return await input.ResponseBuilder
                .AddDirective(new BuyDirective(product.ProductId, "correlationToken"))
                .GetResponse(cancellationToken);
        }

        return await input.ResponseBuilder
            .Speak("I don't think we have a product by that name. Can you try again?")
            .Reprompt("Which product would you like to purchase?")
            .GetResponse(cancellationToken);
    }
}
```

### :speech_balloon: Upsell Flow

Use `UpsellDirective` to proactively offer premium content when a customer tries to access gated content they don't own. Set a `upsellMessage` on the `Payload` to customize the offer:

```csharp
public class GetCategoryFactHandler : IRequestHandler<IntentRequest>
{
    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        // ... resolve factCategory from slot ...

        var result = await _ispClient.GetProductsAsync(cancellationToken: cancellationToken);
        var products = result?.Products ?? [];

        var subscription = products.FirstOrDefault(p => p.ReferenceName == "all_access");
        var categoryProduct = products.FirstOrDefault(p => p.ReferenceName == $"{factCategory}_pack");

        var hasAccess = subscription?.Entitled == Entitled.ENTITLED ||
                        categoryProduct?.Entitled == Entitled.ENTITLED;

        if (hasAccess)
        {
            return await input.ResponseBuilder
                .Speak($"Here's your {factCategory} fact: {GetRandomFact(categoryFacts)}")
                .GetResponse(cancellationToken);
        }

        if (categoryProduct is not null)
        {
            var upsellMessage = $"You don't currently own the {factCategory} pack. " +
                                $"{categoryProduct.Summary} Want to learn more?";

            return await input.ResponseBuilder
                .AddDirective(new UpsellDirective(categoryProduct.ProductId, "correlationToken")
                {
                    Payload = new(categoryProduct.ProductId, upsellMessage)
                })
                .GetResponse(cancellationToken);
        }

        return await input.ResponseBuilder
            .Speak("I'm having trouble accessing those facts right now.")
            .GetResponse(cancellationToken);
    }
}
```

### :x: Cancel Flow

Use `CancelDirective` when a customer requests to cancel a subscription or entitlement:

```csharp
public class CancelSubscriptionHandler : IRequestHandler<IntentRequest>
{
    public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default) =>
        Task.FromResult(input.RequestEnvelope.Request is IntentRequest { Intent.Name: "CancelSubscriptionIntent" });

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        var result = await _ispClient.GetProductsAsync(cancellationToken: cancellationToken);
        var product = result?.Products.FirstOrDefault(p => p.ReferenceName == "all_access");

        if (product is not null)
        {
            return await input.ResponseBuilder
                .AddDirective(new CancelDirective(product.ProductId, "correlationToken"))
                .GetResponse(cancellationToken);
        }

        return await input.ResponseBuilder
            .Speak("I couldn't find that subscription. Can you try again?")
            .GetResponse(cancellationToken);
    }
}
```

## Handling Connections.Response

After the Alexa purchasing UI completes, Alexa sends a `Connections.Response` request back to your skill. Handle it with `IRequestHandler<ConnectionResponseRequest<ConnectionResponsePayload>>`.

Match on `PaymentType` constants to distinguish between Buy/Upsell and Cancel responses:

```csharp
public static class PaymentType
{
    public const string Buy    = nameof(Buy);
    public const string Cancel = nameof(Cancel);
    public const string Upsell = nameof(Upsell);
}
```

### Buy / Upsell Response Handler

```csharp
public class BuyResponseHandler : IRequestHandler<ConnectionResponseRequest<ConnectionResponsePayload>>
{
    private readonly IInSkillPurchasingClient _ispClient;

    public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default) =>
        Task.FromResult(input.RequestEnvelope.Request is ConnectionResponseRequest<ConnectionResponsePayload>
        {
            Name: PaymentType.Buy or PaymentType.Upsell
        });

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        var request = (ConnectionResponseRequest<ConnectionResponsePayload>)input.RequestEnvelope.Request;

        if (request.Status.Code != 200)
        {
            return await input.ResponseBuilder
                .Speak("There was an error with your purchase. Please try again.")
                .GetResponse(cancellationToken);
        }

        var product = await _ispClient.GetProductAsync(request.Payload.ProductId, cancellationToken);

        return request.Payload.PurchaseResult switch
        {
            "ACCEPTED" => await input.ResponseBuilder
                .Speak($"You have unlocked the {product?.Name}! Here's your first fact.")
                .Reprompt("Would you like another fact?")
                .GetResponse(cancellationToken),

            "DECLINED" => await input.ResponseBuilder
                .Speak("No problem. Would you like a free fact instead?")
                .Reprompt("Would you like a free fact?")
                .GetResponse(cancellationToken),

            "ALREADY_PURCHASED" => await input.ResponseBuilder
                .Speak($"You already own the {product?.Name}. Here's your fact!")
                .Reprompt("Would you like another fact?")
                .GetResponse(cancellationToken),

            _ => await input.ResponseBuilder
                .Speak("Something unexpected happened. Would you like a free fact?")
                .Reprompt("Would you like a free fact?")
                .GetResponse(cancellationToken)
        };
    }
}
```

### Cancel Response Handler

```csharp
public class CancelResponseHandler : IRequestHandler<ConnectionResponseRequest<ConnectionResponsePayload>>
{
    public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default) =>
        Task.FromResult(input.RequestEnvelope.Request is ConnectionResponseRequest<ConnectionResponsePayload>
        {
            Name: PaymentType.Cancel
        });

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        var request = (ConnectionResponseRequest<ConnectionResponsePayload>)input.RequestEnvelope.Request;

        if (request.Status.Code != 200)
        {
            return await input.ResponseBuilder
                .Speak("There was an error handling your cancellation. Please try again.")
                .GetResponse(cancellationToken);
        }

        return request.Payload.PurchaseResult switch
        {
            "ACCEPTED" => await input.ResponseBuilder
                .Speak("Your subscription has been cancelled. You can still ask for free facts.")
                .Reprompt("Would you like a free fact?")
                .GetResponse(cancellationToken),

            "NOT_ENTITLED" => await input.ResponseBuilder
                .Speak("You don't currently have a subscription to cancel.")
                .Reprompt("Would you like a free fact?")
                .GetResponse(cancellationToken),

            _ => await input.ResponseBuilder
                .Speak("There was an error with your cancellation. Please try again.")
                .GetResponse(cancellationToken)
        };
    }
}
```

### PurchaseResult Values

| Value | Description |
|-------|-------------|
| `ACCEPTED` | Customer completed the purchase or cancellation |
| `DECLINED` | Customer declined the offer |
| `ALREADY_PURCHASED` | Customer already owns this product |
| `NOT_ENTITLED` | Customer tried to cancel something they don't own |
| `ERROR` | Alexa encountered an internal error |

## Entitlement Caching with Interceptors

Making an `IInSkillPurchasingClient` API call on every request is inefficient. Use a request interceptor to load entitled products once per new session and cache them in session attributes:

```csharp
public class EntitledProductsInterceptor : IRequestInterceptor
{
    private readonly IInSkillPurchasingClient _ispClient;

    public EntitledProductsInterceptor(IInSkillPurchasingClient ispClient)
    {
        _ispClient = ispClient;
    }

    public async Task Process(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        var session = input.RequestEnvelope.Session;

        if (session?.New != true)
            return;

        var result = await _ispClient.GetProductsAsync(cancellationToken: cancellationToken);
        var entitledProducts = result?.Products
            .Where(p => p.Entitled == Entitled.ENTITLED)
            .ToArray() ?? [];

        var sessionAttributes = await input.AttributesManager.GetSessionAttributes(cancellationToken);
        sessionAttributes.SetAttribute("entitledProducts", entitledProducts);
        await input.AttributesManager.SetSessionAttributes(sessionAttributes, cancellationToken);
    }
}
```

Handlers then read the cached products from session attributes without making additional API calls:

```csharp
var sessionAttributes = await input.AttributesManager.GetSessionAttributes(cancellationToken);
sessionAttributes.TryGetAttribute<Product[]>("entitledProducts", out var entitledProducts);

var hasAccess = entitledProducts?.Any(p => p.ReferenceName is "all_access" or "science_pack") ?? false;
```

See [Session Management](session-management.md) for more on `TryGetAttribute<T>` and `SetAttribute<T>`.

## Purchasing Flow Reference

| Flow | Directive | Response Handler | `PaymentType` |
|------|-----------|-----------------|---------------|
| Direct purchase | `BuyDirective` | `BuyResponseHandler` | `PaymentType.Buy` |
| Contextual offer | `UpsellDirective` | `BuyResponseHandler` | `PaymentType.Upsell` |
| Cancel subscription | `CancelDirective` | `CancelResponseHandler` | `PaymentType.Cancel` |

## Directive Reference

All three directives extend `PaymentDirective`, which implements `IEndSessionDirective`. Returning any payment directive automatically ends the current session — Alexa takes over the purchasing UI.

```csharp
// Initiate a direct purchase
new BuyDirective(productId: product.ProductId, token: "correlationToken")

// Present a contextual upsell offer with a custom message
new UpsellDirective(productId: product.ProductId, token: "correlationToken")
{
    Payload = new PaymentPayload(product.ProductId, upsellMessage: "Want to unlock the science pack?")
}

// Initiate subscription cancellation
new CancelDirective(productId: product.ProductId, token: "correlationToken")
```

The `token` value is echoed back in the `ConnectionResponseRequest.Token` property and can be used to correlate the response to the originating directive.

## Best Practices

### 1. Always Check Status.Code Before Reading Payload

The `Connections.Response` status code indicates whether Alexa was able to process the request at all:

```csharp
if (request.Status.Code != 200)
{
    // Log and return a graceful error response
    return await input.ResponseBuilder
        .Speak("There was an error handling your request. Please try again.")
        .GetResponse(cancellationToken);
}
```

### 2. Cache Entitled Products in Session

Avoid making ISP API calls on every request turn. Load entitlements once per new session via an `IRequestInterceptor` and store them in session attributes with `SetAttribute<T>`:

```csharp
// In interceptor — runs once per new session
sessionAttributes.SetAttribute("entitledProducts", entitledProducts);

// In handlers — zero API cost
sessionAttributes.TryGetAttribute<Product[]>("entitledProducts", out var products);
```

### 3. Use ReferenceName for Product Lookup

`ProductId` is an opaque Alexa-assigned GUID. Use `ReferenceName` (your developer-defined identifier) for product lookups in your business logic:

```csharp
var product = products.FirstOrDefault(p => p.ReferenceName == "science_pack");
```

### 4. Use PaymentType Constants for Response Matching

Avoid magic strings when matching `Connections.Response` by name:

```csharp
// ✅ Use PaymentType constants
Name: PaymentType.Buy or PaymentType.Upsell

// ❌ Avoid magic strings
Name: "Buy" or "Upsell"
```

### 5. Register InSkillPurchasingSupport Before Processing Requests

`InSkillPurchasingSupport.Add()` registers type discriminators with the serialization infrastructure. Call it before the Lambda host starts processing requests:

```csharp
// At the top of Program.cs, before any builder calls
InSkillPurchasingSupport.Add();
```

## Examples

For a complete working implementation, see the [Sample.Fact.InSkill.Purchases](https://github.com/LayeredCraft/alexa-vox-craft/tree/main/samples/Sample.Fact.InSkill.Purchases) sample project — a .NET port of the Amazon Node.js Premium Fact ISP sample demonstrating all three purchasing flows.
