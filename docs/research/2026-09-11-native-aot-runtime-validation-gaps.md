# Native AOT Runtime Validation Gaps and Consumer-Reproduced Failures

Status: research only — no ADR, no implementation plan yet. Grounds a follow-up implementation plan in evidence gathered from the codebase, the two filed GitHub issues, and reconstruction of what the existing Native AOT validation actually executes (as opposed to what it was intended or documented to prove).

Date: 2026-09-11

Related: [ADR-0001 — Native AOT Compatibility](../adr/0001-native-aot-compatibility.md) (accepted), [Plan 0003 — Native AOT Compatibility Implementation](../plans/0003-native-aot-compatibility-implementation.md) (executed, merged as #188), [2026-09-08 Native AOT Compatibility Research](2026-09-08-native-aot-compatibility-research.md) (the evidence base for ADR-0001).

## 1. Triggering consumer findings

`trivia-platform`, a real downstream consumer, built a genuine self-contained Native AOT `linux-arm64` binary on a real Amazon-Linux-2023-compatible build image and deployed it to a real `provided.al2023` AWS Lambda function. The function built cleanly (`PublishAot=true`, `EnableAotAnalyzer`/`EnableTrimAnalyzer`), deployed, cold-started successfully, and reached real Alexa request handling — then failed on **every** invocation with:

```
System.NotSupportedException: JsonTypeInfo metadata for type 'AlexaVoxCraft.Model.Apl.APLSkillRequest'
was not provided by TypeInfoResolver of type 'System.Text.Json.Serialization.Metadata.JsonTypeInfoResolverWithAddedModifiers'.
   at System.Text.Json.JsonSerializer.Deserialize[TValue](Stream, JsonSerializerOptions)
   at AlexaVoxCraft.Lambda.Serialization.AlexaLambdaSerializer.Deserialize[T](Stream)
   at MinimalLambda.DefaultEventFeature`1.GetEvent(ILambdaInvocationContext)
```

Separately, a downstream consumer's own analyzer-enabled build (`-p:EnableAotAnalyzer=true -p:EnableTrimAnalyzer=true`) surfaced `IL2026`/`IL3050` warnings against the generated `AddSkillMediator` interceptor, attributed to `ConfigurationBinder.Bind(IConfiguration, object)`.

Both findings were filed as GitHub issues against `LayeredCraft/alexa-vox-craft`:

- **[#190](https://github.com/LayeredCraft/alexa-vox-craft/issues/190)** — Native AOT deserialization failure: `APLSkillRequest` missing `JsonTypeInfo` metadata. Open, no fix merged. (#192 closed as duplicate.)
- **[#191](https://github.com/LayeredCraft/alexa-vox-craft/issues/191)** — `AddSkillMediator` and related config-binding paths use reflection-based `ConfigurationBinder.Bind`, producing `IL2026`/`IL3050` under Native AOT. Open, no fix merged. (#193 closed as duplicate.)

No other open AOT-related issues exist in the repo. A stray reference in #190's timeline to "PR #233" does not resolve (`gh pr view 233` fails) — likely a stale/external artifact; not relied on here.

Both issue bodies already contain unusually complete source-level root-cause analysis (file:line citations, suggested fixes, acceptance criteria). Independent source tracing below **confirms** every claim in both issues via direct reading of `main@3245d95` — the issues are not being taken on faith.

## 2. Issue 1 root cause — `APLSkillRequest` has no `JsonTypeInfo` anywhere

Confirmed by direct reading of `src/AlexaVoxCraft.Model.Apl/APLSkillRequest.cs`, `src/AlexaVoxCraft.Model.Apl/Serialization/AplModelContext.cs`, `src/AlexaVoxCraft.Model/Serialization/ModelContext.cs`, `src/AlexaVoxCraft.Model/Serialization/AlexaJsonOptions.cs`, and both `AlexaLambdaSerializer` implementations.

**The type.** `APLSkillRequest` (`src/AlexaVoxCraft.Model.Apl/APLSkillRequest.cs:6`) is a plain subclass of `SkillRequest` that shadows `Context` with `new APLContext Context`. It is the **only** subclass of `SkillRequest` in the entire library (`grep -rn ": SkillRequest"` across `src/` returns exactly this one hit). It is **not** part of any `[JsonPolymorphic]`/`[JsonDerivedType]` hierarchy — `SkillRequest` (`src/AlexaVoxCraft.Model/Request/SkillRequest.cs:10`) carries no such attributes. Polymorphism in this codebase is confined to the envelope's *inner* properties (`Request`, `Directive`, `Card`, `OutputSpeech`), not to the envelope type itself. `APLSkillRequest` is a **root type** a consumer passes directly to `JsonSerializer.Deserialize<T>` / `AlexaLambdaSerializer.Deserialize<T>` — never a polymorphic dispatch target.

**The gap.** Neither `ModelContext` (`src/AlexaVoxCraft.Model/Serialization/ModelContext.cs:14`, which registers `SkillRequest` — the base type only) nor `AplModelContext` (`src/AlexaVoxCraft.Model.Apl/Serialization/AplModelContext.cs:29-218`, which exhaustively lists ~190 `[JsonSerializable]` entries for every other APL component/command/directive/request-payload type) contains `[JsonSerializable(typeof(APLSkillRequest))]`. A repo-wide grep for `APLSkillRequest` finds only the type's own definition, test files, and sample files — **zero** `[JsonSerializable]` registrations anywhere.

**The call site.** `T` at the failing call is the concrete type, not the envelope base. `samples/Sample.Apl.Function/Function.cs:10` — `Function : AlexaSkillFunction<APLSkillRequest, SkillResponse>` — is the documented shape any APL-capable consumer (including trivia-platform) follows; `LambdaHostExtensions.RunAlexaSkill<Function, APLSkillRequest, SkillResponse>()` ultimately drives `AlexaLambdaSerializer.Deserialize<APLSkillRequest>(stream)`. So the missing metadata is for the **generic call itself** — the simplest, most severe form of this class of gap, not a nested polymorphic-dispatch failure.

**Why it throws under AOT specifically, not under JIT.** `AlexaJsonOptions.CreateOptions()` (`src/AlexaVoxCraft.Model/Serialization/AlexaJsonOptions.cs:69-72`) appends a JIT reflection fallback (`DefaultJsonTypeInfoResolver`) **only if** `JsonSerializer.IsReflectionEnabledByDefault`. That flag is `true` in every normal test host and `false` under `PublishAot`/trimmed publish. So under JIT (including every existing xUnit test in the repo), an unregistered type like `APLSkillRequest` silently resolves via reflection and everything appears correct. Under Native AOT, the same `AlexaJsonOptions.DefaultOptions` object has no fallback, no metadata for the root type, and throws exactly the reported `NotSupportedException`.

**Registration ordering is not the bug.** `APLSupport.Add()` (`src/AlexaVoxCraft.Model.Apl/APLSupport.cs:17-19`) is an explicit, non-automatic static call a consumer must make once at startup to register `AplModelContext.Default` via the internal `AlexaJsonOptions.RegisterPackageTypeInfoResolver`. `AlexaSkillFunction<TRequest,TResponse>.Start()` freezes `AlexaJsonOptions.DefaultOptions` at construction time; as long as `APLSupport.Add()` runs before `RunAlexaSkill<...>()` (as the shipped sample and trivia-platform both do), ordering is correct. The defect is purely that `AplModelContext` — the correct, already-in-scope owning context for this assembly's types — never lists `APLSkillRequest` as a root, regardless of when it's registered.

**Both Lambda serializer implementations are affected identically.** `src/AlexaVoxCraft.MediatR.Lambda/Serialization/AlexaLambdaSerializer.cs` and `src/AlexaVoxCraft.Lambda/Serialization/AlexaLambdaSerializer.cs` are near-duplicate classes, both defaulting to `AlexaJsonOptions.DefaultOptions` when no options are explicitly supplied. `AlexaVoxCraft.MinimalLambda`'s `ServiceCollectionExtensions.cs:41-42` wires the same pair. All three hosting paths (`MediatR.Lambda`, `Lambda`, `MinimalLambda`) share the exact same gap because they all share the one process-wide `AlexaJsonOptions.DefaultOptions`.

### Scope: is this isolated or representative?

Isolated as a *type inventory* gap — `APLSkillRequest` is the only `SkillRequest` subclass in the library, so there is no family of sibling types with the identical defect today. But the underlying **pattern** — a library-owned request-envelope root type never independently exercised through the real Lambda serializer in any test or validation artifact — is structural, not incidental (§4). A future consumer-visible root type (e.g. a new specialized request envelope added to Model or Model.Apl) could reintroduce an equivalent gap unless validation coverage changes (§8).

## 3. Issue 2 root cause — reflection-based `ConfigurationBinder.Bind` at four call sites, plus a diagnostic-visibility blind spot

Confirmed by direct `dotnet build` reproduction of the warnings (not just static reading) against `AlexaVoxCraft.MediatR.csproj` and `AlexaVoxCraft.Smapi.csproj` (both `IsAotCompatible=true`, which turns on `EnableAotAnalyzer`/`EnableTrimAnalyzer`).

**Exact diagnostics, reproduced live:**

```
warning IL2026: Using member 'Microsoft.Extensions.Configuration.ConfigurationBinder.Bind(IConfiguration, Object)' which has
'RequiresUnreferencedCodeAttribute' can break functionality when trimming application code. Cannot statically analyze the
type of instance so its members may be trimmed.
warning IL3050: Using member 'Microsoft.Extensions.Configuration.ConfigurationBinder.Bind(IConfiguration, Object)' which has
'RequiresDynamicCodeAttribute' can break functionality when AOT compiling. Binding strongly typed objects to configuration
values requires generating dynamic code at runtime, for example instantiating generic types.
```

**Four real call sites**, all binding flat, trivially-bindable POCOs (`SkillServiceConfiguration`: three `string?`, a `ServiceLifetime` enum, an `int`; `SmapiDeveloperAccessTokenOptions`: three `string`):

| File:Line | Code | Reachable from documented entry point? |
|---|---|---|
| `src/AlexaVoxCraft.MediatR/DI/ServiceCollectionExtensions.cs:14` | `configuration.GetSection(sectionName).Bind(serviceConfig);` | Yes — the non-generated fallback path for `AddSkillMediator`, reached when the interceptor doesn't run |
| `src/AlexaVoxCraft.MediatR/DI/ServiceCollectionExtensions.cs:23` | `configuration.GetSection(sectionName).Bind(opt);` | Same |
| `src/AlexaVoxCraft.Smapi/ServiceCollectionExtensions.cs:49` | `configuration.GetSection(sectionName).Bind(options)` inside `AddSmapiDeveloperClient` | Yes, documented configuration-driven overload |
| `src/AlexaVoxCraft.Smapi/ServiceCollectionExtensions.cs:143` | Same shape, inside `AddSkillInvocationClient` | Yes, documented configuration-driven overload |

A fifth site, `src/AlexaVoxCraft.MediatR.Generators/Generators/InterceptorEmitter.cs:48,54`, emits the **identical** `configuration.GetSection(sectionName).Bind(cfg)` / `.Bind(opt)` source text into the generated interceptor that backs the *documented, primary* `AddSkillMediator(configuration, cfg => ...)` call every consumer uses — so this is the mainline path, not a fallback.

**Why the interceptor path produces no warning even though it runs the same reflection call.** `InterceptorEmitter.cs:15` prefixes the generated file with `// <auto-generated />`. Roslyn's standard "generated code" heuristic recognizes that marker and the AOT/trim analyzers, like most analyzers, suppress diagnostics inside files so recognized — by default, not via any explicit suppression AlexaVoxCraft added. So the exact same reflection/dynamic-code-dependent call:

- **warns**, correctly, when hand-written in `ServiceCollectionExtensions.cs` (not marked generated), and
- **produces zero warning**, incorrectly (from a "does this represent real risk" standpoint), when emitted verbatim into the `<auto-generated />`-marked interceptor file — even though it is the exact same call, executed on every cold start of every consumer using the documented API.

This is a genuine blind spot, not a false positive to suppress: the interceptor file hides a real `RequiresUnreferencedCode`/`RequiresDynamicCode` risk from the analyzer precisely because it looks like generated boilerplate to Roslyn's generic heuristic.

**Could the Configuration Binder source generator fix this as a drop-in?** Partially:
- For the two hand-written `ServiceCollectionExtensions.cs` sites (both packages): yes in principle — Microsoft's `ConfigurationBindingSourceGenerator` (ships in `Microsoft.Extensions.Configuration.Binder`, already referenced) can intercept `.Bind()`/`.Get<T>()` for statically-known simple types like these. It simply isn't enabled/exercised today (no `EnableConfigurationBindingGenerator`, no manual replacement).
- For `InterceptorEmitter.cs`'s emitted calls: **no** — Roslyn generators cannot see or rewrite another generator's output within the same compilation pass, so Microsoft's config-binding interceptor generator structurally cannot intercept a `.Bind()` call that is itself synthesized text from `AlexaVoxCraft.MediatR.Generators`. The only real fix here is for `InterceptorEmitter` to emit direct, reflection-free property assignments itself (`cfg.SkillId = configuration[$"{sectionName}:SkillId"]`, `cfg.Lifetime = configuration.GetValue<ServiceLifetime>(...)`, etc.) — mechanical, since the generator already has full compile-time knowledge of `SkillServiceConfiguration`'s fixed shape and doesn't need a general-purpose binder at all.

**Reachability confirms this is not cosmetic.** `AddSkillMediator(IConfiguration, ...)` is CLAUDE.md's documented canonical entry point — not optional, not rare. The Smapi `IConfiguration` overloads are the documented configuration-driven convenience overloads (vs. the clean `Action<Options>` overloads).

### Equivalent binder paths elsewhere

None. A full grep of `Observability`, `Http`, `InSkillPurchasing`, `MediatR.Lambda`, `Lambda` for `.Bind(`/`.Get<`/`ConfigurationBinder` found no other occurrences. The pattern is confined to the four sites above plus the generator that mirrors two of them — a **localized, enumerable set**, not a pervasive pattern requiring a library-wide sweep.

## 4. Previous validation reconstruction

Confirmed by direct reading of `test/AlexaVoxCraft.NativeAot.ValidationApp/Program.cs`, `.github/workflows/native-aot-validation.yaml`, `docs/adr/0001-native-aot-compatibility.md`, and `docs/plans/0003-native-aot-compatibility-implementation.md`.

The validation app runs 8+ scenarios as a real, published, executed Native AOT binary (`PublishAot=true`, `linux-x64`, reflection disabled), gated in CI by `.github/workflows/native-aot-validation.yaml`. It is not a sham — Model JSON, Lambda-serializer round-trip (for `SkillRequest`/`LaunchRequest`), MediatR dispatch, MediatR configuration binding (with a minimal in-memory config), SMAPI, and ISP are all genuinely exercised through real public APIs with a real executed native binary. This is legitimate evidence for everything it actually touches.

**What it does *not* touch, and why that gap is exactly issue #190's shape:**

- **Scenario 2** (`Program.cs:87-104`) exercises APL JSON only in the *response* direction — a synthetic `RenderDocumentDirective`, serialized/deserialized as `IDirective`. It never constructs or deserializes `APLSkillRequest`.
- **Scenario 8** (`Program.cs:227-241`), the one scenario that does use the real `AlexaLambdaSerializer`, round-trips only `SkillRequest`/`LaunchRequest` — never `APLSkillRequest`.
- Neither `docs/adr/0001-native-aot-compatibility.md` (which names, at most, an APL `UserEventRequest` as an example type to exercise) nor `docs/plans/0003-…md:851` (which explicitly directs "deserialize a `UserEventRequest`") ever names `APLSkillRequest` — the actual public request-envelope type a consumer deserializes incoming Alexa JSON into — as something to exercise. This is a spec-level gap, not just an implementation shortfall: even a validation app that faithfully executed everything the plan asked for would not have caught this, because `UserEventRequest` (unlike `APLSkillRequest`) *is* correctly registered in `AplModelContext.cs:202`.
- The implemented app additionally deviated from even that (insufficient) plan: it substituted a response-direction directive round-trip for the plan's specified request-direction `UserEventRequest` deserialize.

**Rooting vs. registration — the conflation that let this ship.** `AlexaVoxCraft.Model.Apl` is `ProjectReference`d and thus rooted in the validation app's csproj (`TrimmerRootAssembly`-equivalent via direct reference), so the linker does not strip `APLSkillRequest` from the published binary. That is a necessary but wholly separate property from "the type has a `JsonTypeInfo` entry in the composed resolver chain." The validation app, CI, and the governing docs never draw this distinction explicitly — "the assembly is rooted" was treated as sufficient evidence of AOT-safety for everything in that assembly, when it is only evidence the linker didn't delete it.

**The unit test suite independently masks the same gap.** `test/AlexaVoxCraft.Model.Apl.Tests/APLSkillRequestTests.cs` deserializes `APLSkillRequest` successfully — but as a normal xUnit test host, `JsonSerializer.IsReflectionEnabledByDefault` is `true` there, so `AlexaJsonOptions.CreateOptions()`'s JIT fallback (§2) silently resolves the type even though it was never registered. The exact same `AlexaJsonOptions.DefaultOptions` object, the exact same code path, produces a passing test and a production crash purely because of one ambient runtime flag. **No existing test — unit test or validation-app scenario — actually proves what "AOT-compatible JSON handling" is supposed to mean**, because the one test that exercises the failing type does so in an environment where the failure mode is structurally unreachable.

**A pre-existing completeness test cannot catch this class of gap either.** `test/AlexaVoxCraft.Model.Apl.Tests/Serialization/AplModelContextCompletenessTests.cs` (`AplModelContext_HasMetadata_ForEveryPolymorphicDispatchTarget`) only walks types reachable through `BasePolymorphicConverter<T>.DerivedTypes` — i.e., types dispatched to *polymorphically* from an already-known parent. `APLSkillRequest` is never a polymorphic dispatch target (§2) — it's passed directly as `T` to `Deserialize<T>` — so this completeness test is structurally blind to root-type omissions, not just to this one instance.

**MediatR configuration binding was "validated" in a way that could never surface issue #191.** The validation app's Scenario 5 does call `AddSkillMediator(configuration, cfg => {...})` and does exercise `SkillMediator.Send`, but the app is a **runtime smoke test** — it checks that binding succeeds, not that the mechanism used to bind is trim-safe. It never builds with `EnableAotAnalyzer`/`EnableTrimAnalyzer` as a pass/fail gate against its own source (only as compile flags on the *library* projects it references, not enforced on/via the app's own execution). And separately, the interceptor-file `<auto-generated />` blind spot (§3) means that even if the validation app's own analyzer output were checked, the interceptor-emitted `.Bind()` call would still not appear as a warning there either — it's invisible to the analyzer regardless of which project consumes it.

**Smapi's config-binding call sites are entirely unexercised.** `grep -n "AddSmapiDeveloperClient\|AddSkillInvocationClient" test/AlexaVoxCraft.NativeAot.ValidationApp/Program.cs` returns nothing. `AlexaVoxCraft.Smapi` is referenced (rooted) but its `IConfiguration`-based DI extension methods are never called by the validation app at all — the two Smapi warning sites (§3) have **zero** validation coverage, not even a masked/false-negative kind.

### Coverage matrix

| Production capability | Analyzer covered | Native publish covered | Native execution covered | Actual public API path exercised | Realistic payload |
|---|---|---|---|---|---|
| Model JSON (`SkillRequest`/`LaunchRequest`) | Yes | Yes (rooted) | Yes — `Program.cs:67` | Direct `JsonSerializer.Deserialize` w/ `AlexaJsonOptions.DefaultOptions` | Yes, realistic envelope |
| APL JSON — response direction (components/directives) | Yes | Yes (rooted) | Yes — `Program.cs:88-104` | `APLSupport.Add()` + raw `Serialize/Deserialize<IDirective>` | Synthetic `RenderDocumentDirective` only |
| **APL JSON — request envelope (`APLSkillRequest`)** | **No** | **No** (rooted ≠ registered) | **No** — never constructed/deserialized | **Never exercised** | **None** |
| Lambda serializer deserialize | Yes | Yes | Yes — `Program.cs:228-240`, real `AlexaLambdaSerializer` | Real public entry point | `SkillRequest`/`LaunchRequest` only — never `APLSkillRequest` |
| Lambda serializer serialize | Yes | Yes | Yes | Real public entry point | Plain `SkillRequest` |
| MediatR handler dispatch | Yes | Yes | Yes — `Program.cs:135-167` | Real `ISkillMediator.Send`, generator-produced keyed dispatch | LaunchRequest only |
| MediatR configuration binding — runtime success | Yes (as build-succeeds) | Yes | Yes — real `IConfiguration`/`AddSkillMediator` | Real API | Minimal in-memory config, single key |
| **MediatR configuration binding — AOT/trim safety of the mechanism** | **No** (masked by `<auto-generated />` heuristic on the interceptor path) | N/A (warning is compile-time) | N/A | Real API, but warning invisible regardless | N/A |
| **Smapi configuration binding (`AddSmapiDeveloperClient`/`AddSkillInvocationClient`)** | **No** — never called by validation app | Assembly rooted only | **No** | **Never exercised** | **None** |
| HTTP client (base) | Yes | Yes | Yes (indirectly via ISP/SMAPI below) | — | — |
| SMAPI (interaction model, invocation client) | Yes | Yes | Yes — `Program.cs:191-221` | Real client methods | Fake `HttpMessageHandler`, consumer-owned type |
| ISP | Yes | Yes | Yes — `Program.cs:174-183` | Real client methods | Fake handler, empty product list |

"Rooted" means the linker preserved the assembly/type in the published binary — it says nothing about whether that type has a `JsonTypeInfo` entry in the resolver chain actually used at the real call site. Every row above marked "No" was previously implicitly treated as covered because its containing assembly was rooted; distinguishing rooting from registration, and registration from actual invocation through the real public API with a realistic payload, is the core corrective this research proposes carrying forward (§7).

## 5. Exact reason Issue 1 escaped validation

Two independent, both-necessary facts, each verified directly in code (not inferred):

1. **The library gap is real**: `APLSkillRequest` genuinely has no `[JsonSerializable]` registration in `ModelContext` or `AplModelContext` — not a test artifact, not a validation-app oversight alone.
2. **Every place that could have caught it structurally could not**: the validation app's plan (ADR-0001, Plan 0003) never named `APLSkillRequest` as a type to exercise, and even a faithful implementation of what the plan *did* ask for (`UserEventRequest`) would not have surfaced this specific gap, because that type is correctly registered. The implemented app additionally substituted a response-direction round-trip for the plan's specified request-direction deserialize. Separately, the unit test that does deserialize `APLSkillRequest` runs under JIT, where `AlexaJsonOptions`'s reflection fallback (§2) makes the missing registration invisible. The one test that touches the failing type does so in the one environment where the failure cannot manifest.

Ranked against the examples posed in the investigation brief:
- *"Validation exercised a base request type but not the concrete APL request type"* — **confirmed**, this is the primary mechanism (§4, Scenario 8).
- *"The test payload did not cause the same polymorphic/concrete type path"* — not applicable; `APLSkillRequest` isn't reached polymorphically at all (§2), so this framing doesn't apply, but the adjacent truth — the validation payload never reached this concrete root type — is confirmed.
- *"Generated context existed but runtime initialization ordering was not represented"* — **ruled out**; ordering is correct wherever exercised (§2), the context simply never lists the type regardless of ordering.
- *"Validation rooted Model.Apl but did not exercise its registration path"* — **confirmed and is the single clearest one-sentence summary of the whole bug** (§4).
- *"The consumer setup differs from the validation application's setup"* — not the cause; trivia-platform's setup (`APLSupport.Add()` before `RunAlexaSkill`) matches the documented/sample pattern exactly.

## 6. Reason Issue 2 escaped validation

- The validation app's MediatR scenario proves runtime binding *succeeds*, not that the binding *mechanism* is AOT/trim-safe — those are different claims, and only the first was checked.
- Even if the app's own build were gated on `EnableAotAnalyzer`/`EnableTrimAnalyzer`, the interceptor-emitted `.Bind()` call is invisible to the analyzer because `InterceptorEmitter.cs:15` marks its output `<auto-generated />`, and Roslyn's analyzer infrastructure suppresses diagnostics in files matching that heuristic by default (§3) — a genuine, previously-unrecognized blind spot, not something CI configuration alone could have fixed without also addressing the generator's output.
- The Smapi call sites have no validation-app coverage at all — not masked, simply never invoked.

## 7. Audit of potentially equivalent paths

Explicitly checked and found clear, beyond the two filed issues:

- **JSON root-type registration completeness**: `APLSkillRequest` is the only unregistered root type found; the completeness test's blind spot (only checks polymorphic dispatch targets, §4) means this class of gap is not mechanically prevented for any *future* root type either — this is addressed as a validation-contract change (§8), not by hunting for more instances today, since none were found.
- **Reflection-based configuration binding**: confined to the four sites plus the generator mirror identified in §3; no other package uses `.Bind`/`.Get<T>`/`ConfigurationBinder`.
- **`<auto-generated />` diagnostic-suppression blind spot**: `InterceptorEmitter.cs` is the only generator in the repo (`AlexaVoxCraft.MediatR.Generators` is the sole source generator project) that could reproduce this pattern; no other generated-file emission was found to carry a risky reflection call in the same way.
- **Rooting-vs-registration conflation**: this is a systemic risk to the validation architecture itself (any future AlexaVoxCraft-owned root type added to Model/Model.Apl/ISP/Smapi contexts could reintroduce the same class of gap) rather than a second concrete instance today. It is the primary target of the validation-contract redesign below.

## 8. Proposed validation contract

The validation app should prove the supported public/runtime boundaries that matter under Native AOT — not exhaustively test every line. Concretely, it must be able to answer, for each AlexaVoxCraft-owned root type crossing a serialization boundary at a documented public entry point: **"has this exact type, through this exact public API, with a realistic payload, been deserialized/serialized in an executed Native AOT binary with reflection disabled?"** — and the answer must not depend on rooting alone.

### Architecture decision: fixture-driven dedicated executable, not Native-AOT-published xUnit

This is validation **added on top of** the existing xUnit suites, not a replacement for them. The JIT unit tests keep validating detailed object/converter/snapshot behavior exactly as they do today, unchanged — they remain the fast, primary correctness net. The Native AOT validator adds a second, narrower layer whose only job is proving those same trusted payloads still work when routed through the real production serializer entry points in a reflection-disabled, published Native AOT binary — a property the JIT suite structurally cannot prove (§4 — `AlexaJsonOptions`'s JIT reflection fallback makes JIT test runs blind to missing source-generated metadata).

Two candidate shapes were evaluated for how the "realistic payload" requirement above gets satisfied:

**Option A — publish and run the existing xUnit test suites themselves under Native AOT.** Rejected. This would pull the entire test framework (xUnit v3 runner, AutoFixture/Compono, AwesomeAssertions, Verify snapshot infrastructure) into the AOT/trim compatibility surface — none of which is part of AlexaVoxCraft's product surface and none of which is validated or claimed to be Native AOT compatible today. It would multiply the validation app's build/publish time and failure surface by every existing unit test (hundreds), most of which test detailed object/converter behavior that has nothing to do with AOT-specific risk (reflection fallback, resolver composition, trimming). It also risks false confidence: a test framework quirk under AOT could mask or be confused with a real library regression, and a real library regression could be masked by test-framework behavior differing from a real Lambda's minimal hosting environment.

**Option B (chosen) — keep a small, dedicated Native AOT validation executable, but source its payloads from the same trusted fixture files the existing JIT test suites already use, rather than inventing new ones.** This was the status quo's actual weakness: the existing `test/AlexaVoxCraft.NativeAot.ValidationApp/Program.cs` invents every payload from scratch — 6 of its 8 scenarios build objects in code and serialize them (never deserializing real wire JSON at all), and the 2 that do use JSON text use short hand-written inline string literals disconnected from anything the test suite trusts (confirmed by direct reading of every scenario, §4-note below). This is precisely how a fixture as important as a real APL request envelope was never exercised — the validation app's authors had to remember to hand-write a realistic APL payload from scratch, and didn't.

Fixture inventory (confirmed by direct repo inspection): the test suites already maintain a small, trusted, realistic JSON fixture corpus — roughly 60 files total across `test/AlexaVoxCraft.Model.Tests/Examples/`, `test/AlexaVoxCraft.Model.Apl.Tests/Examples/`, and `test/AlexaVoxCraft.Model.InSkillPurchasing.Tests/Examples/` — embedded as `<EmbeddedResource Include="Examples\**\*.json"/>` and loaded via a shared `Fx(relativePath)` helper (`test/AlexaVoxCraft.Model.Tests/TestBase.cs`, mirrored in Model.Apl.Tests) that reads them via `Assembly.GetManifestResourceStream`. This set is small enough that curation is a convenience, not a necessity — nothing prevents running most/all request-deserialization fixtures through the validator cheaply. Critically, `test/AlexaVoxCraft.Model.Apl.Tests/APLSkillRequestTests.cs` already deserializes `Examples/Requests/UserTouchRequest.json` and `Examples/Requests/APLUserEvent_Answer.json` directly into **`APLSkillRequest`** under JIT — these are exactly the fixtures needed to reproduce and regression-guard Issue 1, already proven realistic by an existing, trusted JIT test.

**Why fixture-driven wins over AOT-running xUnit, concretely:**
- Keeps the test framework itself out of the AOT compatibility surface — the validator only needs to be as capable as a real Lambda consumer, never more.
- Reuses payloads already proven realistic (they're what the JIT tests trust today) instead of a second, divergent, hand-maintained payload set that can silently drift from what real Alexa sends.
- Exercises the actual production serializer entry point (`AlexaLambdaSerializer.Deserialize<T>`), not `JsonSerializer.Deserialize` called directly with validation-only options — closing the exact gap in §4's coverage matrix.
- Stays small, deterministic, and fast — a native process with explicit scenarios and fail-fast assertions, not a second unit-test framework.
- Fixture reuse has no existing precedent for cross-project file linking in this repo (confirmed: zero `Link=` usages anywhere in any `.csproj`), but the established local idiom (`EmbeddedResource Include="Examples\**\*.json"`) extends cleanly to a cross-project reference:
  ```xml
  <ItemGroup>
    <!-- Reuse trusted request fixtures from the JIT test suites; do not duplicate them -->
    <EmbeddedResource Include="..\AlexaVoxCraft.Model.Tests\Examples\Requests\LaunchRequest.json" Link="Examples\LaunchRequest.json" />
    <EmbeddedResource Include="..\AlexaVoxCraft.Model.Tests\Examples\Requests\IntentRequest.json" Link="Examples\IntentRequest.json" />
    <EmbeddedResource Include="..\AlexaVoxCraft.Model.Apl.Tests\Examples\Requests\UserTouchRequest.json" Link="Examples\UserTouchRequest.json" />
    <EmbeddedResource Include="..\AlexaVoxCraft.Model.Apl.Tests\Examples\Requests\APLUserEvent_Answer.json" Link="Examples\APLUserEvent_Answer.json" />
  </ItemGroup>
  ```
  Embedding (not `<None>`/`<Content>` + `CopyToOutputDirectory`) is the deliberate choice, not just convention-matching: the CI workflow (`.github/workflows/native-aot-validation.yaml:54`) invokes the published native binary directly by full path with the **repo root as working directory**, not the publish output folder — a `CopyToOutputDirectory` file loaded via a relative path at runtime would silently fail to resolve under that CWD. An embedded resource is compiled into the assembly and is read via `Assembly.GetManifestResourceStream`, so it has zero dependency on publish layout, working directory, or any runtime file I/O succeeding under a trimmed AOT binary — the same reasoning that already makes it the repo's standard for the JIT test suites applies unchanged here.
- No response-JSON fixtures exist anywhere in the repo (confirmed — the test suites validate responses via Verify snapshots of code-constructed objects, never by deserializing response wire JSON). The validator's response-serialization scenarios should therefore continue constructing `SkillResponse`/`RenderDocumentDirective` objects in code and serializing them, exactly as the current app already does — there is no trusted response fixture corpus to switch to, and none needs inventing for this fix.

Coverage caveat, stated explicitly per the investigation's own instruction not to overstate what execution proves: because the validator constructs `SkillResponse` objects in code rather than deserializing a trusted response fixture, it proves "serialization of a code-constructed response succeeds," not "a realistic response payload round-trips" — an intentionally narrower claim than the request side now gets. This asymmetry is accepted as correct scope, not deferred as a gap, since no trusted response fixture corpus exists to strengthen it against.

### Lambda request/response round trip (the direct fix for Issue 1's validation hole)

Add a scenario that mirrors the real consumer entry point exactly, using the fixtures identified above:
1. Call `APLSupport.Add()` (the documented, required consumer step for APL).
2. Deserialize the embedded `LaunchRequest.json`/`IntentRequest.json` fixtures into `SkillRequest` through the real `AlexaLambdaSerializer.Deserialize<T>` (already covered in spirit by the existing Scenario 8 — replace its hand-built object with these real fixtures so it's provably reusing trusted payloads, not just asserting round-trip self-consistency).
3. Deserialize the embedded `UserTouchRequest.json` (and, for broader surface, `APLUserEvent_Answer.json`, which additionally exercises `Viewport`/`AplVisualContext`) into **`APLSkillRequest`** through the same real `AlexaLambdaSerializer.Deserialize<T>` — the gap this research closes, using the exact fixture the JIT `APLSkillRequestTests.cs` already trusts.
4. Assert the deserialized `APLSkillRequest.Request` is the expected concrete type (`UserEventRequest`) — a semantic assertion consistent with what the JIT test already checks, not a raw JSON/property-ordering comparison.
5. Dispatch the result through the actual MediatR pipeline (`ISkillMediator.Send`).
6. Serialize the resulting `SkillResponse` back out through the same serializer (code-constructed response, per the caveat above).

This directly targets the failure mode: same public API, same process-wide `AlexaJsonOptions.DefaultOptions`, same reflection-disabled execution as a real Lambda, same trusted payload the JIT suite already relies on.

**Reproducing the exact AWS failure, and confirming the old validator would have passed anyway.** Running `UserTouchRequest.json` through `AlexaLambdaSerializer.Deserialize<APLSkillRequest>` in a published, executed, reflection-disabled Native AOT binary reproduces the identical `NotSupportedException` reported by trivia-platform (§1), because it exercises the identical resolver chain gap identified in §2 — `APLSkillRequest` has no `JsonTypeInfo` in `AplModelContext`/`ModelContext` regardless of which realistic payload is used. Confirming why the *old* validator would not have caught this even if it had reused fixtures naively: its Scenario 8 only round-trips `SkillRequest`/`LaunchRequest` (§4) — the bug is specific to the concrete `APLSkillRequest` root type, so the fix is exercising that type through the real serializer, not merely "using a fixture file instead of a hand-written literal." Fixture reuse and correct-type coverage are both necessary; fixture reuse alone would not have caught this if the old validator had reused a `LaunchRequest` fixture instead of an `APLSkillRequest`-shaped one.

### Package combinations

Do not create nine separate apps. The existing single validation app already composes Model + Model.Apl + MediatR + MediatR.Lambda + Http + Smapi + ISP in one process, in the order a real multi-package consumer (like trivia-platform, which uses MediatR.Lambda + Model.Apl + Smapi) would initialize them. That is the right shape — the fix is to make its *scenarios* exercise each package's real public entry point with a realistic payload, not to fragment it. Add the missing Smapi configuration-binding scenario (§"Configuration" below) to the existing app rather than a new one.

### Consumer-owned metadata boundary

Keep the existing negative/positive split already partially present, and make it explicit:
- **Positive**: an AlexaVoxCraft-owned type (e.g. `APLSkillRequest`, after the fix) deserializes with zero consumer-side registration.
- **Negative**: a scenario that deliberately constructs a *consumer-defined* type with no registered `JsonSerializerContext` and asserts it throws the expected `NotSupportedException` — proving the resolver chain doesn't silently fall back to reflection under AOT (it can't, since the fallback is compiled out, but this should be asserted, not assumed).
- **Freshness invariant**: keep/extend the existing proof that `RegisterTypeInfoResolver` calls made after a client is constructed are still picked up before that client's next actual use (already documented behavior, cross-checked against `docs/components/native-aot.md`).

### Rooting vs. registration — make the distinction load-bearing in the validation app itself

Every scenario asserting "type X is AOT-safe" must actually invoke `JsonSerializer.Deserialize<X>`/`Serialize<X>` (or the equivalent real public entry point) with reflection disabled and assert success — never infer safety from `ProjectReference`/`TrimmerRootAssembly` presence alone. This is a review/authoring discipline for future scenarios as much as a one-time fix.

### Configuration

Add a scenario that calls `AddSmapiDeveloperClient(configuration, ...)`/`AddSkillInvocationClient(configuration, ...)` with a real `IConfiguration` at runtime (closes the Smapi coverage gap, §4/§7) — this proves the *fixed* binding mechanism works at runtime, once §3's fix lands. Runtime success alone is not sufficient for the AOT/trim-safety claim (§6) — see the analyzer-gating change next.

### Making the analyzer signal actually load-bearing

Since diagnostics are compile-time, "validation execution" doesn't apply to them — but the validation app's *own build* (and the four real library call sites in §3) should be built with `EnableAotAnalyzer`/`EnableTrimAnalyzer` as a hard gate (warnings treated as errors for `IL2026`/`IL3050` specifically) in CI, applied to the library projects directly (not just inferred from the app's runtime success). This catches regressions at the four hand-written call sites going forward. It will **not** catch a reintroduced interceptor-emitted reflection call on its own (the `<auto-generated />` blind spot is orthogonal to warnings-as-errors) — the real fix for that is removing the reflection from `InterceptorEmitter`'s emitted code entirely (§3), which is also the recommended fix, not a suppression workaround.

### No hidden validation-only setup

Confirmed already true for the scenarios that exist (§4) — `Program.cs` calls only documented public APIs (`APLSupport.Add()`, `AddSkillMediator`, etc.), no internal metadata registration shortcuts. Preserve this property explicitly as a review criterion for the new scenarios above.

## 9. Proposed CI / release validation tiers

Per the user-confirmed sequence and the "don't gate every change on the expensive consumer/AWS layer" instruction:

**Tier 1 — every PR (fast, in this repo, no AWS):**
- Unit/property tests (existing).
- `EnableAotAnalyzer`/`EnableTrimAnalyzer` as warnings-as-errors on the affected library projects (new gate for §3's four call sites and any future reflection-based binding).
- Native AOT validation app: publish + execute, including the new `APLSkillRequest` Lambda round-trip scenario and the new Smapi configuration scenario (§8). This is the existing CI workflow, strengthened in place — no new workflow needed.

**Tier 2 — pre-release (this repo, still no AWS, but a stronger local proof before cutting a version):**
- Same as Tier 1, explicitly re-run on the exact commit being released (already effectively true since Tier 1 runs on every push to `main`).
- Pack local NuGet packages from the fixed source.

**Tier 3 — final release validation (trivia-platform dogfood, real AWS):**
- trivia-platform consumes the exact local Tier-2 packages (not NuGet.org yet).
- trivia-platform Native AOT publish.
- Real AWS dev Lambda deployment.
- A real (or realistic simulated) Alexa request, including at least one APL-capable request, actually processed end-to-end.

Not every code change needs Tier 3. Tier 3 is appropriate before a release is considered to have re-earned the Native AOT compatibility claim (i.e., before this fix ships) and periodically as a trust-but-verify check thereafter — not on every commit. This matches the user's explicit instruction: fix promptly, rerun strengthened validation, dogfood again through trivia-platform and real Lambda before considering the claim validated — and do this once, for this fix, not as a permanent per-PR requirement.

## 10. `IsAotCompatible` assessment

- **Packages implicated by Issue 1**: `AlexaVoxCraft.Model.Apl` (owns the missing registration; the actual fix site) and, derivatively, `AlexaVoxCraft.MediatR.Lambda`, `AlexaVoxCraft.Lambda`, `AlexaVoxCraft.MinimalLambda` (all three share the one `AlexaJsonOptions.DefaultOptions` instance and would all fail identically for any consumer using `APLSkillRequest` through any of them).
- **Package implicated by Issue 2**: `AlexaVoxCraft.MediatR` (both the hand-written fallback and the generator-emitted interceptor code) and `AlexaVoxCraft.Smapi` (two independent, currently-unvalidated call sites).
- **Is the current `IsAotCompatible=true` declaration technically misleading today?** For `Model.Apl`, yes in a narrow but real sense — it has a confirmed, 100%-reproducible runtime crash on a documented, sample-demonstrated usage pattern under Native AOT. For `MediatR`/`Smapi`, the risk is a live analyzer warning plus (for the hand-written fallback path) a real reflection call — less severe than a guaranteed crash but still a genuine violation of the "no AlexaVoxCraft-owned trim/AOT warnings" bar the ADR itself sets.
- **Per explicit user decision (§ this conversation)**: keep `IsAotCompatible=true` on all currently-declared packages. Treat both issues as pre-release defects to be fixed before the next release ships, not as grounds to retract the compatibility declaration now. No temporary `false` flip, no temporary doc caveat — the constraint is that the fix, the closed validation gaps, and a fresh trivia-platform/AWS dogfood cycle must all land **before** the next release, not that the MSBuild property changes in the interim.
- **Narrower qualification needed?** No new per-package qualification is needed once the fixes land — the existing package-level granularity (nine packages, uniform `true`) remains accurate. `AlexaVoxCraft.Observability` and `AlexaVoxCraft.MinimalLambda` remain correctly undeclared (never audited, per ADR-0001) and are out of scope here — `MinimalLambda`'s shared exposure to Issue 1 (§ above) is a fact about its runtime behavior, not a reason to newly declare `IsAotCompatible` on it.

## 11. Public API impact

**None required.** Both fixes are internal:
- Issue 1: add `[JsonSerializable(typeof(APLSkillRequest))]` to the existing internal `AplModelContext` — no new public type, no new public method.
- Issue 2: change `InterceptorEmitter`'s emitted source text and the two hand-written `ServiceCollectionExtensions.Bind(...)` call bodies to direct property assignment — no signature change to `AddSkillMediator`, `AddSmapiDeveloperClient`, or `AddSkillInvocationClient`.

The existing single public surface (`AlexaJsonOptions.RegisterTypeInfoResolver(IJsonTypeInfoResolver)`, per ADR-0001) is sufficient and unaffected. Consumers should not, and will not need to, register any AlexaVoxCraft-owned type themselves — that invariant holds after the fix, it was simply violated for one type before it.

## 12. Documentation impact

- **`docs/components/native-aot.md`**: no correction needed to its consumer-facing instructions (they already correctly state "no code changes needed... works out of the box for every AlexaVoxCraft-owned type" — that claim becomes true again once the fix lands; it does not need a workaround documented, per the explicit instruction not to paper over the bug with consumer-facing guidance).
- **`CLAUDE.md`**: no correction to its internal-mechanics description is needed — the resolver chain and freshness-invariant descriptions there are accurate; they describe the design, not the specific missing registration.
- **`docs/adr/0001-native-aot-compatibility.md`**: no amendment needed (§13) — the fix is a defect within the accepted architecture, not a change to it.
- **`docs/plans/0003-native-aot-compatibility-implementation.md`**: worth a small addendum note (not a rewrite) recording that its own validation scenario spec (naming `UserEventRequest` rather than `APLSkillRequest`) was insufficient to prevent this class of gap — useful institutional memory for whoever authors the next validation-scenario spec, but this is optional polish, not a blocking requirement.
- **No repository-level Claude skill exists** (confirmed: no `.claude/skills/` directory, no `evals.json` anywhere) — no skill/eval update is required.
- Other component docs (`apl-integration.md`, `in-skill-purchasing.md`, `lambda-hosting.md`, `session-management.md`, SMAPI docs) make only general "works out of the box" claims already consistent with the post-fix state — no changes needed.

## 13. ADR determination

**No new or amended ADR is required.** Both fixes are defect corrections within the architecture ADR-0001 already accepted:

- Issue 1's fix (`[JsonSerializable(typeof(APLSkillRequest))]` in `AplModelContext`) is exactly the mechanism ADR-0001 already prescribes for package-owned root types — an omission within the pattern, not a new pattern.
- Issue 2's fix (reflection-free binding for two flat POCOs) touches no resolver ownership, no public API, no package activation semantics, and no configuration API *design* — it changes an *implementation detail* (how one internal method populates two option objects) while leaving the documented `AddSkillMediator`/`AddSmapiDeveloperClient`/`AddSkillInvocationClient` surface untouched.
- The `<auto-generated />` diagnostic-visibility blind spot (§3, §6) is a genuinely new insight, but it is a **validation/tooling gap**, not an architectural decision about the product — the corrective (stop emitting the reflection call in the first place) removes the underlying risk rather than requiring a durable policy about how generated files should be marked.
- The validation-contract strengthening (§8-9) is process/test-architecture work, not a resolver-ownership, initialization-semantics, or public-API decision — it operates entirely within ADR-0001's existing model.

## 14. Recommended implementation scope

One cohesive PR, matching the user's explicit expectation ("I expect this can likely be one implementation PR"):

1. `src/AlexaVoxCraft.Model.Apl/Serialization/AplModelContext.cs`: add `[JsonSerializable(typeof(APLSkillRequest))]`.
2. `src/AlexaVoxCraft.MediatR.Generators/Generators/InterceptorEmitter.cs`: replace the emitted `ConfigurationBinder.Bind(cfg)`/`.Bind(opt)` text with direct, reflection-free property assignments for `SkillServiceConfiguration`'s five bindable properties.
3. `src/AlexaVoxCraft.MediatR/DI/ServiceCollectionExtensions.cs` (lines 14, 23): same reflection-free treatment for the hand-written fallback path.
4. `src/AlexaVoxCraft.Smapi/ServiceCollectionExtensions.cs` (lines 49, 143): same treatment for `SmapiDeveloperAccessTokenOptions`.
5. `test/AlexaVoxCraft.NativeAot.ValidationApp/AlexaVoxCraft.NativeAot.ValidationApp.csproj`: add `<EmbeddedResource>` items linking in `LaunchRequest.json`, `IntentRequest.json` (from `AlexaVoxCraft.Model.Tests/Examples/Requests/`) and `UserTouchRequest.json`, `APLUserEvent_Answer.json` (from `AlexaVoxCraft.Model.Apl.Tests/Examples/Requests/`), per §8's fixture-reuse pattern. `Program.cs`: add the `APLSkillRequest` Lambda-serializer round-trip scenario using these embedded fixtures (with `APLSupport.Add()` active, matching real consumer startup order) and the Smapi configuration-binding runtime scenario. This is additive to the app's existing scenarios, and entirely independent of the unchanged JIT unit test suites (§8) — the JIT tests keep validating detailed behavior exactly as today.
6. `.github/workflows/native-aot-validation.yaml` and/or the library `.csproj`s: gate `EnableAotAnalyzer`/`EnableTrimAnalyzer` output as errors for `IL2026`/`IL3050` on `AlexaVoxCraft.MediatR` and `AlexaVoxCraft.Smapi`.
7. `test/AlexaVoxCraft.Model.Apl.Tests/Serialization/AplModelContextCompletenessTests.cs`: extend (or add a sibling test) so root-type coverage — not just polymorphic-dispatch-target coverage — is mechanically checked, closing the class of gap for future root types.
8. `docs/plans/0003-native-aot-compatibility-implementation.md`: small addendum note (§12), optional.
9. Local pack + trivia-platform dogfood + real AWS dev Lambda validation (Tier 3, §9) before considering the fix complete, per explicit user instruction — outside this repo, not part of the PR diff itself, but part of the completion criteria for this effort.

No ADR, no public API changes, no `IsAotCompatible` MSBuild changes. Everything above fits within the existing accepted architecture.
