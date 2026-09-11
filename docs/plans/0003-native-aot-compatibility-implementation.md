# Plan 0003: Native AOT compatibility implementation (ADR-0001)

## Goal

Implement the architecture decided in `docs/adr/0001-native-aot-compatibility.md` ("the ADR") so that
AlexaVoxCraft can honestly claim Native AOT support for the packages a consumer's Lambda-hosted skill
actually exercises at runtime, per the ADR's Completion Criteria. This plan does not reopen or
re-derive any architectural decision the ADR already made; it sequences the ADR's decisions into
concrete, file-level implementation work. Where this plan states a technical shape not already fixed
by the ADR (e.g. dictionary vs. if-chain dispatch, exact `[RequiresDynamicCode]` usage), it is flagged
explicitly as an **open decision for implementation time**, not settled here.

## Constraints

- **Single PR, single plan, no exceptions.** All work below — including Task Group 1's
  `APLValue<Component>` bugfix — ships in one pull request. Task groups establish implementation *order*
  and in-PR validation checkpoints only; a task group is never a PR boundary and must never be proposed,
  reviewed, split, or merged as one. Task Group 1 remains the first prerequisite task (it must complete,
  with its own tests passing, before Task Group 4's closed APL converter inventory is finalized), but
  "must complete first" is a sequencing requirement within the one PR, not a basis for a separate PR.
- Do not implement anything while writing or revising this plan document — this is planning only.
- Do not redesign anything the ADR already settled: polymorphism stays hand-rolled (no
  `[JsonPolymorphic]`/`[JsonDerivedType]`), no `RegisterContext<TContext>()` convenience overload, no new
  bootstrap classes where an existing mandatory call can be extended, no monolithic cross-package
  context, no forced replacement of consumer-supplied `JsonSerializerOptions`.
- Every task group must preserve current wire semantics (deserialization equivalence) and existing
  public API signatures except for the one new method the ADR authorizes:
  `AlexaJsonOptions.RegisterTypeInfoResolver(IJsonTypeInfoResolver)`.
- Existing Verify/Compono tests and the CloudWatch-derived fixtures (`docs/plans/0002-...md`) are the
  primary regression safety net; add new tests only where a task group's change isn't already covered.
- Every file path below was confirmed against the current tree during planning (2026-09-08); if a path
  has moved by the time a task group starts, re-verify with `grep`/`find` before editing — do not assume
  the plan's paths are still exact after other task groups land.

## Relationship to ADR-0001 and the research doc

This plan implements ADR-0001 exactly as accepted. Any file:line citation below that traces to the
research doc (`docs/research/2026-09-08-native-aot-compatibility-research.md`, cited as "research §N")
was independently re-verified by reading the current source file during this planning pass, not copied
blind — see the per-task-group "Verified against current code" notes.

---

## Task Group 1 — Prerequisite: fix the `APLValue<Component>` bug

Ships as part of the single PR (see Constraints — no separate-PR exception). Must land, and its own
tests must pass, before Task Group 4 (APL converter dispatch) locks in the closed-type inventory, per
the ADR's explicit sequencing requirement ("Completion Criteria": *"The pre-existing `APLValue<Component>`/
`System.ComponentModel.Component` bug is corrected before the final closed APL generic type map is
established"*).

**Verified against current code:**
- `src/AlexaVoxCraft.Model.Apl/Components/AlexaSwipeToAction.cs:1` — `using System.ComponentModel;`;
  line 179 — `public APLValue<Component>? ComponentSlot { get; set; }`.
- `src/AlexaVoxCraft.Model.Apl/Components/AlexaTextListItem.cs:1` — `using System.ComponentModel;`;
  line 28 — `public APLValue<Component>? ComponentSlot { get; set; }`.
- No type literally named `Component` exists in `AlexaVoxCraft.Model.Apl`; the unqualified `Component`
  binds to `System.ComponentModel.Component` via the stray `using`. `APLComponent`
  (`src/AlexaVoxCraft.Model.Apl/Components/APLComponent.cs`) is the intended base component type used
  pervasively elsewhere in the same files for component-typed properties/slots.

**Changes:**
1. In both files, change `APLValue<Component>` → `APLValue<APLComponent>` for the `ComponentSlot`
   property.
2. Remove `using System.ComponentModel;` from both files if nothing else in the file still needs it
   (verify with a build — `ComponentSlot`'s declared type is the only usage found during planning, but
   re-check at implementation time since other members in these files were not exhaustively read here).
3. Grep the rest of `AlexaVoxCraft.Model.Apl` for `using System.ComponentModel;` to confirm no sibling
   file has the same latent bug (planning-time grep found it only in these two files, but re-run at
   implementation time since this plan's grep was not exhaustive across every file).

**Testing:**
- `ComponentSlot` is a property on `AlexaSwipeToAction`/`AlexaTextListItem` — check whether existing
  Verify snapshots under `test/AlexaVoxCraft.Model.Apl.Tests/Snapshots` exercise these two components
  with a populated `ComponentSlot`. If none do, add a focused Compono-based test (serialize + deserialize
  round trip) that sets `ComponentSlot` to a real `APLComponent`-derived value (e.g. a `Text` or
  `Container`) and asserts the wire shape is unchanged in structure (same JSON shape as before — this is
  a type-correctness fix, not a wire-format change, since `System.ComponentModel.Component` was never
  actually serializable/meaningful in this position; if any existing snapshot happens to cover this
  property with a non-null value, treat its diff as **semantic**, not cosmetic — a real behavior change
  from a type that couldn't have round-tripped correctly to one that can).
- Run `dotnet build AlexaVoxCraft.slnx` to confirm no other compile-time dependency on `Component`
  existed in these two files.
- Run the full `Model.Apl` test project (`dotnet run --project test/AlexaVoxCraft.Model.Apl.Tests
  --framework net9.0`, per CLAUDE.md's Microsoft.Testing.Platform convention) and inspect any snapshot
  diffs semantically, not blindly (per repo convention already established in plan 0002).

**Open decisions:** none — this is a fully determined, mechanical fix per the ADR.

---

## Task Group 2 — Core Model serialization/resolver architecture

Implements ADR sections "Serialization Metadata Ownership" (Model portion), "Resolver Composition and
JIT Compatibility", and "JIT Compatibility Behavior".

**Verified against current code:**
- `src/AlexaVoxCraft.Model/Serialization/AlexaJsonOptions.cs` (full file read): static class,
  `DefaultOptions` getter with `_version`/`_cachedVersion`/`_cachedOptions`/`_lock` double-checked
  cache, `CreateOptions()` builds `new AlexaTypeResolver()`, adds `Modifiers.SetNumberHandlingModifier`
  then every registered `_modifiers` entry, builds `JsonSerializerOptions { TypeInfoResolver = resolver,
  ReadCommentHandling = Skip }`, adds `ObjectConverter` then every registered `_converters` entry.
  `RegisterConverter<T>` and `RegisterTypeModifier<T>` both append under `_lock` and bump `_version`.
  `RegisterTypeModifier<T>` wraps the modifier in a closure that filters `ti.Type == typeof(T)` before
  invoking.
- `src/AlexaVoxCraft.Model/Serialization/AlexaTypeResolver.cs` (full file read): `GetTypeInfo` calls
  `base.GetTypeInfo` then, for exactly three types, attaches a `ShouldSerialize` predicate:
  `ResponseBody.Directives` → non-empty `Count`; `Reprompt.Directives` → non-empty `Count`;
  `ImageSource` → `widthPixels`/`heightPixels` properties gated on `Width > 0`/`Height > 0`.

**Changes:**
1. Add a new internal `ModelContext : JsonSerializerContext` (suggested location:
   `src/AlexaVoxCraft.Model/Serialization/ModelContext.cs`, `internal partial class ModelContext :
   JsonSerializerContext`), metadata mode (required for polymorphism support per research §6 —
   fast-path-only generation is explicitly insufficient here).
2. **Determining the complete `[JsonSerializable]` set — do not assume "just the root types."** Verify
   against actual dispatch, not the ADR's narrative summary, before finalizing the list:
   - Every concrete type reachable from `RequestConverter`'s resolver list
     (`src/AlexaVoxCraft.Model/Request/Type/RequestConverter.cs`) — read every `IRequestTypeResolver`
     implementation's `Resolve`/`CanResolve` to enumerate every `Type` it can return, not just the
     resolver class names.
   - Every concrete type in every `BasePolymorphicConverter<T>` subclass's `DerivedTypes` dictionary
     (confirmed subclasses so far: `DirectiveConverter` — `_directiveDerivedTypes`, 12 built-in entries
     read directly from `src/AlexaVoxCraft.Model/Response/Converters/DirectiveConverter.cs`, plus
     `_directiveDataDrivenTypeFactories`'s `ConnectionSendRequestFactory.Create` return type(s); grep for
     every other `BasePolymorphicConverter<T>` subclass in `AlexaVoxCraft.Model` — e.g. `Card`,
     `OutputSpeech`, `ResponseBody`, `ConnectionTask` families — and read each one's `DerivedTypes`).
   - `JsonDirective` (`DirectiveConverter`'s `DefaultType`) and any other converter `DefaultType`/
     fallback type.
   - Every plain (non-polymorphic) type reachable from `SkillRequest`/`SkillResponse`'s object graph that
     isn't itself covered by a polymorphic converter (e.g. `Context`, `Session`, `Application`, `Device`,
     `Intent`, `Slot`, `User`, etc.) — enumerate by reading the object graph, not by guessing from class
     names.
   - Confirm whether `RequestConverter.RegisterRequestTypeResolver<TResolver>()`'s registered types (used
     internally by `AplSupport.Add()`/`InSkillPurchasingSupport.Add()`, but declared in `Model`) need
     entries in `ModelContext` or in the consuming package's own context — since these types are
     package-owned by `Model.Apl`/`Model.InSkillPurchasing`, not `Model`, they belong in those packages'
     contexts (Task Group 3), not `ModelContext`. Confirm this placement doesn't create a resolver-chain
     ordering gap (a `Model`-context lookup for an APL-owned type must correctly fall through to the
     `Model.Apl` context later in the chain, not fail).
   - `ObjectConverter` and any other custom `JsonConverter` currently in `AlexaJsonOptions._converters`/
     hard-registered in `CreateOptions()` — confirm each is declared with `[JsonSerializable]` where its
     target type needs generated metadata, or is purely a converter (no metadata needed) and just needs
     to stay registered on the options object as today.
3. Rewrite `AlexaJsonOptions.CreateOptions()`:
   - Base resolver becomes `IJsonTypeInfoResolver resolver = JsonTypeInfoResolver.Combine(
     ModelContext.Default, ...package resolvers registered via internal chain..., ...consumer resolvers
     registered via `RegisterTypeInfoResolver`..., jitFallback)` where `jitFallback =
     JsonSerializer.IsReflectionEnabledByDefault ? new DefaultJsonTypeInfoResolver() : null` (filtered
     out of `Combine`'s argument list when `null` — `Combine` does not accept `null` entries, so build the
     resolver list conditionally, e.g. via a `List<IJsonTypeInfoResolver>` filtered before calling
     `Combine(...)`, not by passing `null` directly).
   - Apply `Modifiers.SetNumberHandlingModifier` and every `_modifiers` entry via
     `.WithAddedModifier(...)` chained onto the **outermost** combined resolver — per the ADR, not onto
     any individual inner context — to preserve today's "one global modifier list filtered by
     `ti.Type == typeof(T)`" semantics exactly.
   - Re-express `AlexaTypeResolver`'s three hard-coded `ShouldSerialize` modifiers
     (`ResponseBody.Directives`, `Reprompt.Directives`, `ImageSource.Width`/`Height`) as one more
     `.WithAddedModifier(...)` call in the same outermost chain (equivalent to today's
     `resolver.Modifiers.Add(...)` calls inside `AlexaTypeResolver.GetTypeInfo`), applied ahead of or
     after consumer modifiers per current ordering (`AlexaTypeResolver`'s modifications happen inside
     `GetTypeInfo`, i.e. logically "first," before `_modifiers` are consulted — since
     `DefaultJsonTypeInfoResolver.Modifiers` runs in list-order after `base.GetTypeInfo`, confirm and
     preserve the relative order between the built-in `ShouldSerialize` modifier and consumer-registered
     modifiers with a regression test, not by assumption).
   - **Delete** `AlexaTypeResolver` (the ADR states it is *replaced*, not retained alongside the new
     design) once its three `ShouldSerialize` behaviors are fully reproduced via modifiers. Do not delete
     it before the replacement modifier is proven equivalent by test.
   - Cache/version invalidation (`_version`/`_cachedVersion`/`_cachedOptions`/`_lock`) is unchanged
     structurally — only what `CreateOptions()` builds inside the lock changes.
4. Add the new public API:
   ```csharp
   public static void RegisterTypeInfoResolver(IJsonTypeInfoResolver resolver)
   ```
   - Null guard: `ArgumentNullException.ThrowIfNull(resolver)`.
   - Thread safety: same `_lock`/version-bump pattern as `RegisterConverter`/`RegisterTypeModifier`.
   - Storage: a new `ImmutableArray<IJsonTypeInfoResolver> _consumerResolvers` (or equivalently named)
     field, appended to under `_lock`, consulted in `CreateOptions()` at the "step 3: consumer-provided
     resolvers" position in the chain — **after** step 1 (Model) and step 2 (package resolvers,
     Task Group 3) and **before** step 4 (JIT fallback), and this ordering must be structural
     (hard-coded position in the list `CreateOptions()` builds), not dependent on registration call
     order — this is the ADR's call-order-independence invariant. No `RegisterContext<TContext>()`
     convenience overload is added (ADR is explicit on this).
5. Internal package-resolver registration (step 2) needs its own storage separate from the consumer-
   resolver list from step 4, populated only by `AplSupport.Add()`/`InSkillPurchasingSupport.Add()`/
   SMAPI's bootstrap (Task Group 3) calling an **internal**, non-public sibling method or field — the
   ADR requires this remain "an internal concern of each package's own bootstrap path" and never share
   ordering semantics with consumer registrations regardless of call timing. Concretely: add an internal
   static method (e.g. `internal static void RegisterPackageTypeInfoResolver(IJsonTypeInfoResolver
   resolver)`, `InternalsVisibleTo` granted to `AlexaVoxCraft.Model.Apl`/`AlexaVoxCraft.Model.
   InSkillPurchasing`/`AlexaVoxCraft.Smapi`) that appends to a separate list always placed ahead of the
   public consumer list in `CreateOptions()`'s chain construction, independent of whether the public or
   internal registration method was called first at runtime.

**Testing:**
1. **Precedence is call-order-independent** — a test that registers a consumer resolver via
   `RegisterTypeInfoResolver` *before* simulating a package bootstrap call (or vice versa, in a second
   test), then asserts the package/library resolver's `JsonTypeInfo` wins for any type both could
   theoretically produce (or, more directly, asserts the resulting chain's resolver order matches
   expectations via reflection/behavioral probing, since `JsonTypeInfoResolver.Combine`'s internal order
   isn't publicly inspectable — assert via *behavior*: register a consumer resolver covering a type that
   collides with a library-owned type name is not possible today since library types aren't consumer-
   extensible, so instead assert via a type only the JIT fallback could produce, confirming it's never
   reached when a generated resolver already covers the type).
2. **Consumer metadata gets used** — register a test `JsonSerializerContext` covering a POCO not known
   to `ModelContext`, confirm `JsonSerializer.Serialize`/`Deserialize` against `AlexaJsonOptions.
   DefaultOptions` succeeds for that POCO.
3. **JIT fallback still resolves unregistered POCOs when reflection enabled** — a POCO registered nowhere
   still round-trips via `AlexaJsonOptions.DefaultOptions` in an ordinary (non-AOT) test run.
4. **Regression coverage for `RegisterTypeModifier<T>`/`RegisterConverter<T>` before/after first
   `DefaultOptions` access** — four cases: modifier registered before first access, after first access,
   alongside a consumer resolver, alongside a package resolver; confirm all four still take effect
   (today's already-correct "no freeze on first use" behavior, per research §6, must not regress).
5. **`ShouldSerialize` behavior preserved exactly** — dedicated tests (or confirm existing Verify
   snapshots already cover this) for: `ResponseBody` with empty vs. non-empty `Directives` (property
   omitted vs. present), `Reprompt` same, `ImageSource` with `Width`/`Height` <= 0 vs. > 0 (properties
   omitted vs. present).
6. Run the full existing `AlexaVoxCraft.Model.Tests` suite and inspect every Verify snapshot diff
   semantically (does the new output deserialize to an equivalent object graph?) vs. cosmetically
   (property-order-only drift, expected per ADR "Backward Compatibility" — byte-order identity is not a
   promised contract).

**Open decisions for implementation time (not settled by the ADR):**
- Exact internal storage shape for "package resolvers vs. consumer resolvers vs. JIT fallback" (a
  three-list struct, an enum-tagged single list, etc.) — the ADR only requires the *invariant*, not the
  data structure.
- Whether the built-in `ShouldSerialize` modifier is applied before or after registered
  `_modifiers`/consumer modifiers in the final `.WithAddedModifier` chain — must be pinned by a test
  proving today's *effective* behavior (all three built-ins currently run inside `AlexaTypeResolver.
  GetTypeInfo`, i.e., logically before `Modifiers` list entries) is preserved, not decided by
  convenience.

---

## Task Group 3 — Package-owned contexts and runtime serialization paths

Implements ADR "Package Context Ownership" and "Consumer Extension Contract".

### 3a. `AlexaVoxCraft.Model.Apl`

**Verified against current code:** `src/AlexaVoxCraft.Model.Apl/APLSupport.cs` (full file read) —
`APLSupport.Add()` is a single static method calling, in order: directive/request-handler
`AddSupport()`/`AddToRequestConverter()` registrations (12 calls), then ~104 `RegisterTypeInfo<T>()`
calls across nearly every APL model type (confirmed count from research §8's grep:
`grep -rl "RegisterTypeInfo" src/AlexaVoxCraft.Model.Apl` → 104 files).

**Changes:**
1. Add internal `AplModelContext : JsonSerializerContext` in `AlexaVoxCraft.Model.Apl` (suggested:
   `src/AlexaVoxCraft.Model.Apl/Serialization/AplModelContext.cs`), covering every APL
   component/command/document/data-source type reachable from `APLComponentConverter`'s dictionary
   (`src/AlexaVoxCraft.Model.Apl/JsonConverter/APLComponentConverter.cs`, ~50+ entries) plus every plain
   type referenced from `APLSupport.Add()`'s `RegisterTypeInfo<T>()` call list (~104 types) plus every
   closed `APLValue<T>`/`APLValueCollection<T>` instantiation's `T`/item type itself needs to be
   representable — confirm whether STJ source-gen requires `[JsonSerializable(typeof(APLValue<int>))]`
   style declarations for the closed-generic wrapper types themselves (likely yes, since `ModelContext`
   won't know about `Model.Apl`-owned closed generics) in addition to the two factory classes' internal
   dispatch (Task Group 4) — verify this interaction directly against STJ's source-gen behavior for
   `JsonConverterFactory`-serviced open generics before finalizing the list; this is exactly the kind of
   "custom converters and polymorphic dispatch may need explicit entries" case the ADR flags.
2. `AplSupport.Add()` gains one line: `AlexaJsonOptions.RegisterTypeInfoResolver(AplModelContext.Default)`
   — using the **internal** package-registration path from Task Group 2 (step 5), not the public
   consumer-facing `RegisterTypeInfoResolver` overload, so this cannot be reordered relative to consumer
   registrations.
3. Completeness-checking approach: write a test that walks every `[JsonConverter(typeof(...))]`-eligible
   type in the `AlexaVoxCraft.Model.Apl` assembly (via reflection over the assembly's public types, test-
   only reflection is fine — this isn't shipped code) and asserts `AplModelContext` (or the combined
   `AlexaJsonOptions.DefaultOptions` resolver with reflection *disabled*) can produce `JsonTypeInfo` for
   each — this directly catches "forgot to add a type to `[JsonSerializable]`" as a test failure rather
   than a first-use production surprise.

### 3b. `AlexaVoxCraft.Model.InSkillPurchasing`

**Verified against current code:** `src/AlexaVoxCraft.Model.InSkillPurchasing/InSkillPurchasingSupport.cs`
(full file read) — `Add()` calls `PaymentDirective.AddSupport()` and `ConnectionResponseHandler.
AddSupport()` only; a much smaller closed set than APL.

**Changes:** mirror 3a at smaller scale — internal `InSkillPurchasingModelContext`, one line added to
`InSkillPurchasingSupport.Add()`, covering ISP directive/response/product types.

### 3c. `AlexaVoxCraft.Smapi` — special precision required

**Verified against current code:**
- `src/AlexaVoxCraft.Smapi/Clients/AlexaSkillInvocationClient.cs:20-28` (confirms ADR's citation exactly)
  — constructor builds its own `new JsonSerializerOptions { PropertyNamingPolicy = null,
  DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, Encoder =
  JavaScriptEncoder.UnsafeRelaxedJsonEscaping }`, passed to `BaseClient`'s 3-arg constructor.
  `InvokeAsync<TRequest, TResponse>` (open, consumer-owned generic types) flows through this.
- `src/AlexaVoxCraft.Smapi/Clients/AlexaInteractionModelClient.cs:17-18` — uses `BaseClient`'s 2-arg
  constructor (implicit `BaseClient`-owned default options, Task Group 3d).
- `src/AlexaVoxCraft.Smapi/Builders/InteractionModel/InteractionModelBuilder.cs:177-190` (confirms ADR's
  citation exactly) — `ToJson` builds its own ad hoc default `new JsonSerializerOptions{...}` when
  `options` is null.
- **`src/AlexaVoxCraft.Smapi/AlexaVoxCraft.Smapi.csproj`** has exactly one `ProjectReference`:
  `AlexaVoxCraft.Http`. **`src/AlexaVoxCraft.Http/AlexaVoxCraft.Http.csproj`** has **zero**
  `ProjectReference`s at all (only `PackageReference`s to `LayeredCraft.StructuredLogging`,
  `Microsoft.Extensions.Http`, `Microsoft.Extensions.Logging`). Neither `Http` nor `Smapi` references
  `AlexaVoxCraft.Model` today, directly or transitively — `AlexaJsonOptions` (which lives in
  `AlexaVoxCraft.Model`) is **not reachable from either project's code as currently structured**. This is
  the one place the ADR's own text is imprecise against ground truth: the ADR's "Consumer Extension
  Contract" section asserts all three consumer-owned-type surfaces (including `BaseClient.
  Deserialize<TResult>`/`Serialize<T>` and `AlexaSkillInvocationClient.InvokeAsync<TRequest,TResponse>`)
  "ultimately route through the same shared `JsonSerializerOptions.TypeInfoResolver` chain ... confirmed
  from each call site, not assumed" — but as built today, `BaseClient`'s and SMAPI's options objects are
  fully independent `JsonSerializerOptions` instances with **no code path to `AlexaJsonOptions.
  DefaultOptions` at all**, let alone a shared chain. This is not a conflict with the ADR's *architecture*
  (the ADR does not forbid adding an internal `ProjectReference`, and does not require `Http`/`Smapi` to
  own or duplicate `Model`'s context) — it is a gap between the ADR's descriptive claim and the current
  dependency graph that this plan must close with a concrete, in-scope step (below), not silently paper
  over.

**Resolution — the shared-chain participation is settled, not open:**

Per this plan's governing instructions, Http/SMAPI default serialization **must** compose with the same
resolver chain `AlexaJsonOptions.DefaultOptions` already builds (package-owned contexts + consumer
resolvers registered via `RegisterTypeInfoResolver` + JIT fallback), because that is the only way a
consumer's single `RegisterTypeInfoResolver(...)` call satisfies all three surfaces the ADR names. The
mechanism:

1. **Add a `ProjectReference` from `AlexaVoxCraft.Http` to `AlexaVoxCraft.Model`.** This is a new
   dependency-graph edge (confirmed absent above) but is not a new public API and does not violate any
   ADR-0001 constraint — `AlexaJsonOptions.DefaultOptions` is already public, and consuming an already-
   public property from a sibling package is an ordinary internal-architecture choice, not something the
   ADR reserves. `AlexaVoxCraft.Smapi` gains the same reachability transitively through its existing
   `ProjectReference` to `Http`; no separate edge needed for `Smapi`.
2. **`BaseClient`'s default-constructor path (2-arg constructor)** builds its default `JsonSerializerOptions`
   by taking `AlexaJsonOptions.DefaultOptions.TypeInfoResolver` (the fully-composed Model + Model.Apl +
   Model.InSkillPurchasing + consumer-resolver + JIT-fallback chain Task Group 2 builds) as its
   `TypeInfoResolver`, while **preserving every other setting `BaseClient` configures today**
   (`PropertyNamingPolicy = CamelCase`, `DefaultIgnoreCondition = WhenWritingNull`, `WriteIndented = true`,
   `Encoder = UnsafeRelaxedJsonEscaping` — confirmed at `BaseClient.cs:36-42`). `Http` itself still owns
   no closed model types and gets no context of its own — it only reuses `Model`'s already-composed
   resolver, per point 1.
3. **`Smapi`'s three ad hoc options instances** (`AlexaSkillInvocationClient.cs:20-28`,
   `AlexaInteractionModelClient.cs:17-18` via `BaseClient`'s default, `InteractionModelBuilder.
   cs:177-190`) each compose `IJsonTypeInfoResolver resolver = JsonTypeInfoResolver.Combine(
   SmapiModelContext.Default, AlexaJsonOptions.DefaultOptions.TypeInfoResolver!)` — SMAPI's own
   package-owned context first, then the already-fully-composed shared chain appended — while preserving
   each call site's own existing settings (`AlexaSkillInvocationClient`'s `PropertyNamingPolicy = null`,
   `InteractionModelBuilder`'s `CamelCase`/`WriteIndented = true`, etc. — these are call-site-specific and
   must not be unified into one shared options object, only the resolver is shared). This directly
   satisfies all 5 constraints simultaneously:
   - **(1) `SmapiModelContext.Default` available** — baked into the composition unconditionally.
   - **(2) consumer resolvers via `RegisterTypeInfoResolver` available to open `TRequest`/`TResponse`** —
     satisfied because `AlexaJsonOptions.DefaultOptions.TypeInfoResolver` already includes them (Task
     Group 2's chain), and accessing `AlexaJsonOptions.DefaultOptions` re-triggers Model's existing
     version-cached rebuild, so a consumer's registration made at any point before first actual
     serialization use is picked up — **provided SMAPI's own composition is built freshly (or via an
     equivalently version-aware lazy pattern), not frozen once into a `static readonly` field at type-init
     time**, since a `RegisterTypeInfoResolver` call could happen after SMAPI's static fields would
     otherwise have already initialized. This is the one implementation-time detail left open below (a
     caching-shape choice, not an architectural one).
   - **(3) standalone `InteractionModelBuilder.ToJson()` works with no prior DI init** — satisfied: both
     `SmapiModelContext.Default` and `AlexaJsonOptions.DefaultOptions` are plain static members requiring
     no DI container.
   - **(4) JIT fallback remains available in JIT mode** — satisfied: it is already the last entry inside
     `AlexaJsonOptions.DefaultOptions`'s own chain (Task Group 2), so it is inherited automatically by
     composing against that chain; SMAPI does not need its own separate JIT-fallback logic.
   - **(5) no new public bootstrap API** — satisfied: this is pure internal composition plus one new
     `ProjectReference`; no new AlexaVoxCraft-authored public member is introduced anywhere in this
     mechanism.
   - `AlexaSkillInvocationClient`'s constructor and `InteractionModelBuilder.ToJson`'s ad hoc default are
     both changed to build their options this way. `AlexaInteractionModelClient` — verify at
     implementation time whether it needs to move from `BaseClient`'s 2-arg to 3-arg constructor to pass
     an explicit SMAPI-composed default (since a generic `BaseClient` 2-arg default, per point 2, only
     carries `Model`'s chain, not `SmapiModelContext.Default` — SMAPI-owned types need the 3-arg
     constructor with the SMAPI-composed options from this section). This is a small, in-scope, non-
     breaking internal constructor-selection change, not an open architectural question.
4. Existing `AddSmapiDeveloperClient(...)`/`AddSkillInvocationClient(...)` DI extensions may still
   *participate* in configuring the rest of the app's shared options, but per point 3's design they are no
   longer the *only* source of `SmapiModelContext`/shared-chain availability for these three call sites —
   the composition is intrinsic to the types' own static/constructor-time option-building, independent of
   DI having run.

### 3d. `AlexaVoxCraft.Http` / `BaseClient`

**Verified against current code:** `src/AlexaVoxCraft.Http/Clients/BaseClient.cs:36-42` (2-arg
constructor's inline default options — confirmed exact lines) delegates to the 3-arg constructor
(`BaseClient.cs:46-51`) with an inline `new JsonSerializerOptions { PropertyNamingPolicy = CamelCase,
DefaultIgnoreCondition = WhenWritingNull, WriteIndented = true, Encoder = UnsafeRelaxedJsonEscaping }`
(no `TypeInfoResolver` set — implicit reflection default). 3-arg constructor takes an explicit
`JsonSerializerOptions jsonSerializerOptions` and stores it verbatim (null-guarded,
`BaseClient.cs:46-51`), with **no mutation of a caller-supplied options object anywhere in the class** —
confirming the ADR's "Backward Compatibility" contract already holds structurally for the explicit-options
path and only needs to be preserved, not newly implemented.

**Changes:**
1. `BaseClient`'s **default-constructor path (2-arg constructor)** becomes AOT-safe per 3c's resolution
   above: its default `JsonSerializerOptions` must expose `TypeInfoResolver =
   AlexaJsonOptions.DefaultOptions.TypeInfoResolver` (via the new `ProjectReference` to `AlexaVoxCraft.
   Model` from 3c point 1), while keeping its existing `PropertyNamingPolicy`/`DefaultIgnoreCondition`/
   `WriteIndented`/`Encoder` settings unchanged. This is no longer an open question of *whether* `Http`'s
   default includes `Model`'s chain — it must, per the now-settled invariant — only *how* the reference is
   wired is implementation mechanics — subject to the resolver-freshness invariant below, which rules out
   the naive form of "how" (a one-time property assignment captured into a field at construction).

**Resolver freshness invariant (applies to `BaseClient`'s default path here, and to all three SMAPI call
sites in 3c point 3):**

Per `BaseClient.cs:24`/`:36-51`, `JsonSerializerOptions` (the stored field) is `protected readonly` and is
assigned exactly once, in the constructor, to a single `JsonSerializerOptions` instance — the 2-arg
constructor currently builds that instance inline (`BaseClient.cs:36-42`) and delegates to the 3-arg
constructor, which stores it verbatim. `Serialize`/`Deserialize` (`SendAsync<TResult>`, `SendAsync`,
lines ~263-332) always read this same stored instance; nothing re-reads `AlexaJsonOptions.DefaultOptions`
at the moment of actual serialization.

If step 1's default-path `TypeInfoResolver` is captured from `AlexaJsonOptions.DefaultOptions.
TypeInfoResolver` **once, at construction time**, and a consumer later calls
`AlexaJsonOptions.RegisterTypeInfoResolver(MyContext.Default)` **after** that `BaseClient`-derived
instance was already constructed, the already-constructed instance would keep serializing/deserializing
against the pre-registration resolver — even though `AlexaJsonOptions.DefaultOptions` itself was
correctly invalidated and rebuilt (Task Group 2's version-counter semantics). This would make the
documented "one `RegisterTypeInfoResolver` call covers all three surfaces" consumer contract (3c,
"Resolution" intro) silently depend on Http/SMAPI client construction order relative to registration —
an ordering dependency this plan does not intend to introduce and has not chosen to document as part of
the public contract.

**Required invariant:** for AlexaVoxCraft-owned **default** serialization paths (the 2-arg `BaseClient`
constructor here, and `AlexaSkillInvocationClient`, `AlexaInteractionModelClient`, and
`InteractionModelBuilder.ToJson(options: null)` in 3c), consumer resolver registration must not become
accidentally dependent on client-construction order unless that ordering is a deliberate, documented part
of the public contract — which this plan does not choose. Concretely:
- registration before first construction/use must work (already true for any reasonable design);
- registration *after* an earlier `BaseClient`-derived instance was constructed, but *before* that
  instance's first actual `Serialize`/`Deserialize` call, must still be picked up by that instance's
  default-path serialization;
- a package-default `JsonSerializerOptions` must not silently freeze an obsolete resolver merely because
  the client object (or a `static readonly` options field) happened to initialize earlier.

**Implementation-time task:** determine the smallest change to `BaseClient.Serialize<T>`/`Deserialize
<TResult>` (and the equivalent SMAPI call sites) that satisfies this invariant without capturing a stale
`TypeInfoResolver` into a permanently-stored `JsonSerializerOptions`. Candidate shapes — none chosen here,
implementation selects based on what a `JsonSerializerOptions` object's post-first-use immutability
(STJ locks an options instance the first time it is used for (de)serialization) actually permits:
- build the default-path `JsonSerializerOptions` lazily, on first actual use rather than in the
  constructor, still only once per instance (closes the "constructed-before-registration" gap but not a
  "registered, then this exact instance used repeatedly across a long-lived singleton client" gap);
- an internal delegating `IJsonTypeInfoResolver` that forwards each `GetTypeInfo` call to whatever
  `AlexaJsonOptions.DefaultOptions.TypeInfoResolver` currently is, so the outer `JsonSerializerOptions`
  object can be constructed once (and safely locked by STJ) while still observing later registrations —
  this composes naturally with `JsonTypeInfoResolver.Combine`, which already accepts an
  `IJsonTypeInfoResolver`, not just a `JsonSerializerContext`;
- version-aware rebuild (mirroring `AlexaJsonOptions`'s own `_version`/`_cachedVersion` pattern from Task
  Group 2) at the call sites that read the stored options, rebuilding a fresh `JsonSerializerOptions`
  only when `AlexaJsonOptions`'s version has advanced since the client's own options were last built.

This choice is common to `BaseClient`'s default path and all three SMAPI call sites in 3c point 3 — it
should be designed and tested once and reused, not solved independently per call site. Only
**explicitly consumer-supplied `JsonSerializerOptions` (the 3-arg `BaseClient` constructor) are exempt**
— those remain frozen exactly as the caller provided them, per the ADR's "must never be
mutated/replaced" contract; the freshness invariant applies only to AlexaVoxCraft-created default options.

If, after investigation, the smallest correct implementation would require consumers to call
`RegisterTypeInfoResolver(...)` **before** constructing any `BaseClient`-derived instance whose lifetime
crosses that registration — i.e., freshness cannot practically be guaranteed for already-constructed,
long-lived client instances — that is a genuine public behavioral constraint, not an internal detail, and
must be flagged for ADR-0001/documentation consideration rather than silently adopted as the shipped
behavior.
2. **Explicit-options-constructor path (3-arg) is untouched** — a consumer/subclass supplying its own
   `JsonSerializerOptions` continues to own that configuration outright, per ADR "Backward Compatibility"
   ("AlexaVoxCraft must not silently mutate or replace an explicitly consumer-supplied
   `JsonSerializerOptions`") — confirmed already true structurally, per "Verified against current code"
   above; this plan only requires that it remain true.
3. `InSkillPurchasingClient` (`src/AlexaVoxCraft.InSkillPurchasing/Clients/InSkillPurchasingClient.cs`)
   currently uses `BaseClient`'s 2-arg constructor (research §7) — per 3d step 1, `Http`'s own AOT-safe
   default now already carries `AlexaJsonOptions.DefaultOptions.TypeInfoResolver`, which includes
   `InSkillPurchasingModelContext` (3b) once `InSkillPurchasingSupport.Add()` has registered it via the
   internal package-resolver channel (Task Group 2 step 5) — `AlexaVoxCraft.InSkillPurchasing` (the
   runtime client package) already transitively references `AlexaVoxCraft.Model` via its existing
   `ProjectReference` to `AlexaVoxCraft.MediatR` (confirmed:
   `src/AlexaVoxCraft.InSkillPurchasing/AlexaVoxCraft.InSkillPurchasing.csproj` references `Http` and
   `MediatR`; `MediatR` references `Model`), so no new edge is needed there. Whether `InSkillPurchasingClient`
   still needs to move to the 3-arg constructor to pass its own explicitly-composed default (mirroring
   `AlexaSkillInvocationClient`'s pattern in 3c) or can rely on `Http`'s now-AOT-safe 2-arg default as-is is
   the one remaining implementation-time decision here — it depends only on whether ISP has any package-
   specific option settings (naming policy, etc.) beyond what `Http`'s default already provides, which is a
   fact to confirm by reading `InSkillPurchasingClient.cs` at implementation time, not an architectural
   question.
4. Integrate with `services.AddInSkillPurchasing()`
   (`src/AlexaVoxCraft.InSkillPurchasing/ServiceCollectionExtensions.cs:20`) per the ADR's "ride-along"
   principle — no new bootstrap method; either the `AddHttpClient<...>` registration or
   `InSkillPurchasingClient`'s constructor call site is where the ISP-composed default gets supplied, if
   step 3 concludes one is needed.

**Testing (3a-3d, combined):**
- Round-trip tests (serialize + deserialize) for representative types from each package's context,
  run under both reflection-enabled and reflection-disabled (`JsonSerializer.
  IsReflectionEnabledByDefault = false`, via `AppContext.SetSwitch` in a test harness or a dedicated
  test project targeting the switch at publish time — confirm the correct mechanism for flipping this in
  a unit test vs. requiring the Task Group 7 validation app) to catch any type silently depending on the
  JIT fallback.
- SMAPI-specific: a test that constructs `AlexaSkillInvocationClient`/`AlexaInteractionModelClient`/
  `InteractionModelBuilder` **without** any DI container having run `AddSmapiDeveloperClient`/
  `AddSkillInvocationClient`, and confirms default serialization still succeeds for `Smapi`-owned types —
  this directly proves the ADR's "independent of DI init order" requirement, not just asserts it compiles.
- A test proving `RegisterTypeInfoResolver(...)` called by a consumer at app startup is actually visible
  through `AlexaSkillInvocationClient.InvokeAsync<TRequest,TResponse>` for a consumer-owned `TRequest`/
  `TResponse` — this is the concrete proof of 3c's "one registration covers all three surfaces" invariant,
  not merely a compilation check.
- A resolver-freshness test proving the invariant above: construct a `BaseClient`-derived instance (and,
  separately, each SMAPI client) **before** calling `AlexaJsonOptions.RegisterTypeInfoResolver(...)` for a
  new consumer type, then perform a `Serialize`/`Deserialize` call on that *already-constructed* instance
  **after** the registration, and assert the consumer type is now resolvable — proving default-path
  serialization does not freeze a stale resolver captured at construction time. If this test cannot pass
  without requiring registration-before-construction, that failure is itself the signal to escalate per
  the invariant's final paragraph above, not a reason to weaken the test.
- Reuse existing `AlexaVoxCraft.Smapi.Tests`/`AlexaVoxCraft.InSkillPurchasing.Tests` test infrastructure;
  for any fake-HTTP-transport need in these unit test projects (as opposed to the rooted Native AOT
  validation app, which has its own dependency-minimal constraint — see Task Group 7), continue using this
  repo's normal Compono test helpers as before, unchanged by this plan.

**Open decisions for implementation time:**
- Whether `AlexaInteractionModelClient` moves to `BaseClient`'s 3-arg constructor (3c point 3) — settled
  that it needs SMAPI-composed options; only the exact constructor-selection mechanics are open.
- Whether `InSkillPurchasingClient` needs its own 3-arg-constructor-supplied composed default or can rely
  on `Http`'s now-AOT-safe 2-arg default as-is (3d step 3) — a fact to confirm by reading current code, not
  an architectural choice.
- Exact caching/freshness mechanism satisfying the resolver-freshness invariant (introduced above, applies
  identically to `BaseClient`'s default path in 3d and all three SMAPI call sites in 3c point 3) — must
  remain version-aware relative to `AlexaJsonOptions.DefaultOptions`'s own cache rather than freezing once
  at construction/static-init time; the specific data structure (delegating resolver, version-checked
  rebuild, or lazy-on-first-use) is an implementation detail, designed once and reused across both. If
  investigation shows the smallest correct design requires consumers to register before constructing any
  long-lived default-path client instance, that is a public behavioral constraint requiring ADR-0001/
  documentation consideration, not a silently-adopted implementation choice.
- Exact `[JsonSerializable]` interaction with `APLValue<T>`/`APLValueCollection<T>` open-generic wrapper
  types (3a step 1) — must be verified empirically against STJ source-gen behavior, not assumed.

---

## Task Group 4 — APL dynamic-generic elimination

Implements ADR "APL Generic Converter Strategy". **Depends on Task Group 1 being complete** — the closed
type inventory below must be re-verified post-bugfix, not copied from the research doc's pre-fix
enumeration.

**Verified against current code (full read of both factory files):**
- `src/AlexaVoxCraft.Model.Apl/JsonConverter/APLValueConverterFactory.cs`:
  - `CanConvert`: true for `APLDimensionValue`/`APLAbsoluteDimensionValue`/`APLValue<object>` (the
    `_dimensionTypes` list) or any closed `APLValue<T>`.
  - `CreateConverter` delegates to `CreateConverterInternal` via a `ConcurrentDictionary<Type,
    JsonConverter?> _converterCache.GetOrAdd`.
  - `CreateConverterInternal`: for `_dimensionTypes` members, a `switch` returns
    `APLDimensionValueConverter`/`APLAbsoluteDimensionValueConverter`/`APLObjectConverter` (already
    closed dispatch, no reflection — keep as-is). For everything else: extracts `valueType =
    typeToConvert.GetGenericArguments()[0]`; if `valueType` is itself generic and
    `IsEnumerableOfType()`, does `Activator.CreateInstance(typeof(APLEnumerableValueConverter<,>).
    MakeGenericType(innerValueType, valueType), ...)` (confirmed dead branch per research §5/§8 — zero
    shipped properties reach it); otherwise `Activator.CreateInstance(typeof(APLValueConverter<>).
    MakeGenericType(valueType), ...)`.
- `src/AlexaVoxCraft.Model.Apl/JsonConverter/APLValueCollectionConverterFactory.cs`: `CreateConverter`
  unconditionally does `Activator.CreateInstance(typeof(APLValueCollectionConverter<>).
  MakeGenericType(itemType), ..., args: [true], ...)`.

**Changes:**
1. **Re-run the closed-type enumeration after Task Group 1.** Grep `src/`, `test/`, `samples/` for every
   `APLValue<...>`/`APLValueCollection<...>` property declaration and nested usage (the same method
   research §8 used), confirming the previously-found `APLValue<Component>` entries now read
   `APLValue<APLComponent>` and produce no other diffs from the pre-fix 48+30 count. Do not assume the
   count is unchanged — re-count.
2. Replace `APLValueConverterFactory.CreateConverterInternal`'s `MakeGenericType`/`Activator.
   CreateInstance` branch (for the non-dimension-type, non-dead-branch case) with centralized,
   compile-time-closed dispatch **inside this class only** — implementation shape (if-chain vs.
   `Dictionary<Type, Func<JsonConverter>>` built once, mirroring the existing `_dimensionTypes` switch
   pattern) is an **open implementation-time decision**, not fixed by the ADR; either is acceptable as
   long as every arm is a literal `typeof(X)`/`new APLValueConverter<X>()` pair with no
   `MakeGenericType`.
3. Replace `APLValueCollectionConverterFactory.CreateConverter`'s `MakeGenericType`/`Activator.
   CreateInstance` with the equivalent centralized closed dispatch **inside this class only**.
4. Drop the `APLEnumerableValueConverter<TValue,TList>` branch from both closed dispatch tables (per ADR,
   confirmed unreachable). Decide, at implementation time, the exact mechanism for the JIT-only
   compatibility escape hatch for a consumer-constructed `APLValue<T>`/`APLValueCollection<T>`
   instantiation the library doesn't itself ship (e.g. via `JsonAttributeBag`) — the ADR explicitly
   defers this technique choice (gated via `IsReflectionEnabledByDefault`, `[RequiresDynamicCode]`/
   `[RequiresUnreferencedCode]`, or another mechanism) to be selected once the factory code is changed
   and analyzer behavior can be observed. **Investigate actual analyzer output (`EnableAotAnalyzer`/
   `EnableTrimAnalyzer` warnings on the changed factory files) before committing to a specific technique**
   — this is an explicit ADR instruction, not optional due diligence.
5. Cover nullable, enum, primitive, `Uri`, `APLValue<object>` (already special-cased, keep as-is), and
   nested shapes (`APLValueCollection<APLValue<int?>>` at `VectorGraphics/AVGPath.cs:40`, confirmed via
   research §8 — re-verify the line number still matches at implementation time) — nested types resolve
   recursively through the two factories exactly as today; only the leaf dispatch changes.
6. Ensure unsupported types fail loudly (an explicit, clear exception) rather than silently — both in the
   closed dispatch's default/`else` arm and in whatever the chosen JIT-only fallback technique produces
   under `PublishAot`.

**Testing:**
- Reuse existing `AlexaVoxCraft.Model.Apl.Tests` Verify snapshots covering every APL component/document
  that has an `APLValue<T>`/`APLValueCollection<T>` property — confirm the new dispatch produces
  identical (or wire-semantically equivalent, per the ADR's terminology distinction) output for every one
  of the 48/30-ish closed types.
- Add a focused test enumerating the full closed-type list (from step 1's re-enumeration) and asserting
  each resolves to a working converter via the new dispatch, without reflection (run with
  `IsReflectionEnabledByDefault = false` if feasible in-process, or defer this specific check to the
  Task Group 7 validation app if not).
- Add a test asserting the dropped `APLEnumerableValueConverter<,>` branch is genuinely unreachable from
  any shipped property (a compile-time/reflection-based assembly scan test, test-only reflection is
  fine) — this guards against a future property accidentally reintroducing the dead branch's shape.
- Add a test for the "unsupported type fails loudly" behavior using a synthetic `APLValue<TSomeUnknownType>`
  not in the closed set.

**Open decisions:** if-chain vs. dictionary dispatch implementation; exact JIT-only fallback technique
for out-of-library `APLValue<T>` instantiations (both explicitly deferred by the ADR to this
implementation).

---

## Task Group 5 — Mediator dispatch elimination

Implements ADR "Mediator Dispatch".

**Verified against current code:**
- `src/AlexaVoxCraft.MediatR/SkillMediator.cs:56-60` (full file read, confirms ADR's `SkillMediator.
  cs:58-60` citation): `Send` computes `requestTypeInternal = request.Request.GetType()`, then
  `RequestHandlers.GetOrAdd(requestTypeInternal, static t => (RequestHandlerWrapper)(Activator.
  CreateInstance(typeof(RequestHandlerWrapperImpl<>).MakeGenericType(t)) ?? throw ...))`, cached in a
  static `ConcurrentDictionary<Type, RequestHandlerWrapper>`. `SkillMediator` already holds an
  `IServiceProvider _serviceProvider` instance field, set in its constructor
  (`SkillMediator.cs:19,29-33`) and used at `Send`'s call site (`_serviceProvider.GetRequiredService<
  ILogger<SkillMediator>>()`, `handler.Handle(request, _serviceProvider, cancellationToken)`).
- `src/AlexaVoxCraft.MediatR/Wrappers/RequestHandlerWrapper.cs` (full file read): `RequestHandlerWrapper`
  is a **public abstract class** exposing `Handle(SkillRequest, IServiceProvider, CancellationToken)`
  (non-generic surface). `RequestHandlerWrapperImpl<TRequestType>` (`where TRequestType : Request`) is a
  **public, ordinary generic class** — nothing about it is `internal` or otherwise inaccessible from a
  consumer assembly. A consumer's own compiled code can write
  `new RequestHandlerWrapperImpl<MyApp.LaunchRequest>()` today with no reflection, because
  `MyApp.LaunchRequest` (or any `AlexaVoxCraft.Model`-owned request type) is compile-time-known in the
  consumer's own compilation.
- `src/AlexaVoxCraft.MediatR.Generators/Generators/InterceptorEmitter.cs` (full file read): the
  interceptor-generated `AddSkillMediator(...)` override, emitted **into the consumer assembly**, calls
  `services.AddTransient<AlexaVoxCraft.MediatR.IRequestHandler<{requestTypeName}>, {handlerTypeName}>()`
  for every discovered handler (`EmitHandlerRegistrations`, lines 90-120) — a compile-time-closed generic
  DI registration, no `MakeGenericType`/`Activator.CreateInstance` anywhere in this path today. This
  solves **"construct a handler instance for a compile-time-known `IRequestHandler<TRequestType>`."**
  It does **not** solve `SkillMediator.Send`'s actual problem: **"given only a runtime `Type` obtained
  from `request.Request.GetType()`, find the right non-generic dispatch wrapper."** These are genuinely
  different problems — DI registration is keyed by a compile-time generic parameter the *caller* supplies
  (`GetServices<IRequestHandler<TRequestType>>()` requires `TRequestType` as a generic argument at the
  call site); `SkillMediator.Send` has no such compile-time parameter, only a `Type` value, which is
  exactly why today's code resorts to `MakeGenericType`. Extending DI registration alone does not close
  this gap.
- `src/AlexaVoxCraft.MediatR/AlexaVoxCraft.MediatR.csproj:38-39` — `InternalsVisibleTo` is granted only to
  `AlexaVoxCraft.MediatR.Tests`/`AlexaVoxCraft.MediatR.Lambda.Tests`, both fixed, in-solution assembly
  names. No repo-wide grep found any mechanism (`InternalsVisibleTo` or otherwise) that grants a consumer
  assembly (an arbitrary, unknown-at-library-build-time skill project name) access to `AlexaVoxCraft.
  MediatR`'s `internal` members. This rules out the Task Group 2/3 "internal package-registration method"
  pattern for this problem: that pattern works for `AlexaVoxCraft.Model.Apl`/`AlexaVoxCraft.Model.
  InSkillPurchasing` because those are fixed, known, in-solution assembly names the `AlexaVoxCraft.
  MediatR`/`AlexaVoxCraft.Model` projects can name explicitly in `InternalsVisibleTo`. A consumer's skill
  assembly has no fixed name AlexaVoxCraft can pre-authorize, so an equivalent `internal static void
  RegisterHandlerDispatch(...)` bridge is not available for generator-emitted consumer code to call into
  `AlexaVoxCraft.MediatR`.

**The bridge mechanism — resolved, no new public API, no ADR conflict.** The generator already emits
code into an `IServiceCollection` that flows into the same `IServiceProvider` `SkillMediator` already
holds — this **is** the existing, public, cross-assembly bridge; the gap is only that DI registration is
solved for the wrong axis (interface-typed resolution, not runtime-`Type`-keyed lookup). .NET's **keyed
services** (`Microsoft.Extensions.DependencyInjection.Abstractions` ≥ 8.0 — already the package this
project references, confirmed in `Directory.Packages.props:42` pinning `[8.0.2, 9.0.0)` for the net8.0
row, i.e. already new enough) close exactly this gap using only existing, public, non-AlexaVoxCraft
framework APIs:

1. `InterceptorEmitter.EmitHandlerRegistrations` additionally emits, for the same discovered
   `IRequestHandler<TRequestType>` type set it already registers, one keyed-singleton registration per
   distinct `TRequestType`, e.g.:
   ```csharp
   services.AddKeyedSingleton<AlexaVoxCraft.MediatR.Wrappers.RequestHandlerWrapper>(
       typeof(MyApp.LaunchRequest),
       (sp, key) => new AlexaVoxCraft.MediatR.Wrappers.RequestHandlerWrapperImpl<MyApp.LaunchRequest>());
   ```
   `TRequestType` is compile-time-known to the generator (it is the same type it already resolves for the
   `IRequestHandler<TRequestType>` registration on the line above), so this is an ordinary, compile-time
   generic instantiation — no `MakeGenericType`. The `Type` value (`typeof(MyApp.LaunchRequest)`) is used
   purely as an opaque key object for keyed-DI's internal (service type, key) lookup — `Type.Equals`/
   `GetHashCode` are ordinary, non-reflective instance methods, not reflection invocation. This requires
   no new `AlexaVoxCraft.MediatR.Generators`-side abstraction beyond what `EmitHandlerRegistrations`
   already does for the existing `AddTransient<...>()` line — it is the same discovered-type loop, one
   more emitted statement per entry.
2. `SkillMediator.Send` changes its dispatch lookup to try the keyed service first, falling back to
   today's `MakeGenericType`/`Activator.CreateInstance`-based `ConcurrentDictionary` cache only when no
   keyed registration exists for that `Type` (i.e., the `ServiceRegistrar`-only/interceptor-disabled
   carve-out, point 4 below):
   ```csharp
   var handler = _serviceProvider.GetKeyedService<RequestHandlerWrapper>(requestTypeInternal)
       ?? RequestHandlers.GetOrAdd(requestTypeInternal, static t => /* existing MakeGenericType path */);
   ```
   `GetKeyedService<RequestHandlerWrapper>(object? key)` resolves against the **non-generic, compile-time-
   fixed** `RequestHandlerWrapper` base type — the runtime `Type` is only ever used as the key argument,
   never as a generic type parameter, so no `MakeGenericType` is introduced on this path. Both
   `AddKeyedSingleton`/`GetKeyedService` are ordinary `Microsoft.Extensions.DependencyInjection`
   extension methods already available transitively through the package this project already references
   (`Microsoft.Extensions.DependencyInjection.Abstractions`, `AlexaVoxCraft.MediatR.csproj:32`) — no new
   package reference and, critically, **no new `AlexaVoxCraft`-authored public API**: the only public
   surface change ADR-0001 permits (`RegisterTypeInfoResolver`) is untouched by this mechanism, since
   the bridge is entirely existing framework API.
   `RequestHandlerWrapperImpl<TRequestType>` has no per-request-instance state (it resolves everything
   it needs from the `IServiceProvider`/`SkillRequest` parameters passed to `Handle` on each call), so
   registering it as a DI singleton is behaviorally equivalent to today's forever-cached
   `ConcurrentDictionary<Type, RequestHandlerWrapper>` entry — same lifetime semantics, different storage.
3. This mechanism is verified sound against every stated constraint: no `MakeGenericType` on the
   generator-emitted path, no `Activator.CreateInstance` on that path, no new AlexaVoxCraft public API,
   no reflection on the supported Native AOT path (`Type` equality/hashing is not reflection), and it
   does not require `InternalsVisibleTo` for an arbitrary consumer assembly, because nothing crosses an
   `internal` boundary — everything used (`RequestHandlerWrapper`, `RequestHandlerWrapperImpl<T>`,
   `IServiceCollection`, `IServiceProvider`, keyed-DI extensions) is already public.
4. Preserve the existing `ServiceRegistrar` reflection-based fallback
   (`src/AlexaVoxCraft.MediatR/Registration/ServiceRegistrar.cs`) for
   `EnableMediatRGeneratorInterceptor=false`/pre-8.0.400-SDK configurations, unchanged: when the
   interceptor never ran, no keyed registrations exist, so `GetKeyedService` returns `null` for every
   type and `SkillMediator.Send` falls through to today's `MakeGenericType` path automatically —
   document this (Task Group 10) as explicitly non-AOT-supported for that carve-out, per the ADR.

**Changes:**
1. Read `src/AlexaVoxCraft.MediatR/Wrappers/RequestHandlerWrapper.cs` and
   `src/AlexaVoxCraft.MediatR.Generators/Generators/InterceptorEmitter.cs` again at implementation start
   to re-confirm line numbers/shape haven't drifted, then implement the two changes above:
   `InterceptorEmitter.EmitHandlerRegistrations` emits one `AddKeyedSingleton<RequestHandlerWrapper>(...)`
   call per discovered `IRequestHandler<TRequestType>` entry, and `SkillMediator.Send` tries
   `GetKeyedService<RequestHandlerWrapper>(requestTypeInternal)` before falling back to the existing
   `MakeGenericType` path.
2. Confirm (via the generator's existing `SymbolDiscovery`/`RegistrationModel`) that the discovered
   `TRequestType` set for keyed-registration emission is exactly the same set already used for
   `AddTransient<IRequestHandler<TRequestType>, ...>()` — no separate discovery pass needed.
3. If a request type has multiple registered `IRequestHandler<TRequestType>` implementations (the
   existing multi-handler-with-`CanHandle()`-routing pattern), the keyed registration is still exactly
   one `RequestHandlerWrapperImpl<TRequestType>` per distinct `TRequestType` (not one per handler
   implementation) — `RequestHandlerWrapperImpl<T>.Handle` already iterates all registered
   `IRequestHandler<TRequestType>` instances internally via `GetHandlers<IRequestHandler<TRequestType>>`
   (`RequestHandlerWrapper.cs:26`); emit the keyed registration once per distinct `TRequestType`
   encountered, guarding against emitting a duplicate `AddKeyedSingleton` call for the same key if the
   same `TRequestType` has more than one handler.
4. Preserve the existing `ServiceRegistrar` reflection-based fallback unchanged (see point 4 above);
   document it (Task Group 10) as explicitly non-AOT-supported for that carve-out, per the ADR.

**Testing:**
- Generated-output tests: extend `AlexaVoxCraft.MediatR.Generator.Tests` (existing project, per
  `docs/plans/0001-...md`'s note that this project already uses explicit
  `new AlexaVoxCraftDiGenerator()` invocation and Verify snapshots for generated source) with new Verify
  snapshots asserting the emitted `AddKeyedSingleton<RequestHandlerWrapper>(...)` source for a
  representative set of handler types, including the multi-handler-per-request-type case (point 3 above).
- Runtime dispatch tests: a test that registers real handlers via the generator-produced
  `AddSkillMediator` output and calls `SkillMediator.Send` for each registered request type, asserting
  correct handler invocation via the keyed-service path specifically (e.g. by asserting the
  `ConcurrentDictionary<Type, RequestHandlerWrapper>` fallback cache never gets populated for a type that
  has a keyed registration, or by running under `IsReflectionEnabledByDefault=false` — `Activator.
  CreateInstance`/`MakeGenericType` are not gated by that switch today, so this alone doesn't prove the
  keyed path was used; prefer the direct assertion that the fallback path wasn't exercised).
- A test proving the `ServiceRegistrar`-only/interceptor-disabled fallback still dispatches correctly via
  the pre-existing `MakeGenericType` path when no keyed registrations exist (regression coverage for
  point 4's carve-out).
- Full `AlexaVoxCraft.MediatR.Tests`/`AlexaVoxCraft.MediatR.Lambda.Tests` suites must stay green.

**Open decisions:** none for the cross-assembly bridge mechanism itself (resolved above); remaining
implementation-time details that don't require further architectural resolution: exact emitted-statement
formatting/ordering relative to the existing `AddTransient<IRequestHandler<...>>()` line, and whether
`EmitHandlerRegistrations` groups by `TRequestType` in a single pass or a second pass over the same
discovered model (a pure code-shape choice with no behavioral consequence).

---

## Task Group 6 — Deferred package/dependency audits and remediation

Implements ADR "Runtime Package Scope" (Observability/MinimalLambda) and "Native AOT Compatibility
Definition" (`VerifyReferenceAotCompatibility` as investigative tool).

**Verified against current code:** `src/AlexaVoxCraft.Observability` contains `Extensions/
OpenTelemetryBuilderExtensions.cs` and `TelemetryProviders/{MeterProvider,ActivitySourceProvider}.cs` —
not read in depth this pass (ADR explicitly states neither package was deeply audited by the research
doc). `src/AlexaVoxCraft.MinimalLambda/Extensions/ServiceCollectionExtensions.cs:41-53` — full file
segment read: `AddAlexaSkillHost<THandler,TRequest,TResponse>()` calls `services.AddSingleton(
AlexaJsonOptions.DefaultOptions)` (rides Task Group 2's fix automatically), `services.AddSingleton<
ILambdaSerializer, AlexaLambdaSerializer>()`, and `ActivatorUtilities.CreateInstance<THandler>(sp)` — a
compile-time-closed generic per call site (research §5 classifies this as "(5) reflection statically
preservable," low risk).

**Changes (audit tasks, not fixes unless findings require them):**
1. **`AlexaVoxCraft.MinimalLambda`**: read every file in `src/AlexaVoxCraft.MinimalLambda` in full;
   confirm `AlexaLambdaSerializer` here is the same `AlexaVoxCraft.Lambda`-namespaced implementation
   research §3 flagged as a second, independent `ILambdaSerializer` (distinct from `AlexaVoxCraft.
   MediatR.Lambda`'s) — confirm it also just delegates to `AlexaJsonOptions.DefaultOptions`/consumer
   options with no separate reflection surface, or flag any independent `JsonSerializerOptions`
   construction found. Confirm `ActivatorUtilities.CreateInstance<THandler>` is AOT-legitimate per
   `Microsoft.Extensions.DependencyInjection.Abstractions`'s own trim/AOT annotations (it should be, this
   is a standard, widely-AOT-validated ASP.NET Core-style idiom, but verify the referenced package
   version actually carries the annotations rather than assuming).
2. **`AlexaVoxCraft.Observability`**: read every file in `src/AlexaVoxCraft.Observability` in full;
   audit for any reflection-based `Activity`/`Meter`/attribute-scanning surface (OpenTelemetry SDK
   registration commonly uses `Assembly`-scanning-based instrumentation-source discovery in some
   configurations — confirm whether this package's specific usage does). Classify findings using the
   same five-bucket scheme the research doc used (actual blocker / trimming concern / analyzer-warning-
   only / harmless reflection / statically preservable).
3. **External dependencies** (AWS Lambda runtime packages `Amazon.Lambda.RuntimeSupport`/`Amazon.Lambda.
   Serialization.SystemTextJson`/`Amazon.Lambda.Core`, Serilog packages, `LayeredCraft.Logging.
   CompactJsonFormatter` — all referenced by `AlexaVoxCraft.MediatR.Lambda.csproj` per research §11):
   enable `VerifyReferenceAotCompatibility` on `AlexaVoxCraft.MediatR.Lambda` (after `IsAotCompatible=true`
   is set on it, Task Group 8) and observe, in one CI run, whether `IL3058` fires against any of these.
   Use this **as an investigative tool**, not an unconditional gate (ADR is explicit: "Its initial
   cleanliness across every external dependency ... is not required if those dependencies function
   correctly under Native AOT despite lacking corresponding `IsAotCompatible` assembly metadata"). For
   any `IL3058` finding, distinguish: missing-metadata-only (the dependency likely works fine, just
   isn't annotated) vs. a genuine analyzer warning about real reflection inside that dependency vs. an
   actual confirmed runtime incompatibility (only provable via the Task Group 7 validation app actually
   exercising that dependency's code path under a native binary).
4. Neither `Observability` nor `MinimalLambda` is included in any `IsAotCompatible=true` claim (Task
   Group 8) until this audit is complete and its findings addressed or explicitly accepted as
   out-of-scope-but-documented risk.

**Testing:** primarily investigative — the "test" here is the audit itself plus whatever
`VerifyReferenceAotCompatibility`/analyzer output is captured. If the audit surfaces an actual
remediable finding (not merely a noisy warning), add a targeted fix and test following the same pattern
as the in-scope packages' task groups above.

**Open decisions:** whether any finding from this audit requires code changes beyond what's already
planned in Task Groups 2-5 — genuinely unknown until the audit runs, per the ADR's own "Deferred /
Validation-Time Questions" section.

---

## Task Group 7 — Rooted Native AOT validation application

Implements ADR "Validation / CI Requirements".

**Verified against current code:** `test/` currently contains only unit-test projects (`AlexaVoxCraft.
Model.Tests`, `AlexaVoxCraft.Model.Apl.Tests`, `AlexaVoxCraft.MediatR.Tests`, etc. — `ls test` output
above), all governed by `test/Directory.Build.props` (`net8.0;net9.0;net10.0;net11.0` multi-targeting,
`Microsoft.NET.Test.Sdk`/`xunit.v3.mtp-v2`). No existing integration-test or sample-publish project fits
"a rooted, actually-`dotnet publish -p:PublishAot=true`-and-executed application" — this is new. `samples/`
contains real, runnable Lambda function projects (`Sample.Apl.Function`, `Sample.Skill.Function`,
`Sample.Fact.InSkill.Purchases`, `Sample.Generated.Function`, `Sample.Host.Function`) that already
exercise normal public APIs the way a real consumer would, but none currently publish AOT or assert
against a native binary's execution.

**Changes:**
1. Add a new project — decide at implementation time whether it lives under `test/` (matching this
   repo's existing test-project location convention, e.g.
   `test/AlexaVoxCraft.NativeAot.ValidationApp/`) or under `samples/` (matching the "real, rooted consumer
   application using only public APIs" framing) — the ADR requires it be a real application, not a unit
   test project, but this repo's existing conventions lean toward `test/` for anything CI-gated and
   `samples/` for anything meant as consumer-facing documentation-by-example. Recommendation: `test/`,
   since it is CI infrastructure, not a documented sample — but confirm against how CI path-filtering
   (Task Group 9) is easiest to express, since `pr-build.yaml` already scopes broadly.
2. Project must NOT reference internal test-only hooks — only the normal public API, exactly like
   `samples/Sample.Apl.Function`/`samples/Sample.Skill.Function`.
3. Configure `<PublishAot>true</PublishAot>`, `<InvariantGlobalization>true</InvariantGlobalization>`,
   `<EnableTrimAnalyzer>true</EnableTrimAnalyzer>`, `<EnableAotAnalyzer>true</EnableAotAnalyzer>`, and
   force `<JsonSerializerIsReflectionEnabledByDefault>false</JsonSerializerIsReflectionEnabledByDefault>`
   (research §11 step 1 — reproduces the exact failure mode a real AOT-published consumer hits).
4. Exercise, per the ADR's Validation/CI Requirements list, at minimum:
   - **Core Alexa JSON**: deserialize a `LaunchRequest`, a slot-bearing `IntentRequest`, a
     `SessionEndedRequest` (reuse fixtures from `test/AlexaVoxCraft.Model.Tests/Examples`), serialize a
     response including existing polymorphic converters (directives, cards).
   - **APL**: call `AplSupport.Add()`, deserialize a `UserEventRequest`, exercise a representative
     component graph, serialize a response/directive containing `APLValue`/`APLValueCollection`
     properties (reuse `test/AlexaVoxCraft.Model.Apl.Tests/Examples`).
   - **Consumer metadata**: define a validation-app-owned POCO (e.g. `GameState`) plus its own generated
     `[JsonSerializable]` context, register it via `RegisterTypeInfoResolver`, exercise
     `JsonAttributeBag.Set<GameState>`/`Get<GameState>`, and prove resolver precedence (library metadata
     still wins for library types even after the consumer context is registered).
   - **Missing-consumer-metadata failure scenario**: attempt `JsonAttributeBag.Get<TUnregisteredType>()`
     (or similar) for a type genuinely not covered by any registered context, with reflection disabled,
     and assert the resulting exception is clear and immediate (not silent, not a generic crash) — this
     is a required, intentionally-failing scenario, not an oversight.
   - **Mediator**: exercise `SkillMediator.Send` through the Task-Group-5 generated dispatch path for a
     real registered handler.
   - **ISP/Http**: exercise ISP and `Http` serialization against a fake transport implemented as a **tiny,
     validation-app-local `HttpMessageHandler` subclass** (overriding `SendAsync` to return controlled
     `HttpResponseMessage` instances built in-line) — not live network. **Do not** add a dependency on
     `Compono.Http`, `Compono.Http.TestHttpHandler`, `HttpListener`, or any other test-helper/test-infra
     package to this validation app project specifically. Rationale: this executable's entire purpose is
     to prove AlexaVoxCraft and its genuine runtime dependencies under Native AOT; pulling in unrelated
     test infrastructure adds another AOT variable to a binary whose failures must be attributable
     unambiguously to AlexaVoxCraft's own code, not to whether some test-helper package happens to be
     AOT-clean. A custom `HttpMessageHandler` subclass wired into an `HttpClient` the validation app
     constructs itself is a handful of lines and has zero extra dependencies. This constraint is specific
     to this rooted validation app project only — ordinary unit tests elsewhere in the repo (Task Group
     3's "Testing (3a-3d, combined)" section, `AlexaVoxCraft.Smapi.Tests`, `AlexaVoxCraft.InSkillPurchasing.
     Tests`, etc.) continue using this repo's normal Compono test helpers exactly as before; nothing about
     this constraint changes how those tests are written.
   - **SMAPI**: exercise the package-owned metadata path (`InteractionModelDefinition` via
     `AlexaInteractionModelClient`, and standalone `InteractionModelBuilder.ToJson()` called directly, no
     DI container involved — proving Task Group 3c's "independent of DI init order" requirement for
     real), plus `AlexaSkillInvocationClient.InvokeAsync<TRequest,TResponse>` with both an
     AlexaVoxCraft-owned type and a validation-app-owned consumer type, against a fake transport (not
     live network).
   - **Lambda boundary**: exercise the real `AlexaLambdaSerializer`/hosting path (`AlexaSkillFunction` or
     equivalent), not just the underlying JSON calls in isolation.
5. `dotnet publish -r <rid> -p:PublishAot=true` for at least `linux-x64` (matching Lambda's
   `provided.al2023`), then **execute the resulting native binary directly** (not `dotnet run`), assert
   exit code and stdout/output against expectations for every scenario above.
6. Byte-identity/diff check against existing Verify snapshots for the fixtures reused above, per research
   §11 step 4, as an additional regression signal — not a new contract.

**Testing:** this task group *is* the validation application; its own correctness is proven by (a) the
publish step succeeding, (b) the native binary executing and producing expected output for every
scenario in step 4, and (c) the CI gate (Task Group 9) running it on every relevant PR going forward.

**Open decisions:** exact project location (`test/` vs. `samples/`); the fake-transport mechanism for
SMAPI/HTTP scenarios is settled (a validation-app-local `HttpMessageHandler` subclass, no external
test-infra dependency — see the ISP/Http bullet above); whether one monolithic validation app or several
smaller ones better serves clear failure attribution (the ADR does not mandate a specific project count,
only the scenario coverage).

**Addendum (2026-09-11, docs/plans/0004-native-aot-runtime-fixes-and-validation.md Task Group 5):** the
APL bullet above (step 4) named `UserEventRequest` as the type to deserialize for APL coverage — but
`UserEventRequest` is a polymorphic dispatch target, not the actual public entry-point type a consumer
deserializes an incoming request into (`APLSkillRequest`, the type `AlexaSkillFunction<APLSkillRequest,
SkillResponse>` and `AlexaLambdaSerializer.Deserialize<T>` are instantiated with per
`samples/Sample.Apl.Function`). `APLSkillRequest` never appeared in this plan or in ADR-0001's own
examples. The implemented validation app (`test/AlexaVoxCraft.NativeAot.ValidationApp`) additionally
substituted a response-direction `RenderDocumentDirective` round-trip for this bullet's specified
request-direction deserialize, so even the (insufficient) spec above was not fully implemented. Both gaps
together are why the `APLSkillRequest` metadata omission (Issue #190) shipped undetected; see
`docs/research/2026-09-11-native-aot-runtime-validation-gaps.md` §4-§5 for the full reconstruction. Kept
here, unedited above, as the historical record of what this plan actually specified — future validation-
scenario specs should name the real public entry-point type a consumer's serializer call site uses, not
an inner/derived type it happens to produce.

---

## Task Group 8 — Analyzers / `IsAotCompatible` decisions

Implements ADR "Native AOT Compatibility Definition".

**Changes:**
1. Build the explicit package-by-package matrix (columns: current AOT blockers / validation coverage /
   external dependency risk / eligible for `IsAotCompatible`?) for every package in "Runtime Package
   Scope": `AlexaVoxCraft.Model`, `AlexaVoxCraft.Model.Apl`, `AlexaVoxCraft.Model.InSkillPurchasing`,
   `AlexaVoxCraft.MediatR`, `AlexaVoxCraft.MediatR.Lambda`, `AlexaVoxCraft.Lambda`, `AlexaVoxCraft.Http`,
   `AlexaVoxCraft.InSkillPurchasing`, `AlexaVoxCraft.Smapi` — to be filled in **during** implementation as
   each package's task group completes and its validation-app coverage (Task Group 7) confirms it, not
   pre-filled speculatively here.
2. For each package, follow this **ordered, evidence-based checkpoint sequence** (not a temporal soak —
   every checkpoint is a concrete artifact to produce and inspect, gated on the previous checkpoint's
   findings being resolved or explicitly classified, not on elapsed time; per research §11 step 6's
   underlying two-step sequencing, restated here without temporal framing since this plan ships as one PR
   with no elapsed-time gap between steps):
   1. Enable trimming analysis first (`<IsTrimmable>true</IsTrimmable>`, `EnableTrimAnalyzer`) and capture
      its findings.
   2. Resolve or explicitly classify every finding (fix it, or record why it's a legitimate non-blocker —
      never silently ignored).
   3. Validate the package's actual runtime scenarios via Task Group 7's validation app (this is the
      evidence step — a clean analyzer run alone is not sufficient).
   4. Only after step 3 passes for that specific package does it earn
      `<IsAotCompatible>true</IsAotCompatible>` — never uniformly across the whole solution as a shortcut,
      per both the ADR and research §9's explicit warning against assuming simultaneous eligibility.
3. `AlexaVoxCraft.Observability`/`AlexaVoxCraft.MinimalLambda` are excluded from this matrix until Task
   Group 6's audit completes and is either clean or has its findings resolved.

**Testing:** each row's "eligible for `IsAotCompatible`" determination is itself gated on that package's
task group's tests (above) plus its coverage in Task Group 7's validation app passing.

**Open decisions:** none structurally — this task group is explicitly about filling in a matrix during
implementation, not a decision to make now.

---

## Task Group 9 — CI

Implements ADR "Validation / CI Requirements" (CI gate portion).

**Verified against current code:** `.github/workflows/pr-build.yaml` is a thin wrapper delegating to
`LayeredCraft/devops-templates/.github/workflows/pr-build.yaml@v10.5` (a reusable, externally-maintained
workflow) with `solution`, `hasTests`, `useMtpRunner`, `dotnetVersion` (8.0.x/9.0.x/10.0.x/11.0.x),
`runCdk: false` inputs — this repo does not hand-write its own build/test steps, it configures a shared
template. Other workflows present: `dependabot-auto-merge.yml`, `docs.yml`, `pr-title-check.yaml`,
`publish-preview.yaml`, `publish-release.yaml`, `release-drafter.yaml`.

**Changes:**
1. Because `pr-build.yaml` delegates to an external reusable workflow whose steps this repo does not
   control, the Native AOT validation gate cannot simply be inserted as extra steps inside that existing
   job. Add a **new**, separate workflow file (e.g. `.github/workflows/native-aot-validation.yaml`),
   triggered on `pull_request` to `main`, that runs independently of the templated `pr-build.yaml` job:
   build in Release, run trim/AOT analyzers on the in-scope packages (Task Group 8), publish Task Group
   7's validation application with `PublishAot=true` for `linux-x64` (and/or `linux-arm64` matching
   Lambda's `provided.al2023` target), execute the published native binary, and fail the job on any
   non-zero exit/assertion failure.
2. **Path filtering** must be careful not to under-scope: this repo's `pull_request` triggers currently
   have no `paths:` filter at all (`pr-build.yaml` runs on every PR to `main` unconditionally) — decide
   at implementation time whether the new AOT workflow should also run unconditionally (simplest, matches
   existing convention, avoids the exact under-scoping risk this task explicitly warns about) or use a
   `paths:` filter. If a filter is used, it must cover: `src/AlexaVoxCraft.Model/**`,
   `src/AlexaVoxCraft.Model.Apl/**`, `src/AlexaVoxCraft.Model.InSkillPurchasing/**`,
   `src/AlexaVoxCraft.MediatR/**`, `src/AlexaVoxCraft.MediatR.Generators/**`,
   `src/AlexaVoxCraft.MediatR.Lambda/**`, `src/AlexaVoxCraft.Http/**`,
   `src/AlexaVoxCraft.InSkillPurchasing/**`, `src/AlexaVoxCraft.Smapi/**`, the new validation app's own
   directory, and any shared build file (`Directory.Build.props`, `AlexaVoxCraft.slnx`) — a change to a
   shared props file must not silently bypass the gate. **Recommendation: no path filter (run on every
   PR), matching this repo's existing unconditional-trigger convention** and eliminating the
   under-scoping risk entirely, unless CI duration becomes a demonstrated problem during implementation.
3. Do not touch `pr-build.yaml`/the external reusable workflow itself — this is intentionally an
   additive, separate workflow, consistent with how this repo already keeps `docs.yml`,
   `pr-title-check.yaml`, etc. as independent jobs alongside the templated build.
4. This gate must run continuously (every qualifying PR going forward), not as a one-time validation —
   per ADR "The Native AOT compatibility claim is understood to require continuous verification after
   initial implementation, not a one-time badge."

**Testing:** the workflow's own correctness is validated by observing it actually run (and pass, and
fail-on-purpose when a deliberate regression is introduced during development, if the implementer wants
extra confidence) during this PR's own CI run.

**Open decisions:** unconditional trigger vs. path-filtered trigger (recommendation given above, final
call at implementation time); exact RID matrix (`linux-x64` only vs. both `linux-x64`/`linux-arm64`).

---

## Task Group 10 — Documentation / skill synchronization

Implements ADR "Documentation Contract".

**Verified against current code:** `docs/components/lambda-hosting.md:540-547` — confirmed exact current
content:
```xml
<PropertyGroup>
  <PublishTrimmed>true</PublishTrimmed>
  <TrimMode>copyused</TrimMode>
</PropertyGroup>
<ItemGroup>
  <TrimmerRootAssembly Include="AlexaVoxCraft.Model" />
  <TrimmerRootAssembly Include="AlexaVoxCraft.Model.Apl" />
</ItemGroup>
```
under a "### Bundle Size Optimization" heading. No dedicated "serialization documentation" file exists
today (grep of `docs/` found no `*serial*` doc); serialization is currently only discussed ad hoc inside
`docs/components/session-management.md` and `docs/examples/index.md`. `docs/components/` contains:
`apl-integration.md`, `in-skill-purchasing.md`, `lambda-hosting.md`, `observability.md`,
`pipeline-behaviors.md`, `request-handling.md`, `session-management.md`, `source-generation.md`.
`docs/smapi/developer-client.md` is the SMAPI doc. No `docs/api/` reference directory exists. No
migration-guide doc exists under `docs/`. No repo skill file with `evals.json` was found (`find . -iname
"evals.json"` returned nothing) — the ADR's conditional evals-update requirement does not apply today,
but note this explicitly rather than silently dropping the check.

**Consumer-facing vs. developer/architecture-level documentation — explicit distinction.** Internal
implementation types the ADR deliberately keeps non-public (`ModelContext`, `AplModelContext`,
`InSkillPurchasingModelContext`, `SmapiModelContext`) exist specifically so AlexaVoxCraft retains freedom
to restructure, split, or merge them later without a breaking change (ADR "Serialization Metadata
Ownership": "These context classes are never public"). Naming them in consumer-facing docs — README,
getting-started material, the new Native AOT guide, `apl-integration.md`, `in-skill-purchasing.md`,
`developer-client.md`, `session-management.md` — would implicitly promise their names/existence as part
of the public contract, undermining exactly the freedom the ADR preserves. Every change below that touches
consumer-facing documentation must describe **behavior**, not internal type names, e.g.:
- "`AplSupport.Add()` also wires the serialization metadata required by Native AOT" (not "registers
  `AplModelContext.Default`").
- "In-Skill Purchasing support supplies its package metadata automatically once `InSkillPurchasingSupport.
  Add()` runs" (not "via `InSkillPurchasingModelContext`").
- "SMAPI's default serialization is Native-AOT-safe out of the box, independent of whether any SMAPI DI
  extension has run" (not "backed by `SmapiModelContext.Default`").
- "Consumers register metadata only for their own types, through `RegisterTypeInfoResolver(...)`" (never
  naming an AlexaVoxCraft-internal context as something a consumer needs to know about).
Internal context class names **may** still appear in developer/architecture-level material where
implementation detail is appropriate and expected — e.g. this plan document itself, ADR-0001, code
comments/XML-doc inside the context classes' own source files, or a future internal design note — because
that audience is maintainers, not skill-building consumers. This distinction applies to every doc change
in this task group; it is not optional per-file discretion.

**Changes:**
1. `README.md` — add/update a Native AOT support statement (currently the README lists features and
   packages but makes no AOT claim either way; add one only once the claim is actually true, i.e. this
   task group's doc updates land alongside, not ahead of, the completed implementation). Behavior-only
   framing per the distinction above — no internal context names.
2. New doc: a Native AOT guide (suggested: `docs/components/native-aot.md`, matching the existing
   `docs/components/*.md` convention) covering: how to declare a consumer `JsonSerializerContext`, when
   it's required (Native AOT only, not JIT), what failure looks like if omitted (the Task Group 7
   scenario's exact exception shape), the JIT-fallback-vs-AOT-requirement distinction, and the custom
   `JsonSerializerOptions` ownership contract (ADR "Backward Compatibility" — AlexaVoxCraft never
   silently mutates a consumer-supplied options object). Describe AlexaVoxCraft's own generated metadata
   only in terms of what it does ("AlexaVoxCraft ships its own generated serialization metadata for every
   type it owns"), never by internal context class name.
3. `docs/components/lambda-hosting.md:535-547` — correct/remove the `TrimmerRootAssembly` exemption
   guidance now that `Model`/`Model.Apl` are actually trim-safe (per ADR, this exemption existed only
   because they weren't); replace with accurate `PublishTrimmed`/`PublishAot` guidance reflecting the new
   architecture.
4. `docs/components/apl-integration.md` — document, in behavior terms only (no `AplModelContext` name),
   that `AplSupport.Add()` also wires the serialization metadata required by Native AOT, and any
   consumer-facing implication for APL-heavy skills.
5. `docs/components/in-skill-purchasing.md` — same, in behavior terms only (no
   `InSkillPurchasingModelContext` name): `InSkillPurchasingSupport.Add()` supplies ISP's package
   metadata automatically.
6. `docs/smapi/developer-client.md` — document, in behavior terms only (no `SmapiModelContext` name):
   SMAPI's default serialization is Native-AOT-safe independent of DI init order for
   `InteractionModelBuilder.ToJson()`, and the open-generic guidance for
   `AlexaSkillInvocationClient.InvokeAsync<TRequest,TResponse>` (when a consumer needs their own
   registered context vs. when AlexaVoxCraft-owned types already cover it — ADR's "Deferred /
   Validation-Time Questions" item about real-world `InvokeAsync` usage patterns should inform how
   prominently this is flagged, once Task Group 7's validation app or real usage evidence clarifies it).
7. `docs/components/session-management.md` — update `JsonAttributeBag` guidance with the
   `RegisterTypeInfoResolver` consumer contract and the missing-metadata failure mode.
8. New API reference content for `AlexaJsonOptions.RegisterTypeInfoResolver(IJsonTypeInfoResolver)` —
   given no `docs/api/` directory exists today, decide at implementation time whether this belongs in the
   new native-aot.md guide (recommended, avoids inventing a new doc-structure convention this repo
   doesn't have) or a new dedicated API-reference file.
9. A migration guide section (likely inside the new native-aot.md doc, since no standalone migration-
   guide doc convention exists) explaining the JIT-fallback-vs-AOT-requirement distinction in
   consumer-facing terms, per ADR.
10. `CLAUDE.md` (this repo's own agent instructions) — update the "Project Overview"/"Architecture"
    sections and/or add a Native AOT section once the architecture is real, mirroring how the existing
    "OpenTelemetry Implementation TODO" section documents a completed cross-cutting initiative. Do this
    only as part of the same PR, once the work is actually done — do not describe unimplemented
    architecture as current fact. `CLAUDE.md` is repo/agent-facing guidance, not consumer-facing product
    documentation, so internal context class names may appear here where useful for future implementation
    work — the behavior-only constraint above applies specifically to consumer-facing docs (item list
    above, 1-2 and 4-7), not to this file.
11. Repo skill/`evals.json` check: **confirmed no `evals.json` file exists anywhere in this repository
    today** (verified via `find . -iname "evals.json"`). No eval-update action is required by this task
    group. Re-run the same `find` check at implementation time in case one was added between this
    plan's writing and implementation — if one exists then and its guidance touches serialization/AOT
    behavior, update it and compare against baseline per the ADR.

**Testing:** documentation has no automated test in this repo beyond `docs.yml`'s existing build/link
check (if any — confirm what `docs.yml` actually validates at implementation time); the "test" here is
a self-review pass confirming every code example in updated docs actually compiles/runs against the
Task-Group-7 validation app's real usage, not aspirational syntax.

**Open decisions:** exact new-file locations for the Native AOT guide and API reference content (both
recommended above but not fixed).

---

## Task Group 11 — Final full-suite + Native AOT acceptance validation

Closing checkpoint for the single PR, not a separate PR.

**Changes:** none — this is a validation-only checkpoint.

**Testing:**
1. Full existing unit-test suite green across all four TFMs (`net8.0;net9.0;net10.0;net11.0`), via
   `dotnet run` per Microsoft.Testing.Platform convention (CLAUDE.md).
2. Task Group 9's CI workflow passes: build, trim/AOT analyzers clean (or legitimately classified per
   Task Group 8's matrix, never suppressed), Task Group 7's validation app publishes and its native
   binary executes with correct output.
3. Every Completion Criteria item below (checklist) is satisfied.
4. Every Documentation Contract item (Task Group 10) is satisfied.
5. Full solution build (`dotnet build AlexaVoxCraft.slnx`) with no new warnings introduced relative to
   the baseline established in `docs/plans/0002-cloudwatch-driven-verify-tests-and-warning-cleanup.md`
   (that plan's warning-cleanup work is a separate, already-tracked effort — this plan must not
   regress it).

---

## Completion criteria checklist (ADR-0001 → this plan)

Pulled verbatim (condensed only for table formatting) from the ADR's "Completion Criteria" section:

| ADR completion criterion | Satisfied by |
|---|---|
| Package-owned JSON metadata is source-generated for `Model`, `Model.Apl`, `Model.InSkillPurchasing`, and `Smapi` | Task Groups 2, 3a, 3b, 3c |
| Consumer-owned metadata has a supported, documented resolver-registration contract (`RegisterTypeInfoResolver`) | Task Group 2 (API), Task Group 10 (docs) |
| The JIT compatibility fallback is correctly feature-gated (confirmed absent from `PublishAot` output) | Task Group 2 (implementation), Task Group 7 (empirical proof via native binary) |
| Existing JSON modifiers/converters (`RegisterConverter<T>`, `RegisterTypeModifier<T>`, `ShouldSerialize`) preserve current semantics exactly | Task Group 2 |
| Package polymorphism (`RequestConverter`, `BasePolymorphicConverter<T>` subclasses, `APLComponentConverter`, `RegisterDirectiveDerivedType<T>`) retains current wire behavior | Task Groups 2, 3 (unchanged converters, verified via Verify snapshots) |
| APL generic converter factories no longer require dynamic generic activation on the supported Native AOT path | Task Group 4 |
| Pre-existing `APLValue<Component>` bug corrected before the final closed APL generic type map is established | Task Group 1 (must precede Task Group 4) |
| Mediator runtime dispatch (`SkillMediator.Send`) no longer requires dynamic generic activation on the supported Native AOT path | Task Group 5 |
| `Http`, `InSkillPurchasing`, and `Smapi` runtime serialization is AOT-safe | Task Group 3 (3c, 3d) |
| Trim/AOT analyzer findings resolved or legitimately classified (not suppressed to manufacture a clean claim) | Task Groups 6, 8 |
| A real, rooted Native AOT consumer application publishes and executes correctly | Task Group 7 |
| Consumer-owned metadata registration works end to end in that application | Task Group 7 |
| Missing consumer metadata fails clearly and immediately under Native AOT | Task Group 7 (dedicated scenario) |
| Documentation is complete | Task Group 10 |
| CI continuously enforces the compatibility claim, not merely at the moment it was first made | Task Group 9 |
| `IsAotCompatible=true` set only for packages whose compatibility has actually been demonstrated, never speculatively/uniformly | Task Group 8 |
