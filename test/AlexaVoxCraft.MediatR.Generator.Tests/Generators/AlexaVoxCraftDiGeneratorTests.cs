using AlexaVoxCraft.MediatR.Generator.Tests.TestKit.Attributes;
using AlexaVoxCraft.MediatR.Generators.Generators;

namespace AlexaVoxCraft.MediatR.Generator.Tests.Generators;

/// <summary>
/// Tests for the AlexaVoxCraft dependency injection source generator.
/// Verifies that the generator correctly produces C# interceptors for AddSkillMediator calls.
/// </summary>
public class AlexaVoxCraftDiGeneratorTests
{
    private static readonly Dictionary<string, string>? FeatureFlags = new()
    {
        ["InterceptorsPreviewNamespaces"] = "AlexaVoxCraft.Generated",
        ["InterceptorsNamespaces"] = "AlexaVoxCraft.Generated",
    };

    private static readonly Dictionary<string, string> AnalyzerOpts = new()
    {
        ["build_property.EnableMediatRGeneratorInterceptor"] = "true"
    };

    /// <summary>
    /// Verifies that the generator produces an interceptor for a single AddSkillMediator call.
    /// The generated interceptor should have one InterceptsLocation attribute.
    /// </summary>
    [Theory]
    [GeneratorAutoData]
    public async Task GeneratesInterceptor_ForSingleAddSkillMediatorCall(AlexaVoxCraftDiGenerator sut)
    {
        await VerifyGlue.VerifySourcesAsync(sut,
            [
                "Cases/001_Simple/Function.cs"
            ],
            featureFlags: FeatureFlags,
            msbuildProperties: AnalyzerOpts
        );
    }
    
    /// <summary>
    /// Verifies that the generator produces a single interceptor method with multiple InterceptsLocation attributes
    /// when AddSkillMediator is called multiple times, demonstrating proper deduplication.
    /// </summary>
    [Theory]
    [GeneratorAutoData]
    public async Task GeneratesInterceptor_ForMultipleAddSkillMediatorCalls_WithMultipleInterceptsLocations(AlexaVoxCraftDiGenerator sut)
    {
        await VerifyGlue.VerifySourcesAsync(sut,
            [
                "Cases/002_MultiRegister/Function.cs"
            ],
            featureFlags: FeatureFlags,
            msbuildProperties: AnalyzerOpts
        );
    }

    /// <summary>
    /// Verifies that the generator produces an interceptor for a standard skill with request handlers and exception handlers.
    /// Tests that handler registration works correctly when handlers are present in the assembly.
    /// </summary>
    [Theory]
    [GeneratorAutoData]
    public async Task GeneratesInterceptor_ForStandardSkillWithHandlers(AlexaVoxCraftDiGenerator sut)
    {
        await VerifyGlue.VerifySourcesAsync(sut,
            [
                "Cases/003_Standard/Function.cs",
                "Cases/003_Standard/LaunchHandler.cs",
                "Cases/003_Standard/IntentHandler.cs",
                "Cases/003_Standard/ExceptionHandler.cs"
            ],
            featureFlags: FeatureFlags,
            msbuildProperties: AnalyzerOpts
        );
    }

    /// <summary>
    /// Verifies that the generator produces an interceptor for a skill with handlers and both request and response interceptors.
    /// Tests that the generator properly registers all pipeline components including request/response interceptors.
    /// </summary>
    [Theory]
    [GeneratorAutoData]
    public async Task GeneratesInterceptor_ForSkillWithHandlersAndInterceptors(AlexaVoxCraftDiGenerator sut)
    {
        await VerifyGlue.VerifySourcesAsync(sut,
            [
                "Cases/004_WithInterceptors/Function.cs",
                "Cases/004_WithInterceptors/LaunchHandler.cs",
                "Cases/004_WithInterceptors/HelpIntentHandler.cs",
                "Cases/004_WithInterceptors/LoggingRequestInterceptor.cs",
                "Cases/004_WithInterceptors/ResponseInterceptor.cs"
            ],
            featureFlags: FeatureFlags,
            msbuildProperties: AnalyzerOpts
        );
    }

    /// <summary>
    /// Verifies that the generator produces an interceptor for a complete skill with all components.
    /// Tests registration of handlers, exception handler, request/response interceptors, and persistence adapter.
    /// </summary>
    [Theory]
    [GeneratorAutoData]
    public async Task GeneratesInterceptor_ForCompleteSkillWithPersistence(AlexaVoxCraftDiGenerator sut)
    {
        await VerifyGlue.VerifySourcesAsync(sut,
            [
                "Cases/005_WithPersistence/Function.cs",
                "Cases/005_WithPersistence/LaunchHandler.cs",
                "Cases/005_WithPersistence/IntentHandler.cs",
                "Cases/005_WithPersistence/ExceptionHandler.cs",
                "Cases/005_WithPersistence/RequestInterceptor.cs",
                "Cases/005_WithPersistence/ResponseInterceptor.cs",
                "Cases/005_WithPersistence/DynamoDbPersistenceAdapter.cs"
            ],
            featureFlags: FeatureFlags,
            msbuildProperties: AnalyzerOpts
        );
    }

    /// <summary>
    /// Verifies that the generator properly handles a handler implementing IRequestHandler for multiple request types.
    /// Tests that handlers implementing multiple IRequestHandler interfaces are registered correctly.
    /// </summary>
    [Theory]
    [GeneratorAutoData]
    public async Task GeneratesInterceptor_ForHandlerWithMultipleRequestTypes(AlexaVoxCraftDiGenerator sut)
    {
        await VerifyGlue.VerifySourcesAsync(sut,
            [
                "Cases/006_MultiTypeHandler/Function.cs",
                "Cases/006_MultiTypeHandler/SessionEndedOrLaunchHandler.cs"
            ],
            featureFlags: FeatureFlags,
            msbuildProperties: AnalyzerOpts
        );
    }

    /// <summary>
    /// Verifies that the generator properly handles a class implementing both IRequestInterceptor and IResponseInterceptor.
    /// Tests that dual-purpose interceptors are registered correctly for both pipeline stages.
    /// </summary>
    [Theory]
    [GeneratorAutoData]
    public async Task GeneratesInterceptor_ForDualPurposeInterceptor(AlexaVoxCraftDiGenerator sut)
    {
        await VerifyGlue.VerifySourcesAsync(sut,
            [
                "Cases/007_DualInterceptor/Function.cs",
                "Cases/007_DualInterceptor/LoggingInterceptor.cs"
            ],
            featureFlags: FeatureFlags,
            msbuildProperties: AnalyzerOpts
        );
    }

    /// <summary>
    /// Verifies that the generator properly handles handlers with AlexaHandlerAttribute specifying execution order.
    /// Tests that handlers are registered with their explicit Order property values.
    /// </summary>
    [Theory]
    [GeneratorAutoData]
    public async Task GeneratesInterceptor_ForHandlersWithExplicitOrder(AlexaVoxCraftDiGenerator sut)
    {
        await VerifyGlue.VerifySourcesAsync(sut,
            [
                "Cases/008_OrderedHandlers/Function.cs",
                "Cases/008_OrderedHandlers/FirstLaunchHandler.cs",
                "Cases/008_OrderedHandlers/SecondIntentHandler.cs",
                "Cases/008_OrderedHandlers/ThirdFallbackHandler.cs"
            ],
            featureFlags: FeatureFlags,
            msbuildProperties: AnalyzerOpts
        );
    }

    /// <summary>
    /// Verifies that the generator properly handles handlers with different ServiceLifetime values.
    /// Tests that handlers are registered with Transient, Scoped, and Singleton lifetimes as specified.
    /// </summary>
    [Theory]
    [GeneratorAutoData]
    public async Task GeneratesInterceptor_ForHandlersWithDifferentLifetimes(AlexaVoxCraftDiGenerator sut)
    {
        await VerifyGlue.VerifySourcesAsync(sut,
            [
                "Cases/009_DifferentLifetimes/Function.cs",
                "Cases/009_DifferentLifetimes/TransientLaunchHandler.cs",
                "Cases/009_DifferentLifetimes/ScopedIntentHandler.cs",
                "Cases/009_DifferentLifetimes/SingletonSessionEndedHandler.cs"
            ],
            featureFlags: FeatureFlags,
            msbuildProperties: AnalyzerOpts
        );
    }

    /// <summary>
    /// Verifies that the generator properly excludes handlers marked with Exclude = true.
    /// Tests that handlers with AlexaHandlerAttribute(Exclude = true) are not registered automatically.
    /// </summary>
    [Theory]
    [GeneratorAutoData]
    public async Task GeneratesInterceptor_ExcludesHandlersMarkedWithExclude(AlexaVoxCraftDiGenerator sut)
    {
        await VerifyGlue.VerifySourcesAsync(sut,
            [
                "Cases/010_ExcludedHandler/Function.cs",
                "Cases/010_ExcludedHandler/LaunchHandler.cs",
                "Cases/010_ExcludedHandler/IntentHandler.cs",
                "Cases/010_ExcludedHandler/ExcludedSessionEndedHandler.cs"
            ],
            featureFlags: FeatureFlags,
            msbuildProperties: AnalyzerOpts
        );
    }

    /// <summary>
    /// Verifies that the generator properly handles handlers inheriting from an abstract base handler class.
    /// Tests that concrete handlers deriving from BaseHandler&lt;TRequest&gt; are registered, but the abstract base is not.
    /// </summary>
    [Theory]
    [GeneratorAutoData]
    public async Task GeneratesInterceptor_ForHandlersInheritingFromAbstractBase(AlexaVoxCraftDiGenerator sut)
    {
        await VerifyGlue.VerifySourcesAsync(sut,
            [
                "Cases/011_AbstractBaseHandler/Function.cs",
                "Cases/011_AbstractBaseHandler/BaseHandler.cs",
                "Cases/011_AbstractBaseHandler/LaunchHandler.cs",
                "Cases/011_AbstractBaseHandler/IntentHandler.cs",
                "Cases/011_AbstractBaseHandler/SessionEndedHandler.cs"
            ],
            featureFlags: FeatureFlags,
            msbuildProperties: AnalyzerOpts
        );
    }

    /// <summary>
    /// Verifies comprehensive trivia game skill with all generator features combined.
    /// Tests abstract base handler, multi-type handler, IDefaultRequestHandler, various lifetimes and ordering,
    /// excluded handler, dual interceptors, and persistence adapter in a single realistic scenario.
    /// </summary>
    [Theory]
    [GeneratorAutoData]
    public async Task GeneratesInterceptor_ForComprehensiveTriviaGame(AlexaVoxCraftDiGenerator sut)
    {
        await VerifyGlue.VerifySourcesAsync(sut,
            [
                "Cases/012_TriviaGame/Function.cs",
                "Cases/012_TriviaGame/BaseTriviaHandler.cs",
                "Cases/012_TriviaGame/LaunchTriviaHandler.cs",
                "Cases/012_TriviaGame/AnswerHandler.cs",
                "Cases/012_TriviaGame/HelpIntentHandler.cs",
                "Cases/012_TriviaGame/RepeatIntentHandler.cs",
                "Cases/012_TriviaGame/StopOrCancelHandler.cs",
                "Cases/012_TriviaGame/SessionEndedHandler.cs",
                "Cases/012_TriviaGame/DefaultTriviaHandler.cs",
                "Cases/012_TriviaGame/UnhandledIntentHandler.cs",
                "Cases/012_TriviaGame/TriviaExceptionHandler.cs",
                "Cases/012_TriviaGame/LanguageRequestInterceptor.cs",
                "Cases/012_TriviaGame/LoggingInterceptor.cs",
                "Cases/012_TriviaGame/SaveDataResponseInterceptor.cs",
                "Cases/012_TriviaGame/DynamoDbTriviaAdapter.cs"
            ],
            featureFlags: FeatureFlags,
            msbuildProperties: AnalyzerOpts
        );
    }
}