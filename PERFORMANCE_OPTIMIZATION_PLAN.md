# AlexaVoxCraft Performance Optimization Plan

**Generated:** 2025-01-22  
**Target:** Major version update with breaking changes  
**Primary Goal:** Reduce AWS Lambda cold start times by 30-50%

## Phase 1: JSON Source Generation (HIGH IMPACT 🔥🔥🔥)
**Target:** Reduce Lambda cold start by 30-50%

### 1.1 Create Comprehensive JSON Contexts

#### AlexaJsonContext in `AlexaVoxCraft.Model` (~150 types)
**Core Request Types:**
- `SkillRequest` - Base request envelope
- `LaunchRequest`, `IntentRequest`, `SessionEndedRequest` - Intent handling
- `AudioPlayerRequest`, `PlaybackControllerRequest` - Audio playback
- `ConnectionResponseRequest`, `AskForPermissionRequest` - Skill connections
- `SkillEventRequest`, `AccountLinkSkillEventRequest` - Account events
- `DisplayElementSelectedRequest`, `SystemExceptionRequest` - Device interactions

**Core Response Types:**
- `SkillResponse` - Top-level response envelope  
- `ResponseBody`, `Reprompt` - Response structure
- `SimpleCard`, `StandardCard`, `LinkAccountCard`, `AskForPermissionsConsentCard` - Cards
- `PlainTextOutputSpeech`, `SsmlOutputSpeech` - Speech output
- `CardImage`, `ImageSource` - Media content

**Directive Types:**
- `AudioPlayerPlayDirective`, `StopDirective`, `ClearQueueDirective` - Audio control
- `DialogDelegate`, `DialogConfirmIntent`, `DialogElicitSlot` - Dialog management
- `VideoAppDirective`, `HintDirective` - UI directives
- `StartConnectionDirective`, `CompleteTaskDirective` - Task connections

**SSML Types:**
- `Speech`, `Audio`, `Prosody`, `Voice` - Speech synthesis
- `Paragraph`, `Sentence`, `PlainText` - Text structure
- `AmazonEmotion`, `AmazonDomain`, `Lang` - Amazon-specific elements

**Supporting Types:**
- `Intent`, `Slot`, `Context`, `Session` - Core Alexa concepts
- `AudioItem`, `VideoItem`, `Hint` - Media and UI items
- All enum types and value objects

#### AplJsonContext in `AlexaVoxCraft.Model.Apl` (~100 types)
**Core APL Components:**
- `Container`, `Text`, `Image`, `Frame` - Basic layout
- `Sequence`, `Pager`, `ScrollView`, `GridSequence` - List containers  
- `TouchWrapper`, `VectorGraphic`, `Video` - Interactive elements
- `EditText` - Input components

**Alexa Design System Components:**
- `AlexaHeader`, `AlexaFooter`, `AlexaButton` - Layout components
- `AlexaCard`, `AlexaImage`, `AlexaIcon` - Content components
- `AlexaTextList`, `AlexaImageList`, `AlexaPaginatedList` - List components
- `AlexaProgressBar`, `AlexaSlider`, `AlexaRating` - Control components
- `AlexaCheckbox`, `AlexaRadioButton`, `AlexaSwitch` - Input components

**APL Commands:**
- `AnimateItem`, `SetValue`, `SetPage` - Property manipulation
- `ScrollToIndex`, `ScrollToComponent`, `Scroll` - Navigation
- `SendEvent`, `OpenURL`, `Finish` - Event handling
- `PlayMedia`, `ControlMedia` - Media control
- `Sequential`, `Parallel` - Command composition
- `SpeakItem`, `SpeakList`, `Select` - Accessibility

**Supporting APL Types:**
- `APLDocument`, `APLValue<T>`, `APLDimensionValue` - Core types
- `APLExtension`, `APLPackage`, `APLTransformer` - Extensions
- All APL-specific enums and value objects

### 1.2 Implementation Details

```csharp
// File: src/AlexaVoxCraft.Model/Serialization/AlexaJsonContext.cs
[JsonSerializerContext]
[JsonSourceGenerationOptions(
    GenerationMode = JsonSourceGenerationMode.Default,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(SkillRequest))]
[JsonSerializable(typeof(SkillResponse))]
// ... all ~150 types listed above
public partial class AlexaJsonContext : JsonSerializerContext { }

// File: src/AlexaVoxCraft.Model.Apl/Serialization/AplJsonContext.cs  
[JsonSerializerContext]
[JsonSourceGenerationOptions(
    GenerationMode = JsonSourceGenerationMode.Default,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(APLDocument))]
[JsonSerializable(typeof(Container))]
// ... all ~100 APL types listed above  
public partial class AplJsonContext : JsonSerializerContext { }
```

### 1.3 Update AlexaJsonOptions Integration

```csharp
// File: src/AlexaVoxCraft.Model/Serialization/AlexaJsonOptions.cs
public static JsonSerializerOptions DefaultOptions
{
    get
    {
        var options = new JsonSerializerOptions
        {
            TypeInfoResolver = JsonTypeInfoResolver.Combine(
                AlexaJsonContext.Default,
                AplJsonContext.Default,  // New
                new AlexaTypeResolver()  // Existing custom resolver
            )
        };
        // ... rest of configuration
    }
}
```

## Phase 2: Critical Memory Optimizations (HIGH IMPACT 🔥🔥)

### 2.1 Fix AlexaJsonOptions Caching (BREAKING CHANGE)
**File:** `src/AlexaVoxCraft.Model/Serialization/AlexaJsonOptions.cs:14-42`

**Problem:** `DefaultOptions` property creates new `JsonSerializerOptions` on every access
**Impact:** Unnecessary object creation and configuration overhead

```csharp
// Before - creates new instance every time
public static JsonSerializerOptions DefaultOptions
{
    get
    {
        var resolver = new AlexaTypeResolver();
        // ... expensive setup every time
    }
}

// After - cached singleton
private static readonly Lazy<JsonSerializerOptions> _defaultOptions = new(() =>
{
    var resolver = new AlexaTypeResolver();
    // ... same logic, cached result
    return options;
});

public static JsonSerializerOptions DefaultOptions => _defaultOptions.Value;
```

**Breaking Change:** Consumers can no longer mutate the returned options instance

### 2.2 Fix ServiceRegistrar Allocations
**File:** `src/AlexaVoxCraft.MediatR/Registration/ServiceRegistrar.cs:20-47`

**Problem:** Multiple unnecessary `.ToList()` calls in reflection-heavy DI registration
**Impact:** Extra allocations during container startup

```csharp
// Before - unnecessary ToList() in hot path
var defaultHandlers = assembliesToScan.SelectMany(a => a.DefinedTypes)
    .Where(x => x.CanBeCastTo(typeof(IDefaultRequestHandler)) && x.IsConcrete()).ToList();

foreach (var defaultHandler in defaultHandlers)
{
    services.TryAddTransient(typeof(IDefaultRequestHandler), defaultHandler);
}

// After - direct enumeration  
foreach (var defaultHandler in assembliesToScan.SelectMany(a => a.DefinedTypes)
    .Where(x => x.CanBeCastTo(typeof(IDefaultRequestHandler)) && x.IsConcrete()))
{
    services.TryAddTransient(typeof(IDefaultRequestHandler), defaultHandler);
}
```

**Similar fixes needed at lines:** 28-29, 45-47, 79

## Phase 3: Collection Expression Modernization (MEDIUM IMPACT 🔥)

### 3.1 Update ~40+ Collection Initializations

**Pattern:** `new List<T>()` → `[]`, `new T[]{}` → `[]`, `new HashSet<T>()` → `[]`

**Key Files and Locations:**
- `src/AlexaVoxCraft.Model/Serialization/AlexaJsonOptions.cs:10,12`
- `src/AlexaVoxCraft.Model/Response/Ssml/*.cs` - All SSML constructors
- `src/AlexaVoxCraft.Model.Apl/Components/*.cs` - All APL component constructors  
- `src/AlexaVoxCraft.Model.Apl/Commands/*.cs` - Command constructors
- `src/AlexaVoxCraft.MediatR/Registration/ServiceRegistrar.cs` - Multiple locations

```csharp
// Before
private static readonly List<Action<JsonTypeInfo>> AdditionalModifiers = new List<Action<JsonTypeInfo>>();
Elements = elements.ToList();
Items = items.ToList();

// After  
private static readonly List<Action<JsonTypeInfo>> AdditionalModifiers = [];
Elements = [..elements];  // or just assign if already IEnumerable<T>
Items = [..items];
```

### 3.2 Remove Redundant `.ToList()` Calls

**Locations:**
- `AlexaJsonOptions.cs:22,34` - Remove in foreach loops (safe, no allocation needed)
- `ServiceRegistrar.cs:47,79` - Use direct enumeration
- APL component constructors - Replace with collection expressions

```csharp
// Before - unnecessary allocation
foreach (var modifier in AdditionalModifiers.ToList())
{
    resolver.Modifiers.Add(modifier);
}

// After - direct enumeration
foreach (var modifier in AdditionalModifiers)
{
    resolver.Modifiers.Add(modifier);
}
```

## Phase 4: Modern C# Patterns (MEDIUM IMPACT 🔥)

### 4.1 Pattern Matching Updates
**Files:** `DefaultResponseBuilder.cs`, various converters

```csharp
// Before
if (audioItemMetadata != null)
if (updatedIntent != null)

// After  
if (audioItemMetadata is not null)
if (updatedIntent is not null)
```

### 4.2 Target-Typed New Expressions
```csharp
// Before
var card = new StandardCard();
List<string> items = new List<string>();

// After
StandardCard card = new();
List<string> items = [];  // Collection expression preferred
```

## Phase 5: Documentation & Breaking Changes

### 5.1 Document Breaking Changes for Major Version

**Breaking Changes:**
1. **AlexaJsonOptions.DefaultOptions behavior change**
   - Now returns cached singleton instance instead of new instance
   - Mutations to returned options will affect all consumers
   - **Migration:** Create your own `JsonSerializerOptions` if mutation needed

2. **Potential source generation compatibility**
   - Custom converters may need verification with source generation
   - Polymorphic serialization behavior should remain the same

### 5.2 Performance Improvements Documentation
- Quantify Lambda cold start improvements
- Memory allocation reduction metrics
- Guidance on optimal usage patterns

## Testing Strategy

### 5.1 Existing Test Compatibility
- **All existing tests should pass** - changes preserve behavior
- JSON serialization round-trip tests verify source generation compatibility
- MediatR registration tests verify DI container setup

### 5.2 Source Generation Verification  
- Compare serialized JSON output between reflection and source generation
- Verify polymorphic converters work correctly
- Test all major request/response patterns

### 5.3 Collection Expression Testing
- Zero behavioral change expected
- Code review sufficient for syntax changes
- Existing unit tests cover functionality

## Implementation Order & Prioritization

### Priority 1 (Week 1): JSON Source Generation
1. Create `AlexaJsonContext` with core request/response types
2. Create `AplJsonContext` with APL types  
3. Update `AlexaJsonOptions` integration
4. Test serialization compatibility

### Priority 2 (Week 1): Critical Optimizations  
1. Fix `AlexaJsonOptions` caching (breaking change)
2. Optimize `ServiceRegistrar` allocations
3. Document breaking changes

### Priority 3 (Week 2): Collection Expressions
1. Update obvious collection initializations
2. Remove redundant `.ToList()` calls
3. APL component constructor updates

### Priority 4 (Week 2): Modern C# Patterns
1. Pattern matching updates
2. Target-typed new expressions
3. Final code review and cleanup

### Priority 5 (Week 3): Documentation & Release
1. Complete breaking change documentation
2. Performance improvement metrics
3. Migration guide
4. Major version release

## Expected Impact Summary

**Performance Improvements:**
- 🔥🔥🔥 **JSON Source Generation:** 30-50% Lambda cold start reduction
- 🔥🔥 **AlexaJsonOptions caching:** Eliminates repeated option creation overhead
- 🔥🔥 **ServiceRegistrar optimization:** Faster DI container startup  
- 🔥 **Collection expressions:** Reduced allocations, cleaner code

**Code Quality Improvements:**
- Modern C# 12 syntax throughout codebase
- Reduced memory allocations in hot paths
- Better maintainability with collection expressions
- Cleaner, more readable initialization patterns

**Total Estimated Types for Source Generation:** ~250+ types across both contexts

## Notes

- **No benchmarking required** per user preference
- **Object pooling evaluation** for APL components deferred (testing complexity)
- **ConfigureAwait usage already excellent** throughout codebase
- **Span<T> usage already optimal** in ResponseBuilder.cs TrimOutputSpeech method