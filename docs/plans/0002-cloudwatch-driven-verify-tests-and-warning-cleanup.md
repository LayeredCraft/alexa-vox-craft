# Plan 0002: CloudWatch-driven Verify test migration and compiler warning cleanup

## Goal

Replace `AlexaVoxCraft.Model.Legacy.Tests` and `AlexaVoxCraft.Model.Apl.Legacy.Tests` with Verify-based
tests in `AlexaVoxCraft.Model.Tests` and `AlexaVoxCraft.Model.Apl.Tests`, built primarily from real
request/response payload shapes captured from the `disney-trivia-skill-prod-disney-trivia` CloudWatch
log group (not the legacy projects' existing JSON fixtures), then use that test coverage as a safety
net to eliminate the solution's compiler warnings — dominated by nullable-reference-type warnings in
the Model projects — without breaking the public API.

## Constraints

- No public API renames, removals, or signature changes. Adding/adjusting nullable annotations on
  public members is acceptable if it's the cleanest fix, but avoid it when a non-annotation fix is
  equally simple.
- Fix currently-emitted warnings only. Do not enable new analyzer rules or `TreatWarningsAsErrors`.
- Do not reuse JSON fixtures from `Model.Legacy.Tests/Examples` or `Model.Apl.Legacy.Tests/Examples`.
  New fixtures come from sanitized CloudWatch captures, supplemented with Compono/AutoFixture-built
  synthetic data only for shapes CloudWatch doesn't produce (error paths, ISP edge cases, etc.). If a
  needed shape genuinely cannot be found in either log group after a real search, repurposing the
  matching legacy fixture is allowed as a documented fallback (note it in the test/commit as
  legacy-sourced) rather than blocking on it.
- Log sources: `/aws/lambda/disney-trivia-skill-prod-disney-trivia` (unbounded time range) and
  `/aws/lambda/disney-trivia-skill-dev-disney-trivia` (events from 2026-09-04 onward only — dev is
  useful because it carries real, unredacted auth tokens that prod doesn't).
- Scrub all captured fixtures of real identifiers before committing: `AlexaUserId`, `user.userId`,
  `device.deviceId`, `sessionId`, `requestId`, `UserIdentifier`, and any token/secret-looking value not
  already redacted by the skill's own logging. Replace with realistic-shaped, obviously-fake values
  (zero-padded, e.g. `amzn1.ask.account.A0000000000000000000000000000000000000000000000000000000000000000000`).
  Dev-sourced fixtures need extra care: dev is not token-redacted, so `apiAccessToken`, consent
  tokens, and any ISP/purchase tokens must be scrubbed by hand there too, not just assumed handled.
  Grep the sanitized output for residual tokens before commit.
- Keep the solution compiling and the full test suite green at every checkpoint.
- Delete the Legacy test projects only after the new suites have equivalent-or-better coverage.
- Warning cleanup work happens only after the new Verify safety net is in place, project by project,
  smallest/most-mechanical warning categories first.
- Test/fixture naming must never leak provenance ("CloudWatch") into class or file names — the source
  is an implementation detail of how the fixture was built, not part of what's under test. Fixture
  folders are named for what they contain (`Requests/`, `Responses/`, `Components/`), test classes for
  what they test (`SkillRequestTests`, `IntentTests`, `RenderDocumentDirectiveTests`, etc.).
- Two test tiers, mirroring the legacy suite's style: **envelope-level** (does a full captured
  request/response round-trip) and **component-level** (does one piece — an `Intent`, a `Card`, a
  `Directive` — work in isolation, independent of the envelope). A break at either tier pinpoints
  where the regression is.
- Test direction follows envelope role, strictly, even when `System.Text.Json` happens to support the
  other direction: request-side objects (everything reachable from `SkillRequest`) are deserialize-only
  — the skill only ever receives them. Response-side objects (everything reachable from
  `SkillResponse`, including directives) are serialize-only — the skill only ever builds and sends
  them, never reads them back, so a serialize test is added even where the object *could* technically
  deserialize (e.g. `RenderDocumentDirective` — no deserialize test for it, despite legacy testing it
  that way). Only test both directions for a type genuinely used both ways in this SDK. This surfaced
  a real finding: `PaymentDirective`'s constructor can't bind for deserialization at all (params don't
  match property names) — moot under this rule since it's response-only, but worth knowing if anyone
  is ever tempted to deserialize a directive.
- For response-side serialize tests, values are inlined in test code (extracted from real captures for
  realism, e.g. real product ids/tokens) rather than deserializing a captured fixture file — matches
  the existing `BuyDirectiveTests` pattern. Request-side deserialize tests still read fixture files.

## Commit checkpoints

### Commit 0: Baseline and log harvesting tooling

Status: Done (no commit — `scripts/` is repo-gitignored, local tooling only).

Tasks:

- [x] Capture current full-solution warning baseline: `dotnet build AlexaVoxCraft.slnx -v:n --no-incremental`.
      Baseline: 6,348 warnings total (CS8618 4256, CS8625 608, CS8602 480, CS8604 440, CS8601 258,
      CS0108 120, CS8603 104, CS8600 24, CS8767 16, CS8619 16, CS8765 8, SYSLIB0057 6, RS2008 6,
      CS0618 4, CS0114 2). By project: `Model.Apl` 6,704 (incl. its Legacy test project), `Model`
      4,092 (incl. its Legacy test project), `MediatR` 84, `Model.InSkillPurchasing` 48,
      `MediatR.Generators` 12, `RoslynAnalyzers` 6, `MediatR.Lambda` 4.
- [x] Add `scripts/pull-skill-logs.sh` wrapping `aws logs filter-log-events` against both
      `/aws/lambda/disney-trivia-skill-prod-disney-trivia` (unbounded range) and
      `/aws/lambda/disney-trivia-skill-dev-disney-trivia` (events from 2026-09-04 onward only) to
      pull `Raw request:` / `Raw response:` entries and decode the nested JSON-in-string `Raw` field
      into JSON Lines. Not committed (matches `pack-local.sh` precedent — `scripts/` is local-only).
- [x] Confirmed AWS CLI access and log group name/format against a live pull from both groups:
      42 payloads from prod (unbounded), 182 from dev (since 2026-09-04).

No commit for this checkpoint — nothing under `scripts/` is tracked by git.

### Commit 1: Harvest and sanitize CloudWatch JSON fixtures

Status: Done.

Tasks:

- [x] Pulled request/response log pairs: 42 from `disney-trivia-skill-prod-disney-trivia` (unbounded),
      182 from `disney-trivia-skill-dev-disney-trivia` (since 2026-09-04).
- [x] Grouped by distinct shape. Found: LaunchRequest; IntentRequest x AnswerIntent,
      CategorySelectionIntent, AMAZON.HelpIntent, AMAZON.NoIntent, AMAZON.RepeatIntent,
      AMAZON.UnknownIntent, AMAZON.YesIntent, LeaderboardIntent, PlayerStatsIntent;
      `Alexa.Presentation.APL.UserEvent`; responses with plain speech+Simple card,
      `shouldEndSession=true`, `Alexa.Presentation.APL.RenderDocument` directive,
      `Alexa.Presentation.APL.ExecuteCommands` directive.
- [x] Deduped to one representative example per shape, preferring prod (pre-redacted) and falling
      back to dev only when a shape wasn't in prod.
- [x] `SessionEndedRequest` was not present in either log group, but a fixture for it already exists
      at `test/AlexaVoxCraft.Model.Tests/Examples/Requests/SessionEndedRequest.json` (pre-existing,
      non-legacy) — no gap, no action needed.
- [x] Still missing from both log groups, deferred to Commit 2/3 as legacy-fallback candidates (this
      skill doesn't exercise these interfaces): print-task Connections directives (legacy has
      `PrintPDFConnection.json`/`PrintWebPageConnection.json`/`PrintImageConnection.json`), and the
      legacy-only AudioPlayer/Dialog/Display/VideoApp directive fixtures.
- [x] Re-pulled both log groups after a second manual test pass covering more scenarios (prod 42→66,
      dev 182→216 events). New shapes found and landed: `AMAZON.StopIntent`, `DontKnowIntent`
      (→ `Model.Tests/Examples/CloudWatch/Requests/`); ISP flow — `BuyIntent`, `WhatCanIBuyIntent`,
      `ProductDetailIntent`, `Connections.Response`/`Buy` request, `Connections.SendRequest`/`Buy`
      response directive (→ `Model.InSkillPurchasing.Tests/Examples/CloudWatch/`, which already has
      its own non-legacy Verify-based suite — this fills the ISP gap noted above without needing the
      legacy print-connection fixtures). `SessionEndedRequest` (reason `USER_INITIATED`) also
      appeared live, confirming the pre-existing `Model.Tests/Examples/Requests/SessionEndedRequest.json`
      fixture's shape is representative; left as-is. All 7 new fixtures scrubbed and manually
      re-verified clean (per-turn `Connections` correlation tokens and `amzn1.adg.product.*` catalog
      ids left unscrubbed — not user-identifying).
- [x] Scrubbed all 15 fixtures: `amzn1.ask.account.*`, `amzn1.ask.device.*`,
      `amzn1.echo-api.session.*`, `amzn1.echo-api.request.*` replaced with zero-padded fakes; any
      JWT-shaped string (`eyJ...`) redacted regardless of source env. Manually re-grepped every
      fixture file afterward for residual JWTs, non-zeroed `amzn1.*` ids, and unredacted
      `apiAccessToken` values — all clean. Validated all 15 files are well-formed JSON.
- [x] Landed fixtures under `test/AlexaVoxCraft.Model.Tests/Examples/CloudWatch/{Requests,Responses}/`
      (10 request + 2 response files) and
      `test/AlexaVoxCraft.Model.Apl.Tests/Examples/CloudWatch/{Requests,Responses}/`
      (1 request + 2 response files).
- [x] Follow-up: initial `Alexa.Presentation.APL.UserEvent` pick only captured one of two distinct
      `arguments` shapes the skill actually sends on that request type (`["selectCategory", ...]` vs
      `["answer", N]"`) — the naive one-pick-per-request-type approach missed the sub-variant. Renamed
      the original to `APLUserEvent_SelectCategory.json` and added `APLUserEvent_Answer.json` so both
      argument shapes are covered. Worth remembering for Commit 2/3: check `arguments`/`payload`
      sub-shapes within a request type, not just the top-level type, when deciding "do we have this
      shape yet."
- [x] Follow-up 2: a third manual pass through ISP flows in dev surfaced two more
      `Alexa.Presentation.APL.UserEvent` sub-shapes — `token="productDetailToken"` with
      `arguments=["buyProduct", ...]` and `token="triviaPager"` with
      `arguments=["productDetails", ...]` — landed as `Model.InSkillPurchasing.Tests/Examples/
      CloudWatch/Requests/APLUserEvent_BuyProduct.json` and `APLUserEvent_ProductDetails.json`
      (ISP-flow UserEvents belong with the ISP fixtures, not the general Model.Apl ones). Both
      scrubbed and verified clean. No other new request/directive shapes appeared in this pass.

Suggested commit message:

```text
test: add sanitized CloudWatch-derived JSON fixtures
```

### Commit 2: Build envelope- and component-level tests in Model.Tests

Status: Done.

Tasks:

- [x] Reorganized Commit 1's fixtures out of a `CloudWatch/` subfolder into `Examples/Requests/`
      (envelope-level, merged with pre-existing fixtures — one rename to avoid a `LaunchRequest.json`
      collision: `LaunchRequest_FreshSession.json`) and `Examples/Components/` (extracted single-piece
      fragments, new).
- [x] Request side (deserialize-only): folded new `[Fact]`s into the existing `SkillRequestTests` for
      every captured intent/launch shape; added `Request/IntentTests.cs` (component-level) covering
      `Intent` with slot resolution in isolation.
- [x] Response side (serialize-only, per the Constraints rule): added `Response/SkillResponseTests.cs`
      (envelope-level, `SimpleCard` + `shouldEndSession=true`) and `Response/CardTests.cs`
      (component-level `SimpleCard`), both constructing objects in code from real captured values
      rather than deserializing a fixture. Added `VerifySerializedObject` to `Model.Tests/TestHelper.cs`
      (it only had `VerifyRequestObject` before) to support this.
- [x] Validated `dotnet test` (via `dotnet run --framework <tfm>`) across all 4 target frameworks
      (net8.0/9.0/10.0/11.0): 19/19 passing, no stray `.received.*` files.
- [x] Validated solution build: 0 errors.

Suggested commit message:

```text
test(model): add envelope- and component-level tests from real payloads
```

### Commit 3: Build envelope- and component-level tests in Model.Apl.Tests and Model.InSkillPurchasing.Tests

Status: Done.

Tasks:

- [x] `Model.Apl.Tests`: fixtures reorganized the same way (`Examples/Requests/`, no `Components/`
      needed after the response-side rework below). Request side: folded the 4 `UserEvent` argument
      shapes (`selectCategory`/`answer`/`buyProduct`/`productDetails`) into the existing
      `APLSkillRequestTests`. Response side (serialize-only): added `APLSkillResponseTests.cs`
      (envelope, `RenderDocumentDirective` + `ExecuteCommandsDirective` together — this skill always
      emits both for a page-render turn) and `Directive/RenderDocumentDirectiveTests.cs` /
      `Directive/ExecuteCommandsDirectiveTests.cs` (component-level), all constructing small
      representative objects in code (a full hand-built APL document tree matching the huge captured
      one isn't practical) rather than deserializing.
- [x] `Model.InSkillPurchasing.Tests`: fixtures under `Examples/Requests/` (`BuyIntent`,
      `WhatCanIBuyIntent`, `ProductDetailIntent`, `ConnectionsResponse_Buy`) and `Examples/Components/`
      (`ConnectionResponsePayload_Declined.json`). Request side: new
      `Requests/InSkillPurchasingRequestTests.cs` (envelope) plus a new fact on the existing
      `Responses/ConnectionResponseRequestTests.cs` (component-level, matches its established
      pattern). Response side: extended the existing `Directive/BuyDirectiveTests.cs` with a second
      serialize fact using a real captured product id/token — this is where the `PaymentDirective`
      deserialize-incompatibility finding surfaced and was resolved by dropping the deserialize
      attempt (response-only, per the Constraints rule), not by changing the source.
      `Examples/Response/` (singular, pre-existing) renamed to `Examples/Responses/` (plural) to match
      the other two projects' convention; the one reference in `ConnectionResponseRequestTests.cs`
      updated.
- [x] Validated all 4 target frameworks for both projects: `Model.Apl.Tests` 73/73,
      `Model.InSkillPurchasing.Tests` 10/10, no stray `.received.*` files.
- [x] Validated solution build: 0 errors.

Suggested commit message:

```text
test(apl,isp): add envelope- and component-level tests from real payloads
```

### Parity audit (between Commit 3 and the coverage-buildout commits)

Status: Done.

Before assuming Commits 2-3 were "enough," did an honest method-count + subject audit of both Legacy
projects against the new suites. Result: **not close to parity**. Legacy has ~117 test methods in
`Model.Legacy.Tests` and ~136 in `Model.Apl.Legacy.Tests`; Commits 2-3 added ~102 across all three
non-legacy projects, concentrated on the shapes this specific skill actually sends/receives. Large
legacy subsystems have zero coverage in the new suites. Full gap inventory (method counts, which
request/directive/component types are covered vs. not) is preserved in this session's history; the
commits below are scoped directly from it. This blocks Commit 14 (Legacy removal) until closed out —
per Constraints, Legacy only gets deleted once new coverage is equivalent-or-better.

Most of this remaining work has **no real CloudWatch capture behind it** — the trivia skill never
exercises AudioPlayer, legacy Display templates, VideoApp, DataStore, generic print/task Connections,
or most APL extensions/commands/components. Per Constraints, these get Compono/AutoFixture-generated
synthetic data instead, matching legacy's own construction style (many Legacy tests already hand-build
or property-test these objects rather than deserializing a capture).

### Commit 4: Model.Tests — response construction surface (directives, speech, remaining cards, progressive response)

Status: Done.

Tasks:

- [x] Ported `Responses/ResponseTests.cs`'s response-side content as serialize tests (construct in
      code, `TestHelper.VerifySerializedObject`, Verify the JSON) rather than legacy's
      `JsonElementDeepEquals` string-comparison style: `HintDirective`
      (`Response/DirectiveTests.cs`), the 4 Dialog directives (`DialogConfirmIntent`,
      `DialogConfirmSlot`, `DialogDelegate`, `DialogElicitSlot`, same file), `AskForPermissionDirective`
      (same file), `PlainTextOutputSpeech`/`SsmlOutputSpeech` plain + `PlayBehavior` variants
      (`Response/OutputSpeechTests.cs`), `Reprompt` string/Ssml constructors + the
      `SsmlOutputSpeech(string)` constructor (`Response/RepromptTests.cs`), the full example response
      (speech + card + session attributes) added to `Response/SkillResponseTests.cs`.
      `JsonDirective`'s round-trip test and the `ResponseBuilder.Tell`/directive-override test landed
      in a new `Response/JsonDirectiveTests.cs` — `JsonDirective` is a deliberate exception to the
      response-side-serialize-only rule (documented in that file) since it's the generic
      unknown-directive escape hatch, genuinely used both ways by design, not because
      `System.Text.Json` merely happens to allow it.
- [x] Extended `Response/CardTests.cs` with the 3 remaining card types (`StandardCard`,
      `LinkAccountCard`, `AskForPermissionsConsentCard`) — hand-constructed with realistic literal
      values rather than Compono-generated data, matching the style already established in this file
      and `BuyDirectiveTests` (avoids adding composition machinery for 3 more simple POCOs).
- [x] Ported `Responses/ProgressiveResponseTests.cs` as `Response/ProgressiveResponseTests.cs`: the
      `VoicePlayerSpeakDirective`/`ProgressiveResponseRequest` serialize tests, the `Send()`
      null-guard behavioral tests, `CanSend()`, and the 3 HTTP-behavior tests (base address, endpoint
      path, Bearer auth header) — the latter rewritten from a bespoke `ActionMessageHandler` to
      `Compono.Http.TestHttpHandler` directly (matches this repo's established HTTP test-double
      convention from plan 0001; added a `Compono.Http` package reference to `Model.Tests.csproj`,
      which didn't have one yet).
- [x] Found and fixed a real bug along the way: `AskForPermissionDirective`'s constructors set
      `Payload` but never `Name`, even though its own `AskForPermissionDirectiveHandler` discriminates
      incoming `Connections.Response` traffic on `name == "AskFor"` — building this directive the
      normal way produced `"name": null`. Fixed by setting `Name = "AskFor"` in both constructors
      (small, additive, non-breaking).
- [x] Validated all 4 TFMs: 49/49 passing, no stray `.received.*` files. Validated solution build:
      0 errors.

Suggested commit message:

```text
test(model): add response construction coverage (directives, speech, cards, progressive response)
```

### Commit 5: Model.Tests — SSML builder

Status: Done.

Tasks:

- [x] Ported the 21 explicit-value `[Fact]` tests from `Speech/SsmlTests.cs` into
      `Response/SsmlTests.cs`: `Speech` (empty-throws, `<speak>` wrapping), `PlainText`, `Sentence`,
      `Paragraph`, `Break` (plain/time/strength), `SayAs` (plain/format), `Word`, `Sub`, `Prosody`,
      `Emphasis`, `Phoneme`, `Audio`, `AmazonEffect`, terse-vs-verbose construction parity,
      `Voice`+`Lang`, `AlexaName`, `AmazonDomain`, `AmazonEmotion`.
- [x] `ISsml.ToXml()` is a one-directional string/XML builder with no deserialize counterpart at all
      (no `FromXml`), so the request/response envelope-role direction rule from Constraints doesn't
      apply here — there's nothing to be "strict about," only one direction exists.
- [x] Deliberately did not port the ~14 Compono `[Theory]`-generated-data "shape" tests from the same
      legacy file (e.g. `Word_WithGeneratedData_HasValidRole` asserting `StartsWith("<w")` /
      `Contains("role=")`) — they're weaker, redundant checks on the same types the explicit tests
      already assert exact output for. Recorded here as a deliberate simplification, not a silent gap.
- [x] Validated all 4 TFMs: 71/71 passing (22 new, all passed on first run — no Verify snapshots
      needed, plain string-equality assertions). Validated solution build: 0 errors.

Suggested commit message:

```text
test(model): add SSML builder coverage
```

### Commit 6: Model.Tests — legacy-interface directives (AudioPlayer, Display, VideoApp)

Status: Done.

Tasks:

- [x] Ported `Directives/AudioPlayerDirectiveTests.cs`'s serialize coverage into
      `Response/AudioPlayerDirectiveTests.cs`: `AudioPlayerPlayDirective` (with/without metadata),
      `ClearQueueDirective`, `StopDirective`. Dropped the legacy file's deserialize tests and Compono
      property-based "shape" theories (same reasoning as Commits 4-5: response-side is serialize-only,
      and the shape theories duplicate what the explicit serialize tests already assert).
- [x] Ported `Directives/DisplayDirectiveTests.cs` (legacy pre-APL display templates) into
      `Response/DisplayTemplateTests.cs`: `TemplateImage` basic + with size/dimensions. (Its
      `HintDirective` test was already covered in Commit 4.)
- [x] Ported `Directives/VideoAppDirectiveTests.cs` into `Response/VideoAppDirectiveTests.cs`:
      `VideoAppDirective` serialize (object-initializer + `FromSource` constructor), plus the 3
      `IEndSessionDirective` override-behavior tests (overrides to null, stays null when directives
      agree, reverts to explicit when directives contradict) — general `ResponseBody.ShouldEndSession`
      logic, not really AudioPlayer/Display/VideoApp-specific, but this is where legacy had it.
- [x] All three interfaces are unused by the trivia skill (no real captures exist) — synthetic
      construction only, matching legacy's hand-written style.
- [x] Validated all 4 TFMs: 82/82 passing, no stray `.received.*` files. Validated solution build:
      0 errors.

Suggested commit message:

```text
test(model): add AudioPlayer/Display/VideoApp directive coverage
```

### Commit 7: Model.Tests — Connection Tasks

Status: Done.

Tasks:

- [x] Ported the response-side half of `ConnectionTasks/SkillConnectionTests.cs` into
      `Response/ConnectionTaskDirectiveTests.cs`: `PrintPdfV1`, `PrintImageV1`, `PrintWebPageV1`,
      `ScheduleTaxiReservation`, `ScheduleFoodEstablishmentReservation` (with `OnComplete`),
      `PinConfirmation` — all via `.ToConnectionDirective()` — plus `CompleteTaskDirective`. Dropped
      legacy's `StartConnectionDirective` deserialize assertions per the response-side-serialize rule.
- [x] Ported the request-side half into `Request/ConnectionTaskRequestTests.cs`:
      `SessionResumedRequest` deserialize, `LaunchRequest.Task` deserialize (built-in `PrintPdfV1`
      task and a custom task type via the `ConnectionTaskConverter.AddToConnectionTaskResolvers`
      extensibility hook — added a project-local `ExampleConnectionTask`/`ExampleConnectionTaskResolver`
      under `Infrastructure/`, mirroring legacy's `ExampleTask`/`ExampleTaskResolver`), and
      `PinConfirmationResolver.ResultFromSessionResumed` (deserializes a `SessionResumedRequest`
      whose `Cause.Result` carries a PIN-confirmation payload, then resolves it — passed immediately,
      confirming the object→dictionary conversion this depends on still works).
- [x] All fixtures built fresh (not copied from Legacy) under `Examples/Requests/`:
      `SessionResumedRequest.json`, `LaunchRequestWithTask.json`, `LaunchRequestWithCustomTask.json`,
      `SessionResumedRequestWithPinConfirmationResult.json`. Generic print/task connections are
      unused by the trivia skill, so these are synthetic placeholder values, not real captures.
- [x] **Scope note**: while reading the rest of `Requests/RequestTests.cs` to close its remaining
      gaps in this commit, found the actual uncovered surface is far larger than the earlier survey
      suggested — not just `SessionResumedRequest`/custom-request-type/epoch-timestamp, but also
      `IntentSignature` parsing (built-in intents with properties), `SkillEvent` requests (3 kinds),
      `DialogState`, `ConfirmationStatus` on `Intent`/`Slot`, `RequestVerification` timestamp-tolerance
      behavior, `Geolocation`, `Person` info, `AskForPermissionRequest` (the request-side counterpart
      of the `AskForPermissionDirective` bug fixed in Commit 4), `MultiValueSlot`, and SmartProperties
      (`Unit`/`PersistentUnitID`/`PersistentEndpointID`). Split this out to its own Commit 8 rather
      than cram it in here — this file alone is close to the size of everything else in Commits 4-6
      combined.
- [x] Validated all 4 TFMs: 93/93 passing, no stray `.received.*` files. Validated solution build:
      0 errors.

Suggested commit message:

```text
test(model): add Connection Tasks coverage
```

### Commit 8: Model.Tests — remaining RequestTests.cs gaps (IntentSignature, SkillEvents, Geolocation, Person, AskForPermissionRequest, and more)

Status: Not started.

Tasks:

- [ ] `IntentSignature` parsing: plain intent name/signature/action, and a built-in intent
      (`AMAZON.AddAction`-shaped) with namespace + multiple properties (entity/property pairs).
      Request-side, deserialize.
- [ ] `DialogState` on `IntentRequest`, `ConfirmationStatus` on `Intent` and on `Slot` — likely
      addable as assertions on existing `IntentRequest_*` fixtures already in
      `Request/SkillRequestTests.cs`/`IntentTests.cs` rather than new fixtures, if any already carry
      dialog state or a denied/confirmed slot; otherwise a small new fixture.
- [ ] Custom/unknown request-type extensibility hook (`RequestConverter.RegisterRequestTypeResolver`)
      — port the `NewIntentRequestTypeResolver`/`NewIntentRequest` pattern with a project-local
      equivalent under `Infrastructure/`, matching the `ExampleConnectionTaskResolver` approach from
      Commit 7.
- [ ] Epoch-timestamp `LaunchRequest` parsing variant. Request-side, deserialize.
- [ ] `RequestVerification.RequestTimestampWithinTolerance` behavior (in-tolerance and replay-attack
      cases) — functional test, not serialize/deserialize.
- [ ] `Geolocation` deserialization (location services status, coordinate, altitude, heading, speed).
- [ ] `Context.System.Person` deserialization (person id, access token, authentication confidence).
- [ ] `SkillEvent` requests: `AccountLinkSkillEventRequest`, `PermissionSkillEventRequest` (with
      `EventCreationTime`/`EventPublishingTime`), and the non-specialized `SkillEventRequest` fallback.
- [ ] `AskForPermissionRequest` deserialization — the request-side counterpart of
      `AskForPermissionDirective` (fixed in Commit 4): a `Connections.Response` with `name == "AskFor"`
      carrying a `PermissionStatus` and permission scope. Worth double-checking this round-trips
      correctly now that the directive-side `Name` bug is fixed.
- [ ] `MultiValueSlot` deserialization (a slot with multiple resolved values).
- [ ] SmartProperties support: `Context.System.Unit` (`UnitID`, `PersistentUnitID`) and
      `Context.System.Device.PersistentEndpointID`.
- [ ] All fixtures built fresh (not copied from Legacy).
- [ ] Validate all 4 TFMs, solution build.

Suggested commit message:

```text
test(model): add remaining request coverage (signatures, skill events, geolocation, person, permissions)
```

### Commit 9: Model.Apl.Tests — remaining APL commands

Status: Not started.

Tasks:

- [ ] Port the ~10 command types in `APLCommandTests.cs` not already covered by the
      `ExecuteCommandsDirectiveTests` sequence (`Sequential`/`SpeakItem`/`SpeakList` are done):
      `Parallel`, `SetValue`, `SetState`, `SendEvent`, `AnimateItem`, and the rest. Response-side
      serialize, component-level, under `Command/` (new folder, mirrors `Directive/`).
- [ ] Validate all 4 TFMs, solution build.

Suggested commit message:

```text
test(model-apl): add remaining APL command coverage
```

### Commit 10: Model.Apl.Tests — remaining APL components

Status: Not started.

Tasks:

- [ ] Port the ~37 component types in `ComponentTests.cs` not already covered by the pre-existing
      `Components/ContainerTests.cs`/`FrameTests.cs`/`SpacerTests.cs`: `Text`, `Image`, `Pager`,
      `ScrollView`, `TouchWrapper`, the `VectorGraphic` component, etc. Response-side serialize,
      component-level, added to `Components/` following the existing per-type file convention. This
      is the single largest gap (40 legacy tests) — fine to split across more than one working
      session/sub-commit if needed, but land it as this numbered commit (or 9a/9b if split).
- [ ] Validate all 4 TFMs, solution build.

Suggested commit message:

```text
test(model-apl): add remaining APL component coverage
```

### Commit 11: Model.Apl.Tests — APLDocument, Package, Layout, Gradient, VectorGraphic document

Status: Not started.

Tasks:

- [ ] Port `APLDocumentTests.cs` (15 tests): full document feature surface — imports, resources,
      styles, settings, mainTemplate — beyond the trivial one-component document in
      `RenderDocumentDirectiveTests`.
- [ ] Port `APLPackageTests.cs` (1 test: `APLDocumentLink` external package reference).
- [ ] Port `LayoutTests.cs` (8 tests: `Layout` parameters/bindings/items).
- [ ] Port `GradientTest.cs` (1 test) and `VectorGraphicTests.cs` (1 test: AVG document construction).
- [ ] All response-side serialize, component-level.
- [ ] Validate all 4 TFMs, solution build.

Suggested commit message:

```text
test(model-apl): add APLDocument/Package/Layout/Gradient/VectorGraphic coverage
```

### Commit 12: Model.Apl.Tests — DataStore and remaining APL request types

Status: Not started.

Tasks:

- [ ] Port `DataStoreClientTests.cs` (4 tests) and `DataStoreCommandTests.cs` (5 tests:
      `SendIndexListDataDirective`/`SendTokenListDataDirective`/`UpdateIndexListDataDirective`) as
      response-side serialize.
- [ ] Port `AudioTests.cs` (8 tests: APL `Audio` component/track config) as response-side serialize.
- [ ] Close the remaining `RequestTests.cs` (Apl) gaps beyond `UserEventRequest` (already covered):
      `LoadIndexListDataRequest`, `LoadTokenListDataRequest`, `RuntimeErrorRequest`,
      `DataStoreErrorRequest`/`InstallationErrorRequest`, `UsagesInstalledRequest`,
      `UsagesRemovedRequest`, `UpdateRequest`. All request-side, deserialize, added to
      `APLSkillRequestTests`.
- [ ] Validate all 4 TFMs, solution build.

Suggested commit message:

```text
test(model-apl): add DataStore and remaining APL request-type coverage
```

### Commit 13: Model.Apl.Tests — Extensions

Status: Not started.

Tasks:

- [ ] Port `ExtensionTests.cs` (22 tests): `BackStack`, `EntitySensing`, `SmartMotion` extension
      settings/directives (response-side serialize) and extension event requests (request-side
      deserialize, where applicable) — check each test individually for which side it's actually
      exercising rather than assuming uniformly.
- [ ] Validate all 4 TFMs, solution build.

Suggested commit message:

```text
test(model-apl): add APL extension coverage
```

### Commit 14: Remove Legacy test projects

Status: Not started. Blocked on Commits 4-12.

Tasks:

- [ ] Re-run the parity audit (method-count + subject comparison) to confirm Commits 4-12 actually
      closed every gap identified above — don't just assume the commit list was exhaustive.
- [ ] Confirm new suites are a superset of Legacy suite coverage by running both side by side.
- [ ] Delete `test/AlexaVoxCraft.Model.Legacy.Tests` and `test/AlexaVoxCraft.Model.Apl.Legacy.Tests`,
      including their `Examples/*.json` fixtures.
- [ ] Remove both projects from `AlexaVoxCraft.slnx`.
- [ ] Validate solution build and full test suite.

Suggested commit message:

```text
test: remove Legacy test projects superseded by Verify suites
```

### Commit 15: Mechanical warning fixes (SYSLIB0057, CS0618)

Status: Not started.

Tasks:

- [ ] Replace obsolete `X509Certificate2` constructor usage with `X509CertificateLoader`
      (SYSLIB0057, 6 warnings).
- [ ] Replace or scope-suppress obsolete API usage (CS0618, 4 warnings) with a one-line reason
      comment where suppression is the right call.
- [ ] Validate solution build shows these categories at zero.
- [ ] Validate full test suite.

Suggested commit message:

```text
fix: resolve obsolete-API compiler warnings
```

### Commit 16: CS0108/CS0114 member-hiding fixes

Status: Not started.

Tasks:

- [ ] Review each CS0108 (hides inherited member, 120 warnings) and CS0114 (hides inherited member,
      missing override, 2 warnings) site individually — do not blindly add `new` everywhere.
- [ ] Add explicit `new` where shadowing is intentional, `override` where it was a missed override.
- [ ] Validate solution build shows these categories at zero.
- [ ] Validate full test suite, paying attention to any behavior change from switching hide to override.

Suggested commit message:

```text
fix: resolve member-hiding compiler warnings
```

### Commit 17: Analyzer release tracking (RS2008)

Status: Not started.

Tasks:

- [ ] Add missing `AnalyzerReleases.Shipped.md` / `AnalyzerReleases.Unshipped.md` entries for the
      MediatR source generator project (RS2008, 6 warnings).
- [ ] Validate solution build shows this category at zero.

Suggested commit message:

```text
chore(generator): add analyzer release tracking files
```

### Commit 18: Nullable warning cleanup - AlexaVoxCraft.Model

Status: Not started.

Tasks:

- [ ] Fix CS8618 (uninitialized non-nullable member) via constructor-required init, `required`
      modifier, sensible default, or nullable annotation only where the field is genuinely optional
      per the Alexa schema — cross-check against Commit 1's real payload examples.
- [ ] Fix CS8600/CS8601/CS8602/CS8603/CS8604/CS8619/CS8625/CS8765/CS8767 via proper null-checks/
      guard clauses or correct annotations; avoid `!` null-forgiving unless truly guaranteed.
- [ ] Validate `dotnet test test/AlexaVoxCraft.Model.Tests/AlexaVoxCraft.Model.Tests.csproj --no-build --no-restore`.
- [ ] Validate solution build warning count for `AlexaVoxCraft.Model` is zero.

Suggested commit message:

```text
fix(model): resolve nullable-reference compiler warnings
```

### Commit 19: Nullable warning cleanup - AlexaVoxCraft.Model.Apl

Status: Not started.

Tasks:

- [ ] Same approach as Commit 8, scoped to `AlexaVoxCraft.Model.Apl`.
- [ ] Validate `dotnet test test/AlexaVoxCraft.Model.Apl.Tests/AlexaVoxCraft.Model.Apl.Tests.csproj --no-build --no-restore`.
- [ ] Validate solution build warning count for `AlexaVoxCraft.Model.Apl` is zero.

Suggested commit message:

```text
fix(model-apl): resolve nullable-reference compiler warnings
```

### Commit 20: Nullable warning cleanup - remaining projects

Status: Not started.

Tasks:

- [ ] Inventory remaining warnings across `AlexaVoxCraft.MediatR`, `AlexaVoxCraft.MediatR.Lambda`,
      and any other project still emitting warnings.
- [ ] Fix per-project, smallest project first.
- [ ] Validate each affected project's test suite after its fixes.
- [ ] Validate full solution build shows 0 warnings / 0 errors.

Suggested commit message:

```text
fix: resolve remaining compiler warnings
```

## Resulting test composition guidance

- Prefer real captured payload shapes for round-trip coverage of what the skill actually sends and
  receives in production; use Compono/AutoFixture-generated data only to fill gaps the logs don't
  naturally produce (error paths, rare directive types, ISP edge cases).
- Use Verify snapshots for full serialized-shape regression, `AwesomeAssertions` for targeted
  object/constraint checks, matching the pattern already established in `Model.Tests`/`Model.Apl.Tests`.
- Never commit a captured fixture without a manual PII grep pass, even after automated scrubbing.
- Two tiers per subject: envelope-level (`SkillRequestTests`, `SkillResponseTests`, ...) and
  component-level (`IntentTests`, `CardTests`, `Directive/*Tests`, ...). Neither name nor folder
  references where the data came from.
- Test direction follows envelope role: request-side deserializes (fixture files under
  `Examples/Requests/`), response-side serializes (values inlined in code, no fixture file — a
  captured payload is still useful as the source of realistic values even when it can't be the literal
  test input). Verify this holds before writing a test for a new type — don't assume a type supports
  a direction just because the underlying serializer happens to allow it.

## Validation policy

Each commit should include:

- [ ] The targeted test project green.
- [ ] `dotnet build AlexaVoxCraft.slnx --no-restore` green.
- [ ] No accidental real user/device/session IDs, `.received.*`, `bin/`, or `obj/` files staged.

Full suite validation is required before Commit 14 (Legacy project removal) and before each nullable
cleanup commit (18-20), since those are the commits most likely to silently change behavior.
