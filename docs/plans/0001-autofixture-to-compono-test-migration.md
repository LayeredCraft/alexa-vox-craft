# Plan 0001: AutoFixture/NSubstitute to Compono test migration

## Goal

Remove AutoFixture-based test data generation and NSubstitute-based test double usage from the AlexaVoxCraft test suite, replacing them with Compono composition, explicit test data, and small purpose-built fakes where they produce clearer tests.

The goal is not to recreate AutoFixture's test-method shape. If a value is simple, parameterless, or irrelevant to the tested behavior, prefer local construction over composition.

## Constraints

- Keep each chunk small enough to validate and commit independently.
- Preserve test clarity over mechanical conversion.
- Do not reintroduce composition solely to avoid `new` for simple SUTs.
- Treat Verify snapshot identity improvements from removing irrelevant theory parameters as a feature, not a regression.
- Keep the solution compiling at every checkpoint.
- Prefer project-local helpers until repeated friction clearly justifies a reusable package or shared abstraction.

## Commit checkpoints

### Commit 0: Current mixed migration checkpoint

Status: In progress / ready to commit once validated.

Scope already present in the working tree:

- [x] Pin published Compono `0.8.0` packages.
- [x] Add `Compono.Bogus` package version for migrated test projects that need semantic test data.
- [x] Remove the custom `AlexaVoxCraft.Http.TestKit` project.
- [x] Migrate SMAPI HTTP tests to `Compono.Http.TestHttpHandler` via local `HttpTestHarness` and `SmapiHttpTestProfile`.
- [x] Migrate ISP HTTP tests to `Compono.Http.TestHttpHandler` via local `HttpTestHarness` and `IspHttpTestProfile`.
- [x] Convert simple ISP inline AutoFixture theories to explicit `[InlineData]`.
- [x] Convert MediatR generator tests from `GeneratorAutoDataAttribute` to explicit `new AlexaVoxCraftDiGenerator()`.
- [x] Rename generator Verify snapshots to remove the stale `_sut=...` theory-parameter identity component.
- [x] Convert an initial slice of Model Legacy tests from `ModelAutoData` to `Compose<ModelLegacyProfile>`.
- [x] Add project-local Model Legacy JSON test extensions.
- [x] Convert MediatR Lambda tests from `MediatRLambdaAutoData` to `Compose<LambdaTestProfile>` and local fakes where practical.
- [x] Keep `AlexaVoxCraft.TestKit` temporarily where still required by unmigrated tests/helpers.

Validation evidence:

- [x] `dotnet build AlexaVoxCraft.slnx --no-restore` passed with 0 warnings / 0 errors after fixes.
- [x] `dotnet test test/AlexaVoxCraft.MediatR.Generator.Tests/AlexaVoxCraft.MediatR.Generator.Tests.csproj --no-build --no-restore` passed: 84/84.
- [x] `dotnet test test/AlexaVoxCraft.MediatR.Lambda.Tests/AlexaVoxCraft.MediatR.Lambda.Tests.csproj --no-build --no-restore` passed: 284/284.
- [ ] Re-run targeted validations after Commit 0 is created if additional files are staged into it.

Suggested commit message:

```text
test: checkpoint Compono migration baseline
```

### Commit 1: Finish Model Legacy migration

Status: Not started.

Tasks:

- [ ] Inventory remaining `ModelAutoData`, `AutoData`, `Frozen`, and `AlexaVoxCraft.TestKit` usage in `AlexaVoxCraft.Model.Legacy.Tests`.
- [ ] Convert remaining AutoFixture-driven tests to `Compose<ModelLegacyProfile>` or explicit object construction.
- [ ] Replace misleading comments that say "AutoFixture-based tests" where tests are now Compono/explicit-data based.
- [ ] Remove the `AlexaVoxCraft.TestKit` project reference if no longer needed.
- [ ] Keep or refine project-local JSON helpers as needed.
- [ ] Validate `dotnet test test/AlexaVoxCraft.Model.Legacy.Tests/AlexaVoxCraft.Model.Legacy.Tests.csproj --no-build --no-restore`.
- [ ] Validate solution build.

Suggested commit message:

```text
test(model-legacy): complete Compono migration
```

### Commit 2: Finish MediatR Lambda TestKit removal

Status: Not started.

Tasks:

- [ ] Inventory remaining `AlexaVoxCraft.TestKit` global-using reliance in `AlexaVoxCraft.MediatR.Lambda.Tests`.
- [ ] Move/replace any required non-AutoFixture helpers locally, especially logging/test JSON helpers.
- [ ] Remove the `AlexaVoxCraft.TestKit` project reference from `AlexaVoxCraft.MediatR.Lambda.Tests`.
- [ ] Add explicit direct package/project references for anything previously only transitively available through TestKit.
- [ ] Validate `dotnet test test/AlexaVoxCraft.MediatR.Lambda.Tests/AlexaVoxCraft.MediatR.Lambda.Tests.csproj --no-build --no-restore`.
- [ ] Validate solution build.

Suggested commit message:

```text
test(lambda): remove TestKit dependency
```

### Commit 3: Finish MediatR tests migration

Status: Not started.

Tasks:

- [ ] Inventory remaining AutoFixture/NSubstitute/TestKit usage in `AlexaVoxCraft.MediatR.Tests`.
- [ ] Confirm current `MediatRTestProfile` and fake delegates cover the old AutoFixture/NSubstitute behavior intentionally.
- [ ] Replace any remaining NSubstitute-style assertions with Compono test doubles or purpose-built fakes.
- [ ] Remove stale comments unless they document an intentional Compono capability gap.
- [ ] Remove `AlexaVoxCraft.TestKit` project reference if no longer needed.
- [ ] Validate `dotnet test test/AlexaVoxCraft.MediatR.Tests/AlexaVoxCraft.MediatR.Tests.csproj --no-build --no-restore`.
- [ ] Validate solution build.

Suggested commit message:

```text
test(mediatr): complete Compono migration
```

### Commit 4: Migrate APL legacy / remaining model-adjacent AutoFixture usage

Status: Not started.

Tasks:

- [ ] Inventory `AlexaVoxCraft.Model.Apl.Legacy.Tests` AutoFixture usage.
- [ ] Convert only tests that actually depend on AutoFixture/TestKit behavior; leave unrelated tests alone.
- [ ] Add a local profile/helper only if repeated valid APL object construction needs it.
- [ ] Remove `AlexaVoxCraft.TestKit` reference from the project if no longer needed.
- [ ] Validate `dotnet test test/AlexaVoxCraft.Model.Apl.Legacy.Tests/AlexaVoxCraft.Model.Apl.Legacy.Tests.csproj --no-build --no-restore`.
- [ ] Validate solution build.

Suggested commit message:

```text
test(apl-legacy): migrate AutoFixture tests to Compono
```

### Commit 5: Remove shared AutoFixture/NSubstitute test infrastructure

Status: Not started.

Tasks:

- [ ] Confirm no test project depends on `AlexaVoxCraft.TestKit` for AutoFixture/NSubstitute behavior.
- [ ] Delete remaining `*AutoDataAttribute` files.
- [ ] Delete remaining AutoFixture specimen builders/customizations/specifications.
- [ ] Remove AutoFixture package references.
- [ ] Remove `AutoFixture`, `AutoFixture.Xunit3`, and `NSubstitute` global usings from `test/Directory.Build.props`.
- [ ] Remove NSubstitute package references where no longer used.
- [ ] Decide whether `AlexaVoxCraft.TestKit` should be deleted entirely or retained only for non-AutoFixture utilities.
- [ ] Validate `rg "AutoFixture|AutoData|InlineAutoData|Frozen|NSubstitute|Substitute\\.For|Received\\(|Arg\\." test -g '!bin/**' -g '!obj/**'` returns only intentional historical comments or no matches.
- [ ] Validate `dotnet build AlexaVoxCraft.slnx --no-restore`.
- [ ] Validate `dotnet test AlexaVoxCraft.slnx --no-build --no-restore`.

Suggested commit message:

```text
test: remove AutoFixture and NSubstitute infrastructure
```

### Commit 6: Documentation and cleanup

Status: Not started.

Tasks:

- [ ] Remove stale migration comments that are no longer useful after the final cutover.
- [ ] Add/update test documentation if the repo has a suitable location for test composition conventions.
- [ ] Document when to use explicit construction vs. Compono profiles vs. purpose-built fakes.
- [ ] Validate solution build and full test suite.

Suggested commit message:

```text
docs(testing): document Compono test migration patterns
```

## Current remaining usage inventory

Refresh this before each phase with:

```bash
rg "AutoFixture|AutoData|InlineAutoData|ModelAutoData|GeneratorAutoData|MediatRLambdaAutoData|Frozen|Substitute\.For|NSubstitute|Received\(|Arg\." test -g '!bin/**' -g '!obj/**'
```

As of plan creation, remaining matches include:

- `test/Directory.Build.props` global AutoFixture/NSubstitute usings.
- `test/AlexaVoxCraft.TestKit/**` AutoFixture/NSubstitute infrastructure.
- `test/AlexaVoxCraft.Model.Apl.Legacy.Tests/ExtensionTests.cs`.
- Migration comments in already-converted projects.
- MediatR test comments documenting known Compono capability gaps.

## Validation policy

Each commit should include:

- [ ] The targeted test project green.
- [ ] `dotnet build AlexaVoxCraft.slnx --no-restore` green.
- [ ] No accidental `.received.*`, `bin/`, or `obj/` files staged.

Full suite validation is required before the final infrastructure-removal commit and before pushing the completed migration branch.
