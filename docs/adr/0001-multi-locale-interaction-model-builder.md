# [ADR-0001] Multi-Locale Interaction Model Builder with Default Locale Fallback

**Status:** Accepted

**Date:** 2026-03-25

**Decision Makers:** Nick Cipollina

## Context

The `AlexaVoxCraft.Smapi` package provides an `InteractionModelBuilder` for constructing Alexa interaction model definitions in code. The `AlexaInteractionModelClient` already accepts a `locale` parameter on its `GetAsync` and `UpdateAsync` methods, reflecting the SMAPI API's per-locale model structure.

However, the builder has no concept of locale. Callers building skills that support multiple locales (e.g., en-US, en-GB, en-CA) must construct entirely separate `InteractionModelBuilder` instances for each locale, duplicating all intent names, slot definitions, slot type names, and built-in intent registrations. Only sample utterances and invocation names legitimately differ between locales — yet everything must be repeated.

This creates noise, increases the chance of structural drift between locales, and makes multi-locale skill maintenance difficult.

## Decision Drivers

- Alexa skills commonly target multiple English locales (en-US, en-GB, en-CA, en-AU) with near-identical intent schemas but locale-specific invocation names and sample utterances
- Structural elements (intent names, slot names, slot type names, built-in intents) are always identical across locales
- The only genuinely locale-specific content is: invocation name, intent sample utterances, slot-level samples, and slot type values/synonyms
- Developer ergonomics: defining shared structure once and only overriding what differs is the familiar `.resx` resource file mental model
- Existing `InteractionModelBuilder` must remain backward compatible — no breaking changes

## Considered Options

1. **`MultiLocaleInteractionModelBuilder` with shared schema and per-locale overrides (default locale fallback)**
2. **Per-locale `InteractionModelBuilder` instances with no coordination**
3. **Single `InteractionModelBuilder` with a locale collection and locale-keyed sample dictionaries**

## Decision Outcome

Chosen option: **Option 1**, because it eliminates structural duplication, makes locale-specific content explicit and minimal, and follows the well-understood resource file fallback model (base culture → specific culture).

A `WithDefaultLocale` method establishes the base locale (included in output). `ForLocale` registers additional locales that inherit all unoverridden values from the default. `BuildAll()` merges and produces an `IReadOnlyList<LocalizedInteractionModel>` ready to push via `UpdateAllAsync`.

### Positive Consequences

- Intent names, slot definitions, slot types, and built-in intents are declared once
- Only invocation names and utterances need to be written per locale — the actual delta
- Adding a new locale that is identical to the default requires a single `.ForLocale("en-CA")` call with no lambda
- Validation of locale-specific content (unknown intent names, unknown slot types) fails fast at call time, not at build time
- `LocalizedInteractionModel` record cleanly pairs locale + definition for use with the client's new overloads
- Fully backward compatible — `InteractionModelBuilder.Build()` and `ToJson()` are unchanged
- `UpdateAllAsync` provides a natural batch push of all locales in a single call

### Negative Consequences

- The `MultiLocaleInteractionModelBuilder` is a larger surface area than the single builder — more types to understand (`LocaleOverrideBuilder`, `LocalizedInteractionModel`)
- Callers who need fully independent schemas per locale (e.g., completely different intent structures for different languages) must still use separate `InteractionModelBuilder` instances; the multi-locale builder assumes a shared schema
- `AddSlotType` on `MultiLocaleInteractionModelBuilder` makes `configure` optional (unlike the single builder where it is required), which is a subtle API inconsistency — justified because slot values are locale-specific and often not set at the schema level

## Pros and Cons of the Options

### Option 1: `MultiLocaleInteractionModelBuilder` with shared schema and default locale fallback

A new builder class where structural elements (intents, slot types) are defined once. A `WithDefaultLocale` call establishes base locale text. `ForLocale` calls register additional locales with only their overrides. `BuildAll()` merges per locale using fallback resolution: locale value → default value.

- Good, because structural elements are never duplicated
- Good, because the fallback model is familiar from .resx / resource file conventions
- Good, because `.ForLocale("en-CA")` with no lambda is valid and produces a model identical to the default
- Good, because validation fails at the call site (wrong intent name in `WithIntentSamples`) not at build time
- Good, because `UpdateAllAsync` enables batch push of all locales sequentially, respecting SMAPI rate limits
- Bad, because adds new types (`LocaleOverrideBuilder`, `LocalizedInteractionModel`, `MultiLocaleInteractionModelBuilder`)
- Bad, because assumes a shared schema — callers with truly divergent locale schemas must use a different approach

### Option 2: Per-locale `InteractionModelBuilder` instances with no coordination

Callers create independent `InteractionModelBuilder` instances per locale and push each separately.

- Good, because no new types required — callers already know `InteractionModelBuilder`
- Good, because locales are completely independent — no shared state surprises
- Bad, because every intent name, built-in intent, and slot definition must be repeated for each locale
- Bad, because structural drift between locales is easy and silent — a renamed intent in one locale is not reflected in others
- Bad, because pushing multiple locales requires manual iteration

### Option 3: Single `InteractionModelBuilder` with locale-keyed sample dictionaries

Extend the existing builder with methods like `WithIntentSamples("en-US", "OrderIntent", ...)` and a `BuildAll()` that emits one definition per registered locale.

- Good, because stays within a single builder type
- Bad, because mixes structural and locale-specific concerns in one object — the builder becomes large and harder to reason about
- Bad, because the locale string becomes a stringly-typed parameter on every call, increasing error surface
- Bad, because no clean concept of a "default" locale — every locale must fully specify its samples

## Links

- [Implementation Plan](../plans/multi-locale-interaction-model-builder-plan.md)
- [SMAPI Interaction Model API](https://developer.amazon.com/en-US/docs/alexa/smapi/interaction-model-schema.html)
- [Alexa Interaction Model Locale Support](https://developer.amazon.com/en-US/docs/alexa/custom-skills/develop-skills-in-multiple-languages.html)