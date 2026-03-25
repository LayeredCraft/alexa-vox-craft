# Multi-Locale Interaction Model Builder — Implementation Plan

**Related ADR:** [ADR-0001 — Multi-Locale Interaction Model Builder](../adr/0001-multi-locale-interaction-model-builder.md)
**Date:** 2026-03-25

---

## Goals

1. Add locale awareness to `InteractionModelBuilder` via `WithLocale` + `BuildLocalized()` (non-breaking).
2. Introduce `MultiLocaleInteractionModelBuilder` with a shared schema and per-locale override model using a `.resx`-style default locale fallback.
3. Add `LocalizedInteractionModel` as the pairing type (locale + definition) used throughout.
4. Add `UpdateAsync(LocalizedInteractionModel)` and `UpdateAllAsync` convenience overloads to the client.

---

## New Files

| File | Purpose |
|---|---|
| `src/AlexaVoxCraft.Smapi/Models/InteractionModel/LocalizedInteractionModel.cs` | Record pairing `Locale` + `InteractionModelDefinition` |
| `src/AlexaVoxCraft.Smapi/Builders/InteractionModel/LocaleOverrideBuilder.cs` | Builder for locale-specific text overrides; validates against schema at call time |
| `src/AlexaVoxCraft.Smapi/Builders/InteractionModel/MultiLocaleInteractionModelBuilder.cs` | Top-level orchestrator with shared schema, `WithDefaultLocale`, `ForLocale`, `BuildAll()` |
| `test/AlexaVoxCraft.Smapi.Tests/Builders/InteractionModel/MultiLocaleInteractionModelBuilderTests.cs` | Snapshot + exception tests |

---

## Modified Files

| File | Change |
|---|---|
| `src/.../Builders/InteractionModel/IntentBuilder.cs` | Add `internal` members: `Name`, `SlotNames`, `BuildWithSamples(samples)` |
| `src/.../Builders/InteractionModel/SlotBuilder.cs` | Add `internal` members: `Name`, `BuildWithSamples(samples)` |
| `src/.../Builders/InteractionModel/InteractionModelBuilder.cs` | Add `WithLocale(string)` + `BuildLocalized()` — `Build()` and `ToJson()` unchanged |
| `src/.../Clients/IAlexaInteractionModelClient.cs` | Add `UpdateAsync(skillId, stage, LocalizedInteractionModel, ct)` + `UpdateAllAsync(...)` |
| `src/.../Clients/AlexaInteractionModelClient.cs` | Implement the two new overloads |
| `test/.../Builders/InteractionModel/InteractionModelBuilderTests.cs` | Add 2 tests for `BuildLocalized` and missing locale guard |
| `test/.../Clients/AlexaInteractionModelClientTests.cs` | Add tests for new client overloads |

---

## Design

### `LocalizedInteractionModel`

```csharp
public sealed record LocalizedInteractionModel(string Locale, InteractionModelDefinition Definition);
```

Not serialized as a unit — `Definition` is what gets PUT to SMAPI; `Locale` goes in the URL.

---

### `InteractionModelBuilder` additions (backward compatible)

```csharp
public InteractionModelBuilder WithLocale(string locale) { ... }

// Throws if locale not set via WithLocale
public LocalizedInteractionModel BuildLocalized() { ... }
```

`Build()` and `ToJson()` remain unchanged.

---

### `LocaleOverrideBuilder`

Holds locale-specific overrides. Constructed internally by `MultiLocaleInteractionModelBuilder` and passed to the caller's `Action<LocaleOverrideBuilder>` lambda. Constructor is `internal`.

Takes schema references (live dictionary references from the parent builder) so it can validate at call time.

**Public API:**

```csharp
public LocaleOverrideBuilder WithInvocationName(string name);

// Validates intentName exists in schema — throws if not
public LocaleOverrideBuilder WithIntentSamples(string intentName, params string[] samples);

// Validates intent + slot exist in schema — throws if not
public LocaleOverrideBuilder WithSlotSamples(string intentName, string slotName, params string[] samples);

// Validates slotTypeName exists in schema — throws if not
public LocaleOverrideBuilder WithSlotValues(string slotTypeName, Action<SlotTypeBuilder> configure);
```

Override semantics are **replacement**, not accumulation — locale overrides declare the full locale-specific set, not additions to the default.

---

### `MultiLocaleInteractionModelBuilder`

```csharp
public sealed class MultiLocaleInteractionModelBuilder
{
    public static MultiLocaleInteractionModelBuilder Create();

    // Shared metadata
    public MultiLocaleInteractionModelBuilder WithVersion(string version);
    public MultiLocaleInteractionModelBuilder WithDescription(string description);

    // Shared schema (structure only — no samples at this level)
    public MultiLocaleInteractionModelBuilder AddIntent(string name, Action<IntentBuilder>? configure = null);
    public MultiLocaleInteractionModelBuilder AddSlotType(string name, Action<SlotTypeBuilder>? configure = null);
    public MultiLocaleInteractionModelBuilder WithNameFreeInteraction(Action<NameFreeInteractionBuilder>? configure = null);

    // Default locale — base resource file; included in BuildAll() output
    public MultiLocaleInteractionModelBuilder WithDefaultLocale(string locale, Action<LocaleOverrideBuilder> configure);

    // Additional locales — null/no lambda means inherit everything from default
    public MultiLocaleInteractionModelBuilder ForLocale(string locale, Action<LocaleOverrideBuilder>? configure = null);

    // Merges default + overrides per locale; throws if no default locale set
    public IReadOnlyList<LocalizedInteractionModel> BuildAll();
}
```

**Notes:**
- `AddSlotType` makes `configure` optional (unlike single builder) — slot values are locale-specific and often not set at the schema level
- `ForLocale` with a locale matching `_defaultLocale` applies the configure to `_defaultOverrides` directly
- Duplicate `ForLocale` calls for the same locale merge into the same `LocaleOverrideBuilder`
- Output order: default locale first, then `ForLocale` registrations in insertion order

---

### `BuildAll()` merge algorithm

```
1. Guard: no WithDefaultLocale called → InvalidOperationException("A default locale must be specified using WithDefaultLocale.")
2. Guard: version/description not set → same as InteractionModelBuilder.Build()
3. Enumerate all locales: [defaultLocale] + forLocale registrations (in insertion order, deduped)
4. For each locale:
   a. Resolve invocationName:   locale override → default
   b. For each intent, resolve samples:  locale override → default
   c. For each intent's slot, resolve slot samples:  locale override → default
   d. For each slot type, resolve values:  locale override → default
   e. Build LanguageModel → InteractionModelBody → InteractionModelDefinition
   f. Emit LocalizedInteractionModel(locale, definition)
```

---

### Internal additions to `IntentBuilder` and `SlotBuilder`

These are `internal` — not part of the public API.

**`IntentBuilder`:**
```csharp
internal string Name { get; }
internal IReadOnlyCollection<string> SlotNames { get; }

// Builds the intent with caller-supplied samples instead of accumulated _samples
// Reuses slot structure unchanged
internal Intent BuildWithSamples(IReadOnlyList<string> samples);
```

**`SlotBuilder`:**
```csharp
internal string Name { get; }

// Builds the slot with caller-supplied samples
internal IntentSlot BuildWithSamples(IReadOnlyList<string> samples);
```

`BuildWithSamples` avoids mutating shared builder state — builders remain idempotent and `BuildAll()` can be called multiple times safely.

---

### Client additions

```csharp
// IAlexaInteractionModelClient
Task UpdateAsync(string skillId, string stage, LocalizedInteractionModel model, CancellationToken ct);
Task UpdateAllAsync(string skillId, string stage, IEnumerable<LocalizedInteractionModel> models, CancellationToken ct);

// AlexaInteractionModelClient
public Task UpdateAsync(string skillId, string stage, LocalizedInteractionModel model, CancellationToken ct)
    => UpdateAsync(skillId, stage, model.Locale, model.Definition, ct);

public async Task UpdateAllAsync(string skillId, string stage, IEnumerable<LocalizedInteractionModel> models, CancellationToken ct)
{
    var errors = new List<Exception>();
    foreach (var model in models)
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            await UpdateAsync(skillId, stage, model, ct).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            errors.Add(ex);
        }
    }

    if (errors.Count > 0)
        throw new AggregateException($"Failed to update {errors.Count} locale(s).", errors);
}
```

Sequential execution — no parallelism — to respect SMAPI rate limits. All locales are attempted regardless of individual failures; errors are collected and thrown together as an `AggregateException` at the end. `OperationCanceledException` propagates immediately to honour cancellation.

---

## Validation Rules

| Guard | When | Exception |
|---|---|---|
| No `WithDefaultLocale` | `BuildAll()` | `"A default locale must be specified using WithDefaultLocale."` |
| Missing version | `BuildAll()` | `"Interaction model version must be specified."` |
| Missing description | `BuildAll()` | `"Interaction model description must be specified."` |
| Unknown intent name | `WithIntentSamples` / `WithSlotSamples` call time | `"Intent '{name}' is not defined in the interaction model schema."` |
| Unknown slot name | `WithSlotSamples` call time | `"Slot '{slot}' is not defined on intent '{intent}'."` |
| Unknown slot type | `WithSlotValues` call time | `"Slot type '{name}' is not defined in the interaction model schema."` |
| Missing locale | `BuildLocalized()` (single builder) | `"A locale must be specified using WithLocale."` |

---

## Typical Usage

```csharp
// Single locale (additive to existing API)
InteractionModelBuilder.Create()
    .WithLocale("en-US")
    .WithInvocationName("my skill")
    .WithVersion("1").WithDescription("...")
    .AddIntent("OrderIntent", i => i.WithSamples("order {drink}"))
    .BuildLocalized(); // → LocalizedInteractionModel

// Multi-locale
var models = MultiLocaleInteractionModelBuilder.Create()
    .WithVersion("1").WithDescription("My skill")
    .AddIntent("OrderIntent", i => i.WithSlot("drink", "DrinkType"))
    .AddIntent(BuiltInIntent.Cancel)
    .AddIntent(BuiltInIntent.Stop)
    .AddSlotType("DrinkType")
    .WithDefaultLocale("en-US", locale => locale
        .WithInvocationName("my skill")
        .WithIntentSamples("OrderIntent", "order {drink}", "get {drink}")
        .WithSlotValues("DrinkType", v => v.WithValue("coffee").WithValue("tea")))
    .ForLocale("en-CA")                                  // inherits everything
    .ForLocale("en-GB", locale => locale
        .WithIntentSamples("OrderIntent", "order {drink}", "I'd like {drink}"))
    .BuildAll();
// → [en-US, en-CA (identical to en-US), en-GB (different samples)]

await client.UpdateAllAsync(skillId, "development", models, ct);
```

---

## Test Coverage

### `MultiLocaleInteractionModelBuilderTests` (new)

| Test | Type |
|---|---|
| `BuildAll_WithSingleDefaultLocale_ReturnsOneModel` | Snapshot |
| `BuildAll_WithMultipleLocales_ReturnsAllLocales` | Snapshot |
| `BuildAll_LocaleOverridingInvocationName_UsesOverrideForThatLocale` | Snapshot |
| `BuildAll_WithIntentSamplesOverride_AppliesOverrideForLocale` | Snapshot |
| `BuildAll_WithSlotSamplesOverride_AppliesOverrideForLocale` | Snapshot |
| `BuildAll_WithSlotValuesOverride_AppliesOverrideForLocale` | Snapshot |
| `BuildAll_WithForLocaleAndNoLambda_InheritsAllFromDefault` | Snapshot |
| `BuildAll_WithDuplicateForLocale_MergesIntoSameBuilder` | Snapshot |
| `BuildAll_WithNoDefaultLocale_ThrowsInvalidOperationException` | Exception |
| `WithIntentSamples_ForUnknownIntent_ThrowsInvalidOperationException` | Exception |
| `WithSlotSamples_ForUnknownSlot_ThrowsInvalidOperationException` | Exception |
| `WithSlotValues_ForUnknownSlotType_ThrowsInvalidOperationException` | Exception |

### `InteractionModelBuilderTests` additions

| Test | Type |
|---|---|
| `BuildLocalized_WithLocale_ReturnsLocalizedModel` | Assertion |
| `BuildLocalized_WithoutLocale_ThrowsInvalidOperationException` | Exception |

### `AlexaInteractionModelClientTests` additions

| Test | Type |
|---|---|
| `UpdateAsync_WithLocalizedModel_CallsCorrectEndpoint` | Mock assertion |
| `UpdateAllAsync_WithMultipleModels_CallsEndpointForEach` | Mock assertion |
| `UpdateAllAsync_WithEmptyModels_CompletesWithoutCalling` | Mock assertion |
| `UpdateAllAsync_WhenSomeLocalesFail_ThrowsAggregateExceptionAfterAttemptingAll` | Exception + mock assertion |

---

## Implementation Order

1. `LocalizedInteractionModel.cs` — no dependencies
2. `internal` additions to `IntentBuilder.cs` and `SlotBuilder.cs`
3. `LocaleOverrideBuilder.cs` — depends on #2
4. `MultiLocaleInteractionModelBuilder.cs` — depends on #3
5. `InteractionModelBuilder.cs` additions — depends on #1
6. `IAlexaInteractionModelClient.cs` additions — depends on #1
7. `AlexaInteractionModelClient.cs` implementations — depends on #6
8. Test files — depend on all of the above