# 0001: Native AOT Compatibility Architecture

## Status

Accepted

No prior ADR directory or numbering scheme existed in this repository (`docs/` contains `docs/plans/` for
plans and `docs/research/` for research, both using a `NNNN-title.md` convention but no MADR-style
status/decision-driver format). This ADR establishes `docs/adr/` using a MADR-like numbered format
(`NNNN-title.md`, starting at `0001`) for future architecture decisions.

Native AOT compatibility as a **product requirement** is already settled — that decision was made
before this document and is not revisited here. This ADR decides the **architecture, contracts,
scope, and validation bar** by which AlexaVoxCraft delivers it. It is grounded entirely in
`docs/research/2026-09-08-native-aot-compatibility-research.md` (the "research doc"), which is treated
as the evidentiary record; findings are cited by section (`§N`) rather than re-derived.

## Context

The research doc's findings (§1-§11), taken as ground truth:

- AlexaVoxCraft's entire JSON pipeline funnels through one process-wide `JsonSerializerOptions`
  (`AlexaJsonOptions.DefaultOptions`, `src/AlexaVoxCraft.Model/Serialization/AlexaJsonOptions.cs:50-77`)
  built on `AlexaTypeResolver : DefaultJsonTypeInfoResolver`
  (`src/AlexaVoxCraft.Model/Serialization/AlexaTypeResolver.cs:8`) — STJ's fully reflection-based
  resolver, unconditionally constructed (research §6, case 2: not gated behind
  `JsonSerializer.IsReflectionEnabledByDefault`, so the AOT/trim linker cannot eliminate it).
- At least three more independently-constructed, unwired, reflection-default `JsonSerializerOptions`
  instances exist outside `AlexaJsonOptions` entirely: `BaseClient.cs:36-41`,
  `AlexaSkillInvocationClient.cs:22`, `InteractionModelBuilder.cs:183`, `AccessTokenClient.cs:19`
  (research §3, §5).
- AlexaVoxCraft's polymorphism (`RequestConverter`, `BasePolymorphicConverter<T>` subclasses,
  `APLComponentConverter`) is already implemented as closed, compile-time-enumerable
  `typeof(X)`-literal dictionaries feeding the non-generic `JsonSerializer.Deserialize(json, Type,
  options)` overload — a shape a generated `JsonSerializerContext` services correctly without any
  rewrite (research §3, §6, §8). This is the finding that makes source generation tractable rather
  than a rewrite.
- `APLValueConverterFactory`/`APLValueCollectionConverterFactory` use
  `Activator.CreateInstance(typeof(APLValueConverter<>).MakeGenericType(valueType), ...)` — a confirmed
  AOT blocker (research §5) — over an exhaustively-enumerated, closed set of 48 + 30 package-owned
  closed generic instantiations (research §8), plus one pre-existing correctness bug: `APLValue<Component>`
  in `AlexaSwipeToAction.cs:179` and `AlexaTextListItem.cs:28` resolves to
  `System.ComponentModel.Component` via a stray `using System.ComponentModel;`, not the intended
  `APLComponent` (research §8, §14).
- `SkillMediator.Send` dispatches via
  `Activator.CreateInstance(typeof(RequestHandlerWrapperImpl<>).MakeGenericType(t))` where `t` is a
  runtime `Type` (`src/AlexaVoxCraft.MediatR/SkillMediator.cs:58-60`) — a distinct, non-JSON hazard
  the research doc treats as a hard product-level gate on the "Native AOT compatible" claim, separate
  from and in addition to the JSON work (research §1, §5, §13 step 8).
- Three genuinely open, consumer-owned-type surfaces exist and cannot be closed by any amount of
  library-side source generation: `JsonAttributeBag.Get<T>/Set<T>/TryGet<T>/GetRequired<T>`
  (research §3, §7), `BaseClient.Deserialize<TResult>`/`Serialize<T>` (research §3, §7, §9), and
  `AlexaSkillInvocationClient.InvokeAsync<TRequest,TResponse>` (research §9).
- `AlexaVoxCraft.Smapi` is not tooling-only: per product-owner clarification captured in the research
  doc (§9), its clients (`AlexaSkillInvocationClient`, `AlexaInteractionModelClient`) are invoked from
  application code running inside Lambda functions in real, published-package consumers — a usage
  pattern invisible from this repo's own `ProjectReference` graph.
- `docs/components/lambda-hosting.md:535-547` currently documents a "Bundle Size Optimization" section
  that enables `PublishTrimmed`/`TrimMode=copyused` but exempts `AlexaVoxCraft.Model` and
  `AlexaVoxCraft.Model.Apl` from trimming entirely via `TrimmerRootAssembly` — the closest existing
  written acknowledgment that these assemblies are not trim-safe today (research §4). This guidance has
  no `PublishAot` equivalent and does not help Native AOT at all.
- Confirmed current TFMs: `net8.0;net9.0;net10.0;net11.0` (`src/Directory.Build.props:5`, research §9)
  for every package except `AlexaVoxCraft.MediatR.Generators` (`netstandard2.0`, compile-time only, no
  runtime AOT relevance). All are within STJ's/the runtime's Native AOT support window.
- No prior AOT initiative exists anywhere in this repo's history (research §4): no
  `JsonSerializerContext`, `[JsonSerializable]`, `IsAotCompatible`, `IsTrimmable`, or `PublishAot`
  usage has ever appeared, on any branch.

## Decision Drivers

Carried forward directly from the task brief and cross-checked against the research doc's own stated
principles (research §1, §12, §13):

- Smallest architecture that fully satisfies the product requirement.
- Protect long-term public API quality; minimize breaking changes.
- Preserve JIT ergonomics where doing so doesn't compromise AOT correctness.
- Treat trimming/AOT as architecture constraints to design around, not warning-suppression exercises.
- Prefer compile-time/source-generated behavior over runtime discovery, but don't add source generation
  where explicit, hand-written code suffices.
- Don't redesign unrelated serialization/polymorphism behavior; preserve current Alexa wire semantics.
- Generated implementation details stay internal unless consumers genuinely need them.
- "Native AOT compatible" is an end-to-end product claim about a running consumer application, not
  merely "JSON source generation exists somewhere in the library."

## Decision

Adopt the research doc's **Alternative B extended with Alternative E** (research §12-§13) as the
architecture: package-owned, internal, source-generated `JsonSerializerContext` metadata for every
closed AlexaVoxCraft-owned model graph, composed into the existing shared options pipeline via
`JsonTypeInfoResolver.Combine`, with exactly one new small public extension point for consumer-owned
types, and a feature-switch-gated JIT-only reflection fallback that compiles out entirely under
`PublishAot`. Existing hand-rolled polymorphic converters are kept unchanged. The two APL generic
converter factories are rewritten internally to closed, compile-time dispatch. Mediator dispatch is
made AOT-safe as a hard completion gate, likely by extending the existing MediatR source generator.
Each subsection below is a binding part of the decision.

### Serialization Metadata Ownership

**AlexaVoxCraft-owned types.** Each package that owns a closed model graph owns one **internal**
`JsonSerializerContext`: at minimum `AlexaVoxCraft.Model`, `AlexaVoxCraft.Model.Apl`,
`AlexaVoxCraft.Model.InSkillPurchasing`, and `AlexaVoxCraft.Smapi`. These context classes are never
public. This is not a stylistic choice — it is the only shape the `<ProjectReference>` dependency
graph permits (research §7 "Context granularity and visibility"): `AlexaVoxCraft.Model` has zero
project references and cannot know about `Model.Apl`/`Model.InSkillPurchasing` types; a single
monolithic context would require inverting a dependency or duplicating type declarations. The only
thing that needs to cross a package boundary is an already-realized `IJsonTypeInfoResolver` instance
(a context's `.Default` property), never the context class itself.

**Consumer-owned types.** AlexaVoxCraft cannot generate metadata for arbitrary consumer POCOs or
custom polymorphic types it has never seen. Exactly one new small API is added to the existing JSON
configuration surface (`AlexaJsonOptions`, which already exposes `RegisterConverter<T>` and
`RegisterTypeModifier<T>` — this reuses that idiom rather than inventing a new one):

```csharp
public static void RegisterTypeInfoResolver(IJsonTypeInfoResolver resolver);
```

No `RegisterContext<TContext>()` convenience overload is added. The research doc names it as a
plausible variant (§7, §10) but does not establish a need beyond "slightly more convenient syntax";
`RegisterTypeInfoResolver(IJsonTypeInfoResolver)` already accepts `MyContext.Default` directly with no
adapter step, so a second overload would be pure surface-area growth for a syntactic convenience, not
a new capability — decision-driver "smallest architecture" governs.

Consumers publishing Native AOT apps must register generated metadata for consumer-owned types that
cross AlexaVoxCraft boundaries: `JsonAttributeBag.Get<T>`/`Set<T>`, custom `IDirective` types
registered via `RegisterDirectiveDerivedType<T>`, custom request-resolver types registered via
`RegisterRequestTypeResolver<T>`, and open generic `TResult`/request-body types passed to `BaseClient`-
or `Smapi`-derived clients. Representative usage:

```csharp
[JsonSerializable(typeof(GameState))]
[JsonSerializable(typeof(MyDirective))]
internal partial class SkillJsonContext : JsonSerializerContext;
```

```csharp
AlexaJsonOptions.RegisterTypeInfoResolver(SkillJsonContext.Default);
```

No AlexaVoxCraft-specific source generator is introduced for consumer serialization metadata. STJ's
own `JsonSerializerContext` generator is the correct, sufficient tool (research §7, "Models
considered" table, last row) — a second generator here would be redundant ceremony (a package
reference plus a generator) for something STJ already does for free once a consumer declares one
`[JsonSerializable]` partial class.

### Resolver Composition and JIT Compatibility

The default resolver chain, built inside `AlexaJsonOptions.CreateOptions()`, has an explicit,
fixed order (research §6, §13 step 2-3):

1. AlexaVoxCraft-generated metadata (the package's own internal `JsonSerializerContext.Default`,
   e.g. `ModelContext.Default`).
2. Package-specific generated metadata registered through each package's own existing bootstrap/config
   path (`AplModelContext.Default` via `AplSupport.Add()`, `InSkillPurchasingModelContext.Default` via
   `InSkillPurchasingSupport.Add()`, `SmapiModelContext.Default` via the existing SMAPI DI extensions).
3. Consumer-provided generated resolvers, appended via `AlexaJsonOptions.RegisterTypeInfoResolver(...)`.
4. A gated JIT-only reflection fallback (see below), appended last.

Composition uses `JsonTypeInfoResolver.Combine(...)`. Combine queries resolvers in declared order and
returns the first non-null `JsonTypeInfo` — placing the reflection fallback last guarantees it is only
ever consulted for types no generated metadata covers, and can never shadow or diverge from the
library's own generated shape (research §12, Alternative E).

This precedence is an **invariant of `AlexaJsonOptions.CreateOptions()` itself, not an artifact of call
order**. Steps 1-2 (library- and package-owned resolvers) are always placed ahead of step 3 (consumer
resolvers registered via `RegisterTypeInfoResolver(...)`) regardless of the order in which package
bootstrap methods (`AplSupport.Add()`, `InSkillPurchasingSupport.Add()`, SMAPI's DI extensions) and
`RegisterTypeInfoResolver(...)` happen to be called at consumer startup. `RegisterTypeInfoResolver(...)`
is the one public consumer extension point; package-owned resolver registration is an internal
concern of each package's own bootstrap path and must never be allowed to share ordering semantics
with, or be reordered relative to, consumer registrations based on initialization timing. This ADR does
not prescribe the internal data structure that enforces the invariant (e.g. separate internal/consumer
resolver lists composed in a fixed order at options-build time) — only that call-order independence is
a required property of the implementation.

The existing `AlexaJsonOptions.RegisterTypeModifier<T>` semantics are preserved by applying every
registered modifier — including the built-in `ShouldSerialize` conditional-property logic that
`AlexaTypeResolver` performs today for `ResponseBody.Directives`, `Reprompt.Directives`, and
`ImageSource.Width`/`Height` — to the **outermost combined resolver** via
`JsonTypeInfoResolver.Combine(...).WithAddedModifier(...)`, not to any individual inner context. This
is required, not a style preference: a modifier applied to one inner context in the chain only fires
for `JsonTypeInfo` produced by that specific context, whereas today's semantics are "one global
modifier list, filtered by `ti.Type == typeof(T)` inside the delegate, regardless of which resolver in
the chain produced the type" (research §6, confirmed via `WithAddedModifier`'s documented behavior).
Applying modifiers outermost is the only construction that reproduces this exactly.

Existing cache/version-invalidation semantics are unchanged: `AlexaJsonOptions`'s version-counter cache
(`_version`/`_cachedVersion`) already rebuilds `DefaultOptions` lazily on next access after any
`RegisterConverter`/`RegisterTypeModifier`/`RegisterTypeInfoResolver` call, with no "freeze on first
use." Registrations before or after the first `DefaultOptions` access continue to behave as they do
today (research §6, confirmed from the existing implementation, not assumed).

### JIT Compatibility Behavior

A gated JIT-only reflection fallback is an **intentional, permanent part of this architecture**, not a
transitional or "merely considered" measure (research §12, Alternative E — confirmed, not left open):

```csharp
JsonSerializer.IsReflectionEnabledByDefault
    ? new DefaultJsonTypeInfoResolver()
    : null
```

appended as the final resolver in the chain, filtered out when `null`. Consequences, stated explicitly
as part of the decision:

- Ordinary JIT consumers retain today's zero-registration behavior for arbitrary POCOs (e.g.
  `JsonAttributeBag.Get<MyGameState>()` with no `JsonSerializerContext` ever declared) wherever
  reflection is available — no code change required of the large existing JIT-only consumer base.
- AlexaVoxCraft-owned generated metadata takes precedence over reflection (resolver ordering, above).
- Consumer-generated metadata takes precedence over reflection (resolver ordering, above).
- `PublishTrimmed`/`PublishAot` builds do **not** get the reflection fallback. `JsonSerializer.
  IsReflectionEnabledByDefault` is a link-time constant the trimmer/AOT compiler can prove statically
  (mirroring `RuntimeFeature.IsDynamicCodeSupported`); under `PublishAot` the switch is forced `false`
  at publish time and the linker removes the `new DefaultJsonTypeInfoResolver()` branch entirely —
  not merely marks it unreachable at runtime, but omits it from the published binary, with no
  `EnableAotAnalyzer` warning (research §6, item 3 — verified against current Microsoft guidance, not
  assumed).
- Missing consumer metadata under Native AOT must fail clearly — an immediate, comprehensible STJ
  `InvalidOperationException`/`NotSupportedException` at the point of use — not silently fall back to
  anything.
- The goal is **no unsupported reflection on supported Native AOT paths**, not ideological elimination
  of reflection from JIT builds. A JIT consumer who never publishes AOT sees zero behavioral change.

The current `AlexaTypeResolver : DefaultJsonTypeInfoResolver` architecture — an unconditional,
always-constructed reflection resolver with no feature-switch gate — is **replaced**, not retained
alongside the new design. It is precisely the shape the linker cannot eliminate (research §6, case 2),
which is why it is a hard AOT blocker today.

### Package Context Ownership

| Package | Owns internal context for | Bootstrap ride-along |
|---|---|---|
| `AlexaVoxCraft.Model` | `Request`/`Directive`/`Card`/`OutputSpeech`/etc. base types | Unconditional base of `AlexaJsonOptions.CreateOptions()` — same assembly, no new wiring |
| `AlexaVoxCraft.Model.Apl` | APL component/command/document/data-source types | `AplSupport.Add()` gains one line: `AlexaJsonOptions.RegisterTypeInfoResolver(AplModelContext.Default)` |
| `AlexaVoxCraft.Model.InSkillPurchasing` | ISP models | `InSkillPurchasingSupport.Add()` gains the identical one line |
| `AlexaVoxCraft.Smapi` | `InteractionModelDefinition` and the closed interaction-model/skill-management type family | Existing `AddSmapiDeveloperClient(...)`/`AddSkillInvocationClient(...)` DI extensions; `InteractionModelBuilder.ToJson(...)`'s existing optional-`options` parameter |

No package that already has a mandatory startup/config call gets a new bootstrap method invented for
this. `AplSupport.Add()` and `InSkillPurchasingSupport.Add()` already exist and are already mandatory
for consumers of those packages; each gains exactly one line. This is not a new pattern applied
uniformly ("every package gets an `X.Add()`") — it is "find whatever existing required call already
exists for that package and extend it" (research §7), which for `Http`/`InSkillPurchasing`(runtime)/
`Smapi` means a constructor default or an existing DI extension, not a new static bootstrap class (see
Runtime Package Scope below).

For SMAPI specifically: `SmapiModelContext.Default` must be available to **every** SMAPI default
serialization path independently of whether `AddSmapiDeveloperClient(...)`/`AddSkillInvocationClient(...)`
happened to run first. In particular, `InteractionModelBuilder.ToJson(...)` used directly with its
default `options` parameter must remain AOT-safe on its own — its correctness cannot depend on an
unrelated DI container having been configured. The existing DI registration methods may still
*participate* in configuring `AlexaJsonOptions` for the rest of the app, but the package's own default
options (wherever `BaseClient`, `InteractionModelBuilder`, and the SMAPI clients source their
`JsonSerializerOptions` today) must carry `SmapiModelContext.Default` intrinsically — e.g. baked into
the type's own static default-options construction — rather than relying on global initialization
order. No new public bootstrap API is added for this; it is a correctness property of SMAPI's existing
default-options construction.

### Consumer Extension Contract

Three consumer-owned-type surfaces get the identical contract — register a generated
`JsonSerializerContext` and append it via `AlexaJsonOptions.RegisterTypeInfoResolver(...)` once at
startup — because all three ultimately route through the same shared `JsonSerializerOptions.
TypeInfoResolver` chain via the non-generic `Deserialize(json, Type, options)`/generic
`Deserialize<T>(...)` overloads (research §7, confirmed from each call site, not assumed):

1. `JsonAttributeBag.Get<T>`/`Set<T>`/`TryGet<T>`/`GetRequired<T>` — arbitrary consumer session/
   persistence POCOs.
2. Custom `IDirective` types registered via `DirectiveConverter.RegisterDirectiveDerivedType<T>(key)`,
   and custom request-resolver types registered via
   `RequestConverter.RegisterRequestTypeResolver<TResolver>()`.
3. Open generic `TResult`/request-body types flowing through `BaseClient.Deserialize<TResult>`/
   `Serialize<T>` and `AlexaSkillInvocationClient.InvokeAsync<TRequest,TResponse>`.

One registration call covers all three simultaneously — there is no separate metadata-wiring step per
API, because they share the resolver chain, not because they are the same API.

### Polymorphism

Keep the existing hand-written polymorphic converters exactly as they are: `RequestConverter`,
`DirectiveConverter`, every `BasePolymorphicConverter<T>` subclass, `APLComponentConverter`, and their
existing discriminator/wire behavior. **Do not** migrate to `[JsonPolymorphic]`/`[JsonDerivedType]`.

Rationale (research §6, §8, explicitly settled, not merely deferred): these converters are already
structurally AOT-compatible once the concrete types they resolve to have generated metadata backing
them — the blocker was never the converters, it was the reflection-based base resolver underneath
them. `[JsonDerivedType]` requires either a single STJ-controlled discriminator placement or nested-
type wrapping, either of which risks subtly changing output byte-shape versus the current hand-rolled
`Write` (which drives serialization entirely from the runtime type's own `JsonTypeInfo`, with each
concrete type serializing its own `type` property as an ordinary member). There is no demonstrated
consumer need for STJ's built-in polymorphism attributes; migrating carries only wire-format risk with
no offsetting AOT or product benefit.

`DirectiveConverter.RegisterDirectiveDerivedType<T>()` is a real, actively-used public extension seam
(8 call sites across 3 packages, XML-documented) and **must remain supported**, under the same
consumer-metadata contract as any other consumer-owned type crossing the boundary.

`RequestConverter.RegisterRequestTypeResolver<T>()` is public but undocumented and has no confirmed
external caller (research §8). It is preserved technically under the identical Native AOT contract —
because the underlying dispatch mechanism is structurally identical to `RegisterDirectiveDerivedType<T>`
— but its documentation prominence is **not** elevated solely because of this migration. Its
current obscurity is a separate, pre-existing product/API-surface fact this ADR does not change.

### APL Generic Converter Strategy

Keep `APLValueConverterFactory` and `APLValueCollectionConverterFactory` as `JsonConverterFactory`
implementations, attached via `[JsonConverter(typeof(...Factory))]` on the open generic type
definitions `APLValue<T>`/`APLValueCollection<T>` exactly as today, so STJ continues to invoke them
automatically for every property with zero per-property wiring.

Replace `MakeGenericType`/`Activator.CreateInstance` for AlexaVoxCraft-owned types with **centralized,
compile-time-closed dispatch inside those two factory classes only** — e.g. a
`valueType == typeof(X)` chain, or a `Dictionary<Type, Func<JsonConverter>>` built once (the same shape
the factory already uses today for its `APLValue<object>` special case). This ADR decides the
boundary — where the knowledge lives — not the literal if/dictionary implementation choice, which is
an implementation detail for a future change, not an architectural one.

Properties this dispatch must satisfy (research §8, "three designs compared"):

- All 48 (`APLValue<T>`) + 30 (`APLValueCollection<T>`) library-owned closed generic instantiations are
  statically visible to the Native AOT compiler.
- Converter-selection knowledge is centralized in the two factory classes only — no per-property
  `CustomConverter` registration scattered across the ~90 model files that declare `APLValue<T>`/
  `APLValueCollection<T>` properties, and no second source generator emitting the same table
  mechanically. Both alternatives were compared and rejected on maintainability grounds (research §8):
  per-property registration scatters "which CLR type does property X need" across ~90 unrelated files
  with no compiler enforcement if forgotten; a second generator adds a permanent maintenance surface
  (a second Roslyn project, its own test suite) for a table that is small (78 entries), closed, and
  changes rarely — not justified by the evidence.
- `APLValue<object>`'s existing special case (`_aplObjectType`/`APLObjectConverter`) may remain as-is;
  the new dispatch is a direct extension of the same pattern, not a new one.
- Nested (`APLValueCollection<APLValue<int?>>`), nullable, and enum closed generic shapes must resolve
  correctly through the dispatch table (each is one additional entry; nested types resolve recursively
  through the two factories exactly as today).
- The confirmed-unreachable `APLEnumerableValueConverter<TValue,TList>` branch (research §5, §8: zero
  shipped properties reach it — every real enumerable-shaped `APLValue<T>` instead uses an explicit
  `CustomConverter = new GenericSingleOrListConverter<X>(...)` per property) is dropped from the closed
  table. A JIT-only compatibility escape hatch may remain for a consumer who constructs an
  `APLValue<T>`/`APLValueCollection<T>` instantiation the library doesn't itself ship (e.g. via
  `JsonAttributeBag`) — the architectural invariant is that any such fallback is never reachable/rooted
  on the supported Native AOT path. This ADR does not prescribe the specific implementation technique
  (e.g. whether it is gated via `JsonSerializer.IsReflectionEnabledByDefault`, annotated with
  `[RequiresDynamicCode]`/`[RequiresUnreferencedCode]`, or some other mechanism achieving the same
  invariant) — those are plausible techniques to select once the factory code is actually changed and
  analyzer behavior can be observed, not a decision to lock in ahead of that.

**Pre-existing bug, sequencing dependency only.** `APLValue<Component>` in `AlexaSwipeToAction.cs:179`
and `AlexaTextListItem.cs:28` resolves to `System.ComponentModel.Component` (a stray
`using System.ComponentModel;`), not the intended `APLComponent`. This is a **separate, pre-existing
correctness bug**, not part of the AOT architecture itself, and this ADR does not decide how to fix it.
It is recorded here only as a sequencing dependency: the bug must be fixed before the closed
`APLValue<T>`/`APLValueCollection<T>` type inventory backing the dispatch tables above is finalized, so
the architecture is built against the intended type (`APLComponent`), not the accidental one
(`System.ComponentModel.Component`) — building the dispatch table first would permanently encode the
bug as if it were a real requirement, turning a one-line pre-migration fix into a later breaking change
to the generated/hand-written type inventory.

### Mediator Dispatch

Native AOT cannot be claimed while `Activator.CreateInstance(typeof(RequestHandlerWrapperImpl<>).
MakeGenericType(runtimeType))` (`SkillMediator.cs:58-60`) remains reachable on the supported Native AOT
path. This is a **hard completion gate**, not an optional follow-on, and it is independent of the JSON
work — fixing `AlexaJsonOptions` alone does not touch it (research §1, §5, §13 step 8).

Direction: extend the existing MediatR DI source generator (`AlexaVoxCraft.MediatR.Generators`), which
already discovers every registered `IRequestHandler<T>` at compile time for DI registration, to
additionally emit a compile-time mapping from `Type` to a non-reflective wrapper/factory delegate, so
runtime dispatch no longer needs `MakeGenericType`/`Activator.CreateInstance`. This is the one place in
this architecture where extending the existing generator is the right tool — a distinction worth
holding precisely against the APL factory decision above, where a second generator was rejected for the
opposite reason (small, closed, rarely-changing table; here the generator already exists, already
discovers the exact type set needed, and already solves the structurally identical DI-registration
problem). The precise shape of the generated mapping is an implementation detail for a future
plan, not this ADR.

The reflection-based `ServiceRegistrar` fallback used when the interceptor generator is explicitly
disabled (`EnableMediatRGeneratorInterceptor=false`) or on `.NET SDK < 8.0.400` may remain a documented,
JIT-only/non-AOT mode. A consumer who opts out of source-generated registration cannot expect
AlexaVoxCraft to claim Native AOT support for that configuration — this is a scoped, documented
carve-out, not a gap in the architecture.

### Runtime Package Scope

AlexaVoxCraft packages that can legitimately participate in runtime application code are expected to
support Native AOT: `AlexaVoxCraft.Model`, `AlexaVoxCraft.Model.Apl`, `AlexaVoxCraft.Model.
InSkillPurchasing`, `AlexaVoxCraft.MediatR`, `AlexaVoxCraft.MediatR.Lambda`, `AlexaVoxCraft.Lambda`,
`AlexaVoxCraft.Http`, `AlexaVoxCraft.InSkillPurchasing`, `AlexaVoxCraft.Smapi`.

`AlexaVoxCraft.Smapi` is explicitly **not** tooling-only. Per product-owner clarification captured in
the research doc (§9), SMAPI clients are used from application code running inside Lambda functions in
real, published-package consumers — invisible from this repo's own internal `ProjectReference` graph
because it is a published-package consumption pattern, not an in-repo one. Dependency-graph invisibility
from this repo is evidence only that this repo's own samples don't exercise the path, not evidence of
tooling-only status.

Concretely in scope for the resolver-chain treatment: `BaseClient`'s orphaned default
`JsonSerializerOptions` (`BaseClient.cs:36-44`), and `Smapi`'s three independent ad hoc options
instances (`AlexaSkillInvocationClient.cs:20-28`, `AlexaInteractionModelClient.cs:17-18` via
`BaseClient`'s default, `InteractionModelBuilder.cs:177-190`).

`AlexaVoxCraft.Observability` and `AlexaVoxCraft.MinimalLambda` were **not** deeply audited by the
research doc (§9, §14). This ADR does not invent architecture for unfound issues in either package.
Their compatibility must be independently verified — their own JSON/reflection surfaces audited using
the same method as this ADR's other packages — before either is included in any package-level
`IsAotCompatible` claim. Until verified, they are out of scope for the initial Native AOT claim.

No `AddAot()`/`UseAot()`/other AOT-specific consumer configuration API is added anywhere. Native AOT is
a supported **deployment model**, not a separate **programming model** — every package rides its AOT-
safe defaults in on whatever existing bootstrap/constructor/DI-registration path it already has (Package
Context Ownership, above); there is no ceremony a consumer performs specifically because they intend to
publish AOT, beyond registering metadata for their own consumer-owned types (Consumer Extension
Contract, above) — and even that requirement is publish-mode-dependent, not universal:

- **Native AOT consumers** must provide generated metadata (a `JsonSerializerContext` registered via
  `RegisterTypeInfoResolver(...)`) for any consumer-owned type crossing an AlexaVoxCraft serialization
  boundary (Consumer Extension Contract, above) — reflection is compiled out under `PublishAot` (JIT
  Compatibility Behavior, above), so there is no fallback to rely on.
- **JIT consumers** may continue relying on the gated reflection fallback for consumer-owned types when
  `JsonSerializer.IsReflectionEnabledByDefault` is enabled (the default) — no registration is required
  of them, matching today's behavior exactly.

This is intentionally one of the accepted behavioral differences between the JIT and Native AOT modes
under this ADR (JIT Compatibility Behavior, above), not an oversight: a consumer's obligation to
register metadata for their own types is a Native-AOT-specific requirement, not a universal one imposed
"regardless of publish mode."

### Native AOT Compatibility Definition

`<IsAotCompatible>true</IsAotCompatible>` is **not** the mechanism that creates compatibility — it is
the declaration earned *after* compatibility is demonstrated, never a substitute for demonstrating it.
Setting it prematurely does not correct any of this ADR's architectural decisions above; it only
mislabels a package.

A package earns `IsAotCompatible=true` only after all of the following (research §11):

- Clean trim analysis (`EnableTrimAnalyzer`).
- Clean AOT analysis for supported paths (`EnableAotAnalyzer`).
- Clean single-file analysis (implied by `IsAotCompatible=true`).
- A package-specific reference-compatibility review of its own external dependencies.
- Real Native AOT consumer validation (see Validation / CI Requirements below) — not merely a
  successful `dotnet publish -p:PublishAot=true` build.
- Execution of the resulting published native binary, with output verified against expectations.

`VerifyReferenceAotCompatibility` (the analyzer property that requires every referenced assembly to
itself carry `IsAotCompatible` metadata, surfacing `IL3058` otherwise) is used as an **investigative
and hardening mechanism** — turned on to empirically discover which external dependencies are noisy —
not as an unconditional gate. Its initial cleanliness across every external dependency (AWS Lambda
assemblies, Serilog packages, `LayeredCraft.Logging.CompactJsonFormatter`) is **not** required if those
dependencies function correctly under Native AOT despite lacking corresponding `IsAotCompatible`
assembly metadata (research §11, item 7). No blanket suppressions are used to manufacture a clean
claim in either direction — not to silence a real finding, and not to force a premature "compatible"
declaration.

### Validation / CI Requirements

Native AOT compatibility is defined and validated in terms of a **real, rooted consumer application**,
never library compilation alone (research §11). The validation application must:

- Use AlexaVoxCraft exclusively through its normal public APIs (no test-only internal hooks).
- Disable reflection-based STJ defaults (`JsonSerializerIsReflectionEnabledByDefault=false`, forcing the
  exact failure mode a real AOT-published consumer would hit).
- Deserialize realistic Alexa request payloads (reusing the existing CloudWatch-derived fixtures under
  `test/AlexaVoxCraft.Model.Tests/Examples` / `.../Snapshots` and the APL equivalents — at minimum a
  `LaunchRequest`, a slot-bearing `IntentRequest`, a `SessionEndedRequest`, and an APL `UserEventRequest`).
- Execute actual mediator handler dispatch (`SkillMediator.Send`), exercising the post-fix dispatch path
  for real, not merely asserting the fix compiles.
- Serialize Alexa responses, including at least one containing APL directives (the largest, most
  reflection-dependent surface via `AplSupport.Add()`'s per-type registrations).
- Exercise APL end to end.
- Exercise consumer-owned session/persistence POCO metadata through `JsonAttributeBag`, demonstrating
  the consumer resolver-registration contract working for a genuine custom type.
- Exercise ISP and `Http` serialization.
- Exercise representative SMAPI runtime behavior: both the package-owned metadata path
  (`InteractionModelDefinition` via `AlexaInteractionModelClient`) and the open generic consumer-owned
  request/response scenario (`AlexaSkillInvocationClient.InvokeAsync<TRequest,TResponse>`).
- Demonstrate consumer resolver composition explicitly (a consumer `JsonSerializerContext` registered
  via `RegisterTypeInfoResolver` actually taking effect).
- Demonstrate a clear, comprehensible failure when consumer metadata is omitted under an AOT build (not
  a silent fallback, not a generic crash).
- Actually `dotnet publish -p:PublishAot=true` and **execute the resulting native binary** — not merely
  confirm the publish step exits `0`.
- Verify correct runtime behavior and output from that execution.

The existing unit test suite is **not** AOT-compiled wholesale — that is unnecessary ceremony. The
existing Verify/Compono model tests plus the CloudWatch-derived production fixtures remain the wire-
semantic regression safety net (research §3, §11); the AOT validation app is a separate, additional
artifact, not a replacement test-running mode for the existing suite.

**Wire-semantic compatibility** (a payload produced before the migration deserializes to an equivalent
object graph after it, and vice versa) is the required product guarantee. **Byte/property-order
identity** (literal byte-for-byte equality of serialized output, including property emission order) is
a useful regression-detection signal — reusing the 207 existing Verify snapshot files as an oracle for
unintended shape drift — but it is **not** a newly-created public contract; AlexaVoxCraft has never
publicly promised stable JSON property ordering, and this migration does not begin promising it
(research §10).

**CI gate.** A durable CI job, separate from the existing `dotnet test`/Testing-Platform run, must:
build and test normally; run trim/AOT analyzers; publish the rooted Native AOT validation application;
execute the resulting native binary on at least one Linux RID matching the Lambda `provided.al2023`
runtime target (`linux-x64`/`linux-arm64`); fail the build on any functional incompatibility. This gate
prevents future regressions that reintroduce reflection or runtime-metadata dependency into supported
paths, and it must run on every PR touching `src/AlexaVoxCraft.Model*`, `src/AlexaVoxCraft.MediatR*`,
or any `JsonSerializerContext` implementation file. The Native AOT compatibility claim is understood to
require continuous verification after initial implementation, not a one-time badge.

### Backward Compatibility

- Existing public configuration APIs (`AlexaJsonOptions.RegisterConverter<T>`, `RegisterTypeModifier<T>`)
  keep their exact signatures and semantics; only what base resolver they compose against changes.
- Consumers who already supply their own explicit `JsonSerializerOptions` (`AlexaLambdaSerializer`'s
  constructor parameter, `BaseClient`'s constructor overload) own that configuration outright.
  AlexaVoxCraft's own default options are AOT-safe; a consumer's custom options are AOT-safe only if
  the consumer configures them with AOT-safe metadata themselves. AlexaVoxCraft must **not** silently
  mutate or replace an explicitly consumer-supplied `JsonSerializerOptions` merely to make it
  AOT-compatible — doing so would violate the consumer's explicit configuration choice silently.
- The only new public API surface across this entire architecture is
  `AlexaJsonOptions.RegisterTypeInfoResolver(IJsonTypeInfoResolver)`. No existing method signature
  changes.
- `docs/components/lambda-hosting.md`'s current `TrimmerRootAssembly` exemption guidance for
  `AlexaVoxCraft.Model`/`AlexaVoxCraft.Model.Apl` (lines 535-547) is superseded by this architecture —
  it exists today only because those assemblies are not trim-safe; once they are, the exemption is no
  longer needed and the documentation must be corrected (see Documentation Contract below).
- Precedent for a large, non-breaking reflection-elimination migration already exists in this codebase:
  the MediatR DI source generator changed `AddSkillMediator()` from always-reflection to intercepted-by-
  default with zero required consumer code changes, while keeping a documented opt-out
  (`EnableMediatRGeneratorInterceptor=false`) for JIT-only edge cases (research §10). This migration
  follows the identical shape: default to the new AOT-safe resolver, keep the reflection escape hatch
  documented and working for JIT-only consumers who need it.

## Consequences

**Positive.** AlexaVoxCraft becomes able to honestly claim Native AOT support for the packages a
consumer's Lambda-hosted skill actually exercises at runtime, without rewriting its polymorphism, its
wire format, or its existing public configuration surface. The fix is concentrated (one resolver
construction site per package, two converter-factory rewrites, one mediator-dispatch change) rather
than diffuse. JIT-only consumers — the overwhelming majority of today's consumer base — see zero
required code changes. Source-gen resolution is also faster than reflection-based resolution in the
general case, a secondary performance benefit.

**Negative / cost.** Every closed AlexaVoxCraft-owned type must be enumerated into a `[JsonSerializable]`
list per package (potentially 150+ types across `Model` + `Model.Apl`); forgetting one produces a clear
STJ error at first use, not a silent gap, but it is still a new category of thing to remember when
adding a model type. The mediator-dispatch generator extension and the two APL factory rewrites are
each nontrivial, self-contained engineering efforts with their own risk surfaces, gated by the same
validation bar as the JSON work. `AlexaVoxCraft.Smapi` and `AlexaVoxCraft.Http` — previously reasonable
to treat as build/deploy-time-only — now carry the same AOT-correctness obligation as the core request/
response path, widening the surface this ADR's completion criteria apply to.

## Alternatives Considered

1. **Continue reflection-based serialization; document Native AOT as unsupported.** Zero
   implementation cost, zero risk, zero product benefit — does not satisfy the settled requirement.
   Rejected as the end state; useful only as the baseline the other alternatives are judged against
   (research §12, Alternative A).
2. **Warning/linker-annotation workarounds** (`[RequiresDynamicCode]`/`[RequiresUnreferencedCode]`
   propagated through the public API) instead of an architectural change. Improves the failure mode
   from a confusing runtime exception to a build-time warning but still does not make anything
   AOT-*compatible* — it treats the symptom, not the architecture, and is explicitly the kind of
   warning-suppression exercise this decision's drivers reject. Rejected as an end state; acknowledged
   as a plausible short-term interim step, not adopted as part of this architecture (research §12,
   Alternative C).
3. **Require `JsonTypeInfo<T>` at every serialization call site**, eliminating the shared
   `AlexaJsonOptions.DefaultOptions` singleton entirely. Maximizes AOT correctness but is a breaking
   change to every existing `Get<T>`/`Deserialize<T>` call across every consumer skill, for a
   correctness guarantee the resolver-chain design already provides at a fraction of the ceremony.
   Rejected — optimizes only for AOT users at ordinary-consumer expense (research §12, Alternative D).
4. **Force consumers to replace AlexaVoxCraft's entire `JsonSerializerOptions`.** Correct only if the
   consumer manually re-registers every AlexaVoxCraft type and every `RegisterTypeModifier` side
   effect — fragile, easy to silently break directive/APL serialization shape, and defeats the purpose
   of a shared library default. Rejected (research §7, "Models considered" table).
5. **Expose AlexaVoxCraft's generated `JsonSerializerContext` classes publicly.** Nothing about the
   resolver-chain design requires it — only the already-realized `IJsonTypeInfoResolver` instance needs
   to cross a boundary. Publicizing them would grow AlexaVoxCraft's public surface and its generated-
   code footprint in consumer IntelliSense/docs for no benefit, and would remove the freedom to
   restructure or split/merge contexts later without a breaking change. Rejected (research §7).
6. **One monolithic cross-package context.** Architecturally impossible without inverting the
   `Model → Model.Apl`/`Model.InSkillPurchasing` dependency direction or duplicating type declarations
   across contexts (research §7). Rejected on dependency-graph grounds, not preference.
7. **Rewrite polymorphism with `[JsonPolymorphic]`/`[JsonDerivedType]`.** No demonstrated consumer need;
   risks subtly changing wire-format byte-shape for no offsetting AOT benefit, since the existing
   hand-rolled converters are already AOT-legitimate once the base resolver changes. Rejected (research
   §6, §8).
8. **Per-property `CustomConverter` registration across ~90 APL model files** (the prior research pass's
   recommendation) to eliminate the two factories' `MakeGenericType` hazard. Technically sufficient, but
   scatters "which CLR type does property X need" across ~90 unrelated files by property-name string,
   with no compiler enforcement if a future property is added and forgotten. Rejected in favor of
   centralized dispatch inside the two factories (research §8, design (a) vs (b)).
9. **A new APL-specific source generator to emit the closed converter dispatch mechanically.** Removes
   the small residual risk of a hand-forgotten dispatch entry, but adds a second permanent Roslyn
   generator project (its own SDK-version maintenance burden, its own test suite) for a table that is
   small (78 entries), closed, and changes only when a human deliberately adds a new APL type — an event
   that already requires touching that type's own file. Not justified by the evidence. Rejected (research
   §8, design (b) vs (c)).
10. **Remove all reflection from JIT builds** instead of retaining the gated compatibility fallback.
    Would break today's zero-registration `JsonAttributeBag`/`BaseClient` ergonomics for the large JIT-
    only consumer population for no Native AOT benefit (the reflection fallback already compiles out of
    AOT builds entirely). Rejected — the goal is no *unsupported* reflection on the Native AOT path, not
    reflection elimination as an end in itself (research §12, Alternative E "verdict").
11. **Treat `AlexaVoxCraft.Smapi` as tooling-only** and exclude it from the runtime AOT scope. Was the
    prior research pass's conclusion, based on this repo's internal `ProjectReference` graph showing no
    in-repo SMAPI consumer. Overturned by explicit product-owner clarification that SMAPI clients are
    used from real Lambda application code in published-package consumers — dependency-graph invisibility
    from this repo is not evidence of tooling-only status for a published library (research §9).
    Rejected.

## Risks

- **Property-ordering/shape drift during migration.** STJ source-gen's default member ordering can
  differ subtly from `DefaultJsonTypeInfoResolver`'s reflection-based ordering. Mitigated by the
  existing 207 Verify snapshots acting as a regression oracle, but every diff must be triaged as
  wire-semantic (must fix) versus cosmetic re-recording (expected) — this triage has a real chance of
  being done sloppily under time pressure and accidentally masking a genuine regression as "expected."
- **Mediator dispatch generator extension is the least-precedented piece of this architecture.** Unlike
  the JSON work (which has a documented, already-proven STJ pattern) and the APL factory rewrite (a
  contained, mechanical two-file change), extending the DI source generator to also emit dispatch
  wrappers is new engineering territory for this codebase's generator, with correctness dependent on a
  real `PublishAot` execution, not just a clean build (research §5: canonical/shared generic support for
  reference-type generics under Native AOT "often works" but is explicitly flagged as needing real
  validation, not assumed correctness).
- **External dependency AOT status is unverified and outside AlexaVoxCraft's control.** AWS Lambda
  assemblies, Serilog packages, and `LayeredCraft.Logging.CompactJsonFormatter` may or may not carry
  `IsAotCompatible` metadata; if any genuinely prevents a supported runtime package from working under
  Native AOT (not merely triggers an `IL3058` warning), that is a real external blocker requiring
  resolution — potentially outside this project's control on its own timeline — before that specific
  package can honestly get `IsAotCompatible=true`.
- **Scope creep risk on `Http`/`InSkillPurchasing`/`Smapi`.** These three packages were only brought
  into scope by an explicit, late product-owner clarification and were less thoroughly audited than
  `Model`/`Model.Apl` in the research pass (three JSON-touching call sites enumerated for `Smapi`, per
  research §9, versus the much deeper audit given to `Model`). There is real risk of under-scoping the
  actual remediation effort for these three packages relative to the confidence this ADR expresses about
  `Model`/`Model.Apl`.
- **`AlexaVoxCraft.Observability`/`AlexaVoxCraft.MinimalLambda` are unknowns, not confirmed-safe.**
  Treating them as "out of scope for now" is correct given the evidence, but it means the true Native
  AOT surface of the full package set is not yet fully known until they are audited.

## Deferred / Validation-Time Questions

Carried forward verbatim in substance from the research doc's genuinely-unresolved items (§14) — these
require empirical testing during implementation/validation, not further architectural decision-making,
and are explicitly not blockers on this ADR:

- Whether AWS's `provided.al2023` custom runtime plus `Amazon.Lambda.RuntimeSupport`/`Amazon.Lambda.
  Serialization.SystemTextJson`/`Amazon.Lambda.Core` (the exact pinned versions) impose any additional
  AOT constraints beyond what's captured here — answerable empirically via
  `VerifyReferenceAotCompatibility` once `IsAotCompatible=true` is set on `AlexaVoxCraft.MediatR.Lambda`.
- Serilog's and `LayeredCraft.Logging.CompactJsonFormatter`'s AOT-compatibility status (dependencies of
  `AlexaVoxCraft.MediatR.Lambda`) — not independently verified; answerable the same way.
- `AlexaVoxCraft.Observability` and `AlexaVoxCraft.MinimalLambda`'s own JSON/reflection surfaces — not
  audited at all; require the same audit method applied to the other packages before either can be
  included in an `IsAotCompatible` claim.
- Whether real-world consumers of `AlexaSkillInvocationClient.InvokeAsync<TRequest,TResponse>` typically
  pass AlexaVoxCraft-owned `SkillRequest`/`SkillResponse` types (already covered by the `Model` context,
  no extra consumer action needed) or fully custom types (requiring a consumer-supplied context) — this
  affects how prominently the open-`T` caveat needs to be documented for this specific API, not the
  architecture itself.

## Completion Criteria

Native AOT support is not complete — and the "Native AOT compatible" label must not be used — until all
of the following hold simultaneously:

- Package-owned JSON metadata is source-generated for `Model`, `Model.Apl`, `Model.InSkillPurchasing`,
  and `Smapi`.
- Consumer-owned metadata has a supported, documented resolver-registration contract
  (`RegisterTypeInfoResolver`).
- The JIT compatibility fallback is correctly feature-gated (`IsReflectionEnabledByDefault`-conditioned,
  confirmed absent from `PublishAot` output).
- Existing JSON modifiers and converters (`RegisterConverter<T>`, `RegisterTypeModifier<T>`, the
  `ShouldSerialize` conditional-property logic) preserve their current semantics exactly.
- Package polymorphism (`RequestConverter`, `BasePolymorphicConverter<T>` subclasses,
  `APLComponentConverter`, `RegisterDirectiveDerivedType<T>`) retains current wire behavior.
- The APL generic converter factories no longer require dynamic generic activation on the supported
  Native AOT path.
- The pre-existing `APLValue<Component>`/`System.ComponentModel.Component` bug is corrected **before**
  the final closed APL generic type map is established.
- Mediator runtime dispatch (`SkillMediator.Send`) no longer requires dynamic generic activation on the
  supported Native AOT path.
- `Http`, `InSkillPurchasing`, and `Smapi` runtime serialization is AOT-safe.
- Trim/AOT analyzer findings are resolved or legitimately classified (not suppressed to manufacture a
  clean claim).
- A real, rooted Native AOT consumer application publishes and executes correctly (Validation / CI
  Requirements, above).
- Consumer-owned metadata registration works end to end in that application.
- Missing consumer metadata fails clearly and immediately under Native AOT.
- Documentation is complete (Documentation Contract, below).
- CI continuously enforces the compatibility claim, not merely at the moment it was first made.
- `IsAotCompatible=true` is set only for packages whose compatibility has actually been demonstrated
  per the above — never speculatively, never uniformly across the whole solution as a shortcut.

## Documentation Contract

Documentation is part of completion, not a follow-on. A future implementation must update, as
applicable:

- README / getting-started material.
- Serialization documentation.
- Lambda hosting documentation, including `docs/components/lambda-hosting.md:535-547`'s current
  `TrimmerRootAssembly` exemption guidance — this must be corrected/removed once `Model`/`Model.Apl`
  are actually trim-safe, since the exemption exists today specifically because they are not.
- APL documentation.
- ISP documentation.
- SMAPI documentation.
- API reference for the new `RegisterTypeInfoResolver` extension point.
- Native-AOT-specific usage guidance for consumers (how to declare a consumer `JsonSerializerContext`,
  when it's required, what failure looks like if omitted).
- Migration guidance explaining the JIT-fallback-vs-AOT-requirement distinction in consumer-facing terms.
- Repo skills/agent instructions (e.g. this repo's `CLAUDE.md`) if they describe serialization or
  compatibility behavior that this architecture changes.
- If any repo skill carries an `evals.json` and that skill's guidance changes as a result of this work,
  the evals must be updated and compared against baseline — noted here as a **future-implementation
  reminder**, not an action taken by this ADR.
