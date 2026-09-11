# Plan 0004: Native AOT runtime fixes and validation reliability

## Goal

Fix the two Native AOT defects surfaced by a real deployed consumer (trivia-platform) — issues
[#190](https://github.com/LayeredCraft/alexa-vox-craft/issues/190) (`APLSkillRequest` runtime
deserialization failure) and [#191](https://github.com/LayeredCraft/alexa-vox-craft/issues/191)
(`ConfigurationBinder.Bind` AOT/trim warnings) — **and** close the validation gaps that let both ship
past a "continuously-CI-run Native AOT validation application," as one cohesive Native AOT reliability
effort. This plan implements the conclusions of
`docs/research/2026-09-11-native-aot-runtime-validation-gaps.md` ("the research doc"). No ADR is
required — both fixes and the validation redesign fit entirely within the architecture already accepted
in `docs/adr/0001-native-aot-compatibility.md` ("ADR-0001"); see the research doc §13 for the explicit
determination.

## Constraints

- **Single PR, single plan, no exceptions.** Task groups establish implementation order and in-PR
  validation checkpoints only; a task group is never a PR boundary.
- Do not implement anything while writing or revising this plan document — this is planning only.
- No public API changes. No new `AlexaJsonOptions` method, no new `IJsonTypeInfoResolver`-related
  surface, no signature change to `AddSkillMediator`/`AddSmapiDeveloperClient`/`AddSkillInvocationClient`.
  The existing `AlexaJsonOptions.RegisterTypeInfoResolver(IJsonTypeInfoResolver)` (ADR-0001) remains the
  only consumer-facing metadata-registration API; per the research doc, consumers must never need to
  register any AlexaVoxCraft-owned type themselves — that invariant was violated for `APLSkillRequest`
  and this plan restores it, not works around it.
- No `IsAotCompatible` MSBuild changes on any package. Explicit user decision (recorded in this
  conversation): keep `IsAotCompatible=true` everywhere, fix promptly, no temporary downgrade and no
  temporary doc caveat — but the fix, the strengthened validation, and a fresh trivia-platform/AWS
  dogfood cycle must all land before the next release ships.
- Existing JIT unit test suites (`AlexaVoxCraft.Model.Tests`, `AlexaVoxCraft.Model.Apl.Tests`, etc.) are
  **not replaced, rearchitected, or run under Native AOT**. They remain the fast, primary, unchanged
  correctness net. The Native AOT validation work in this plan is strictly additive — a second, narrower
  layer proving the same trusted fixtures survive the real production serializer path under a reflection-
  disabled published binary, a property the JIT suite structurally cannot prove (research doc §4, §8).
- Do not invent new simplified Alexa payloads for the Native AOT validator. Reuse the existing, trusted
  JSON fixture files already embedded in `AlexaVoxCraft.Model.Tests`/`AlexaVoxCraft.Model.Apl.Tests` via
  cross-project `<EmbeddedResource Include="..\...\Examples\..." Link="..." />` references (research doc
  §8) — not file copies, not `CopyToOutputDirectory` content items (the CI workflow's execution working
  directory does not match the publish output folder; embedding is publish-layout-independent).
- No broad warning suppression for the `IL2026`/`IL3050` findings (`UnconditionalSuppressMessage` without
  a proven-safe justification, file/assembly-scope `#pragma warning disable`). Fix the underlying
  reflection call, per both issues' explicit acceptance criteria.
- Preserve existing configuration-binding *behavior* exactly — `SkillServiceConfiguration` and
  `SmapiDeveloperAccessTokenOptions` must bind identically to today for every currently-supported
  `IConfiguration` shape (section-relative keys, defaults when a key is absent, enum parsing for
  `ServiceLifetime`).
- Every file:line citation below was confirmed during the investigation that produced the research doc
  (2026-09-11); re-verify with `grep`/`find` at implementation time before editing, per this repo's
  existing planning convention (see Plan 0003's equivalent note).

## Relationship to the research doc

This plan implements `docs/research/2026-09-11-native-aot-runtime-validation-gaps.md` exactly as
concluded. Every file:line citation below traces to that document's §2-§8 findings, independently
verified by direct source reading and (for issue #2) live `dotnet build` reproduction of the warnings
during the investigation — not copied blind.

---

## Task Group 1 — Fix `APLSkillRequest` missing JSON metadata (Issue #190)

Implements research doc §2 and §14 item 1. This is the fix for the confirmed 100%-reproducible runtime
crash on real deployed Lambda infrastructure — the highest-severity item in this plan.

**Verified against current code:**
- `src/AlexaVoxCraft.Model.Apl/APLSkillRequest.cs:6` — `class APLSkillRequest : SkillRequest`, the only
  `SkillRequest` subclass in the library, not part of any polymorphic hierarchy.
- `src/AlexaVoxCraft.Model.Apl/Serialization/AplModelContext.cs:29-218` — ~190 `[JsonSerializable]`
  entries, none for `APLSkillRequest`.
- `src/AlexaVoxCraft.Model/Serialization/ModelContext.cs:14` — registers `SkillRequest` (base only).
- `src/AlexaVoxCraft.Model/Serialization/AlexaJsonOptions.cs:69-72` — JIT-only reflection fallback,
  compiled out under `PublishAot`, which is why this gap is invisible to every existing JIT test.

**Changes:**
1. Add `[JsonSerializable(typeof(APLSkillRequest))]` to `AplModelContext` — the natural owning context
   (same assembly as the type, already the exhaustive registry for every other APL root/dispatch type;
   `ModelContext` cannot own it since `AlexaVoxCraft.Model` has no reference to `AlexaVoxCraft.Model.Apl`).
2. Re-run `dotnet build AlexaVoxCraft.slnx` to confirm the added attribute compiles and the generated
   `AplModelContext` partial picks up the new root type with no other change required.

**Testing:**
- No new JIT unit test is required to *catch* this specific regression again (JIT reflection fallback
  masks it structurally — research doc §4) — the regression guard belongs in the Native AOT validator
  (Task Group 3), not the JIT suite. However, if `test/AlexaVoxCraft.Model.Apl.Tests/Serialization/
  AplModelContextCompletenessTests.cs`'s existing test is extended per Task Group 4, confirm it now
  passes with `APLSkillRequest` included in whatever new root-type check is added there.
- Full `AlexaVoxCraft.Model.Apl.Tests` suite must stay green (`dotnet run --project
  test/AlexaVoxCraft.Model.Apl.Tests --framework net9.0`).

**Open decisions:** none — single-attribute, fully determined fix.

---

## Task Group 2 — Eliminate reflection-based `ConfigurationBinder.Bind` (Issue #191)

Implements research doc §3 and §14 items 2-4. Four real call sites across two packages, plus the
generator that mirrors two of them into every consumer's interceptor.

**Verified against current code (with live `dotnet build` reproduction of the exact warnings):**
- `src/AlexaVoxCraft.MediatR/DI/ServiceCollectionExtensions.cs:14` —
  `configuration.GetSection(sectionName).Bind(serviceConfig);`
- `src/AlexaVoxCraft.MediatR/DI/ServiceCollectionExtensions.cs:23` —
  `configuration.GetSection(sectionName).Bind(opt);` (inside `services.Configure<SkillServiceConfiguration>`)
- `src/AlexaVoxCraft.Smapi/ServiceCollectionExtensions.cs:49` — inside `AddSmapiDeveloperClient`
- `src/AlexaVoxCraft.Smapi/ServiceCollectionExtensions.cs:143` — inside `AddSkillInvocationClient`
- `src/AlexaVoxCraft.MediatR.Generators/Generators/InterceptorEmitter.cs:48,54` — emits the identical
  `.Bind(cfg)`/`.Bind(opt)` text into the generated interceptor backing the documented, primary
  `AddSkillMediator(configuration, cfg => ...)` call path. This file starts with `// <auto-generated />`
  (`InterceptorEmitter.cs:15`), which is why Roslyn's analyzer infrastructure suppresses `IL2026`/`IL3050`
  for this call even though it is the mainline path every consumer hits on every cold start — a
  diagnostic-visibility blind spot, not evidence of safety (research doc §3, §6).
- Bound types confirmed flat/simple: `SkillServiceConfiguration` (`src/AlexaVoxCraft.MediatR/DI/
  SkillServiceConfiguration.cs`) — `string?`/`string?`/`string?`/`ServiceLifetime`/`int`, plus an
  `internal` no-setter `List<Assembly>` the binder cannot touch. `SmapiDeveloperAccessTokenOptions`
  (`src/AlexaVoxCraft.Smapi/Auth/SmapiDeveloperAccessTokenOptions.cs`) — three `string` properties.

**Changes:**
1. `InterceptorEmitter.cs`: replace the emitted `configuration.GetSection(sectionName).Bind(cfg)` /
   `.Bind(opt)` source text with direct, reflection-free per-property assignment/read statements for
   `SkillServiceConfiguration`'s five bindable properties (`CustomUserAgent`, `SkillId`,
   `DefaultVoiceName`, `Lifetime` via `configuration.GetValue<ServiceLifetime>(key, default)`,
   `CancellationTimeoutBufferMilliseconds`). The generator already has full compile-time knowledge of
   this type's fixed shape, so this is a mechanical string-template change, not a new capability. This
   path is not eligible for Microsoft's Configuration Binding source generator (research doc §3 —
   Roslyn generators cannot see or rewrite another generator's emitted output within the same
   compilation), so hand-emitted reflection-free binding is mandatory here, not a choice.
2. `src/AlexaVoxCraft.MediatR/DI/ServiceCollectionExtensions.cs:14,23` and
   `src/AlexaVoxCraft.Smapi/ServiceCollectionExtensions.cs:49,143` — **decision criterion, resolved at
   implementation time, not prescribed here:** these four call sites are ordinary, statically-known
   `.Bind()` calls (unlike the generator's emitted text, nothing prevents Microsoft's
   `ConfigurationBindingSourceGenerator` — already available via the already-referenced
   `Microsoft.Extensions.Configuration.Binder` package — from intercepting them). Before writing manual
   per-property binding for these four sites, evaluate enabling the Configuration Binding source
   generator (`EnableConfigurationBindingGenerator`) for `AlexaVoxCraft.MediatR`/`AlexaVoxCraft.Smapi` and
   letting it bind `SkillServiceConfiguration`/`SmapiDeveloperAccessTokenOptions` at these sites, subject
   to all of the following holding:
   - the existing public API/signatures of `AddSkillMediator`, `AddSmapiDeveloperClient`,
     `AddSkillInvocationClient` are unchanged;
   - defaults for absent/missing keys, `ServiceLifetime` enum parsing, and
     `CancellationTimeoutBufferMilliseconds` numeric parsing bind identically to today's
     `ConfigurationBinder.Bind` behavior (verified by the behavioral test below, not assumed from the
     generator's general reputation for correctness);
   - the resulting `IL2026`/`IL3050` warnings are actually eliminated at these sites when built with
     `EnableAotAnalyzer`/`EnableTrimAnalyzer` (confirm by inspecting compiler output, not just an absence
     of visible warnings in an IDE);
   - it introduces no consumer-visible project configuration requirement, no hidden SDK/analyzer version
     floor beyond what these packages already require, and no meaningfully greater complexity than the
     small, fixed shape of these two option types justifies.
   If the source generator satisfies all of the above cleanly, prefer it over hand-written binding at
   these four sites — it keeps one binding-semantics definition (Microsoft's generator) rather than two
   independently-maintained ones (the hand-rolled interceptor code plus a second hand-rolled copy here),
   and means a future property added to either options type needs updating in fewer places. If it
   introduces awkward project configuration, a behavioral difference, or complexity disproportionate to
   these two small types, fall back to the same direct, reflection-free per-property binding pattern used
   for `InterceptorEmitter.cs` in point 1 — this remains fully acceptable per the original plan and is not
   a regression from it.
3. Confirm no other `.Bind(`/`.Get<`/`ConfigurationBinder` usage exists elsewhere in the repo (research
   doc §3/§7 confirms none today) — re-grep at implementation time to catch any drift since the research
   pass.

**Testing:**
- Existing configuration-binding tests (wherever `AddSkillMediator`/`AddSmapiDeveloperClient`/
  `AddSkillInvocationClient` are exercised with a real `IConfiguration` today) must continue to pass
  unchanged — behavioral equivalence is the bar, not just "no warning."
- **Behavioral test for the generated MediatR path, required regardless of which binding approach point 2
  resolves to**: a runtime test that drives the actual generated `AddSkillMediator(configuration, ...)`
  interceptor output (not a hand-constructed `SkillServiceConfiguration`) through a real `IConfiguration`
  and asserts, property by property, that every bindable `SkillServiceConfiguration` member
  (`CustomUserAgent`, `SkillId`, `DefaultVoiceName`, `Lifetime`, `CancellationTimeoutBufferMilliseconds`)
  binds correctly — including the `ServiceLifetime` enum parse and the numeric
  `CancellationTimeoutBufferMilliseconds` parse — and that each property falls back to its existing
  default when its key is absent or the section is partial. This is a behavioral/semantic test, distinct
  from and in addition to the existing generated-source Verify snapshot (the snapshot proves the emitted
  *shape*; this test proves the emitted *binding semantics*, so a future refactor of the emitted code
  shape cannot silently change binding behavior without a test failing).
- Add the equivalent property-by-property behavioral test for `SmapiDeveloperAccessTokenOptions` via
  `AddSmapiDeveloperClient`/`AddSkillInvocationClient`.
- `dotnet build` `AlexaVoxCraft.MediatR.csproj` and `AlexaVoxCraft.Smapi.csproj` directly (both
  `IsAotCompatible=true`, which turns on `EnableAotAnalyzer`/`EnableTrimAnalyzer`) and confirm the four
  `IL2026`/`IL3050` warnings at the cited lines are gone, with no new warnings introduced.
- Generated-output test: extend the existing generator-output Verify-snapshot tests (per Plan 0003's
  precedent for `AlexaVoxCraft.MediatR.Generator.Tests`) to assert the new reflection-free
  `AddSkillMediator` interceptor source shape.

**Open decisions:**
- Whether points 2's four hand-written call sites use Microsoft's Configuration Binding source generator
  or the same hand-emitted pattern as `InterceptorEmitter.cs` — resolved at implementation time against
  the criteria listed in point 2, not prescribed by this plan. Either outcome satisfies this task group;
  the behavioral test above is required either way and is what actually proves correctness.
- Exact statement formatting/ordering for any hand-emitted property assignments — a pure code-shape
  choice with no behavioral consequence, left to implementation time.

---

## Task Group 3 — Native AOT validator: reuse trusted fixtures through the real Lambda serializer path

Implements research doc §8 (validation contract) and §14 item 5. This closes the actual validation hole
that let Issue #190 escape — the prior validator never deserialized anything into `APLSkillRequest`, and
invented every payload from scratch rather than reusing a trusted fixture (research doc §4).

**Verified against current code:**
- `test/AlexaVoxCraft.NativeAot.ValidationApp/Program.cs` — 8 scenarios, all payloads either inline
  string literals or code-constructed objects; zero fixture reuse from the JIT test suites (confirmed by
  direct reading of every scenario during the investigation).
- Fixture corpus: ~60 JSON files across `test/AlexaVoxCraft.Model.Tests/Examples/`,
  `test/AlexaVoxCraft.Model.Apl.Tests/Examples/`, `test/AlexaVoxCraft.Model.InSkillPurchasing.Tests/
  Examples/`, embedded via `<EmbeddedResource Include="Examples\**\*.json"/>`, loaded via a shared
  `Fx(relativePath)` helper (`test/AlexaVoxCraft.Model.Tests/TestBase.cs`, mirrored in Model.Apl.Tests).
- `test/AlexaVoxCraft.Model.Apl.Tests/APLSkillRequestTests.cs` already deserializes
  `Examples/Requests/UserTouchRequest.json` and `Examples/Requests/APLUserEvent_Answer.json` directly
  into `APLSkillRequest` under JIT — these are the exact fixtures this task group reuses.
- No `Link=` cross-project file references exist anywhere in the repo today (confirmed via repo-wide
  grep) — this is a new but repo-idiom-consistent pattern, not a precedent-breaking one (mirrors the
  existing `EmbeddedResource Include="Examples\**\*.json"` convention, just pointed cross-project).
- `.github/workflows/native-aot-validation.yaml:54` executes the published native binary directly by
  full path with the **repo root** as working directory — confirms `CopyToOutputDirectory` content +
  runtime relative-path loading would be fragile/broken here; embedding is the robust choice.
- No `SkillResponse`/response-envelope JSON fixtures exist anywhere in the repo — response validation is
  via Verify snapshots of code-constructed objects. The validator's response-serialization scenario
  continues constructing objects in code, unchanged from today's approach; there is no fixture to switch
  it to.

**Changes:**
1. `test/AlexaVoxCraft.NativeAot.ValidationApp/AlexaVoxCraft.NativeAot.ValidationApp.csproj`: add
   ```xml
   <ItemGroup>
     <EmbeddedResource Include="..\AlexaVoxCraft.Model.Tests\Examples\Requests\LaunchRequest.json" Link="Examples\LaunchRequest.json" />
     <EmbeddedResource Include="..\AlexaVoxCraft.Model.Tests\Examples\Requests\IntentRequest.json" Link="Examples\IntentRequest.json" />
     <EmbeddedResource Include="..\AlexaVoxCraft.Model.Apl.Tests\Examples\Requests\UserTouchRequest.json" Link="Examples\UserTouchRequest.json" />
     <EmbeddedResource Include="..\AlexaVoxCraft.Model.Apl.Tests\Examples\Requests\APLUserEvent_Answer.json" Link="Examples\APLUserEvent_Answer.json" />
   </ItemGroup>
   ```
2. `Program.cs`: add a resource-loading helper mirroring the JIT suites' `Fx()` pattern (read via
   `Assembly.GetManifestResourceStream` against the validation app's own assembly, using the `Link`-based
   manifest resource name).
3. `Program.cs`, Scenario 8 (existing Lambda-serializer boundary scenario, `Program.cs:227-241`): replace
   its hand-built `SkillRequest` object with the embedded `LaunchRequest.json`/`IntentRequest.json`
   fixtures, deserialized through the real `AlexaLambdaSerializer.Deserialize<SkillRequest>` — proves
   reuse of a trusted payload, not just round-trip self-consistency.
4. `Program.cs`: add a new scenario (Scenario 9) that:
   a. calls `APLSupport.Add()` (the documented, required consumer step — no validation-only shortcut);
   b. deserializes the embedded `UserTouchRequest.json` into `APLSkillRequest` via the real
      `AlexaLambdaSerializer.Deserialize<APLSkillRequest>` — the exact call trivia-platform's Lambda made
      when it crashed;
   c. asserts `Request` is the expected concrete type (`UserEventRequest`) — a semantic assertion
      matching what the JIT `APLSkillRequestTests.cs` already checks, not a raw JSON comparison;
   d. repeats b-c with `APLUserEvent_Answer.json` for `Viewport`/`AplVisualContext` surface coverage;
   e. dispatches the deserialized request through the real `ISkillMediator.Send`;
   f. serializes the resulting `SkillResponse` back out through the same `AlexaLambdaSerializer`
      (code-constructed response, per the existing approach — no response fixture exists to switch to).
5. `Program.cs`: add a scenario exercising `AddSmapiDeveloperClient(configuration, ...)`/
   `AddSkillInvocationClient(configuration, ...)` with a real `IConfiguration` at runtime — closes the
   Smapi configuration-binding coverage gap identified in research doc §4/§7 (currently zero validation-
   app coverage for these two call sites).
6. Fail-fast assertion/exception discipline for every new scenario, consistent with the app's existing
   pattern (non-zero exit on failure) — this stays a small executable with explicit scenarios, not a
   second test framework.

**Testing:**
- Publish and execute the strengthened validator locally
  (`dotnet publish test/AlexaVoxCraft.NativeAot.ValidationApp/... -r <rid> -p:PublishAot=true`, then run
  the resulting native binary) **before** Task Group 1 lands, to confirm Scenario 9 reproduces the exact
  `NotSupportedException` trivia-platform hit — this is the regression-proof step.
- Re-run after Task Group 1 lands to confirm Scenario 9 now passes.
- Confirm the existing 8 scenarios (renumbered as needed) still pass unchanged in behavior, only sourcing
  Scenario 8's payload from the new embedded fixtures instead of a hand-built object.
- CI (`.github/workflows/native-aot-validation.yaml`) must run the strengthened app publish + execute
  step unchanged in trigger/shape — no new workflow file needed (research doc §9, Tier 1).

**Open decisions:** whether to also embed and exercise one or more ISP fixtures
(`IntentRequest_BuyIntent.json`, `ConnectionResponsePayload_Declined.json`) in the same pass — the
research doc identifies them as available and cheap to add given the fixture corpus is small, but they
are not required to close either filed issue. Left to implementation time as a low-cost addition if
convenient; not a blocking requirement of this plan.

---

## Task Group 4 — Close the completeness-test blind spot for future root types

Implements research doc §4's finding that the existing `AplModelContextCompletenessTests.cs` only walks
types reachable through `BasePolymorphicConverter<T>.DerivedTypes` and is therefore structurally blind to
a missing *root*-type registration like `APLSkillRequest` — not just blind to this one instance, but to
the whole class of gap for any future root type.

**Verified against current code:**
- `test/AlexaVoxCraft.Model.Apl.Tests/Serialization/AplModelContextCompletenessTests.cs` —
  `AplModelContext_HasMetadata_ForEveryPolymorphicDispatchTarget` walks `BasePolymorphicConverter<T>.
  DerivedTypes` only.

**Changes:**
1. Add a sibling test (or extend the existing one) that separately enumerates the set of types a
   consumer is documented/expected to pass directly as `T` to `JsonSerializer.Deserialize<T>`/
   `AlexaLambdaSerializer.Deserialize<T>` for this package (today: `SkillRequest` via `ModelContext`,
   `APLSkillRequest` via `AplModelContext`) and asserts each has a `JsonTypeInfo` in its owning context's
   `.Default` resolver — a direct, explicit root-type check, independent of the polymorphic-dispatch-
   target walk.
2. Keep this test intentionally small and enumerable (a short, explicit list of "known root types per
   context"), not a reflection-based scan of "every public class in the assembly" — the goal is proving
   the specific documented entry points stay covered, not policing every type in the library.

**Testing:**
- The new/extended test must fail against the pre-Task-Group-1 state (i.e., temporarily verify it catches
  the `APLSkillRequest` gap if run against a version without the Task Group 1 fix) and pass after Task
  Group 1 lands — this is the regression guard that closes the "future root type" risk research doc §7
  flags as the primary remaining systemic risk.

**Open decisions:** none.

---

## Task Group 5 — Documentation addendum (optional, low-cost)

Implements research doc §12's determination that no consumer-facing documentation is incorrect today (it
already correctly claims "no code changes needed... works out of the box for every AlexaVoxCraft-owned
type" — a claim restored to true by Task Group 1, not one that needs a workaround documented) and no
skill/eval update is required (no `.claude/skills/` directory or `evals.json` exists in this repo).

**Changes:**
1. `docs/plans/0003-native-aot-compatibility-implementation.md`: add a short addendum note recording
   that its own validation-scenario spec named `UserEventRequest` (already-registered, would not have
   caught this class of gap) rather than the actual consumer-facing root type `APLSkillRequest` — useful
   institutional memory for whoever authors the next validation-scenario spec.

**Testing:** none — documentation only.

**Open decisions:** none; this task group may be skipped entirely without affecting the fix's
completeness, per its "optional" framing in the research doc.

---

## Completion criteria

This plan is complete when:

1. `APLSkillRequest` deserializes successfully through `AlexaLambdaSerializer.Deserialize<T>` in an
   executed, published Native AOT binary with reflection disabled (Task Group 1, proven by Task Group 3
   Scenario 9).
2. `AlexaVoxCraft.MediatR` and `AlexaVoxCraft.Smapi` build with zero `IL2026`/`IL3050` warnings at the
   four identified call sites, with no broad suppression (Task Group 2).
3. The Native AOT validation app reuses trusted JIT-test fixtures for its request-deserialization
   scenarios rather than inventing new payloads, and exercises the real `AlexaLambdaSerializer` and real
   Smapi configuration-binding entry points (Task Group 3).
4. A completeness test mechanically prevents a future AlexaVoxCraft-owned root type from shipping without
   `JsonSerializerContext` registration the way `APLSkillRequest` did (Task Group 4).
5. All existing JIT unit test suites remain green, unchanged in scope or architecture.
6. Per the explicit user decision on `IsAotCompatible`: after all of the above, pack local NuGet packages
   and dogfood them through trivia-platform (exact local packages, not NuGet.org) with a real Native AOT
   publish and a real AWS dev Lambda deployment processing at least one APL-capable request end-to-end,
   before this release is considered to have re-earned its Native AOT compatibility claim. This step is
   outside this repository and this PR's diff, but is part of this effort's completion bar per the user's
   explicit instruction.
