using AutoFixture;
using AutoFixture.Kernel;
using AutoFixture.Xunit3;
using NSubstitute;
using Amazon.Lambda.Core;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.MediatR.Lambda.Context;
using AlexaVoxCraft.MediatR.Lambda.Abstractions;
using System.Reflection;
using AlexaVoxCraft.Model.Request.Type;

namespace AlexaVoxCraft.MediatR.Lambda.Tests;

/// <summary>
/// Base class for all Lambda tests providing common setup and utilities.
/// </summary>
public abstract class TestBase
{ 
    protected T CreateSubstitute<T>() where T : class => Substitute.For<T>();
}

/// <summary>
/// AutoData attribute that uses a customized fixture for consistent test data generation.
/// </summary>
public class AutoDataAttribute : AutoFixture.Xunit3.AutoDataAttribute
{
    public AutoDataAttribute() : base(() => CreateFixture()) { }

    private static IFixture CreateFixture()
    {
        var fixture = new Fixture();
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        
        // Add abstract class mappings
        fixture.Customizations.Add(new TypeRelay(typeof(SkillContext), typeof(DefaultSkillContext)));
        
        fixture.Customizations.Add(new LambdaContextSpecimenBuilder());
        fixture.Customizations.Add(new SkillContextAccessorSpecimenBuilder());
        fixture.Customizations.Add(new RequestSpecimenBuilder());
        fixture.Customizations.Add(new SkillRequestSpecimenBuilder());
        fixture.Customizations.Add(new SkillResponseSpecimenBuilder());
        return fixture;
    }
}

/// <summary>
/// Custom specimen builder for ILambdaContext based on parameter naming conventions.
/// </summary>
public class LambdaContextSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is not ParameterInfo parameter || parameter.ParameterType != typeof(ILambdaContext))
            return new NoSpecimen();

        var lambdaContext = Substitute.For<ILambdaContext>();
        lambdaContext.AwsRequestId.Returns(Guid.NewGuid().ToString());
        lambdaContext.FunctionName.Returns("TestFunction");
        lambdaContext.FunctionVersion.Returns("1.0");
        lambdaContext.InvokedFunctionArn.Returns("arn:aws:lambda:us-east-1:123456789012:function:TestFunction");
        lambdaContext.MemoryLimitInMB.Returns(512);
        lambdaContext.RemainingTime.Returns(TimeSpan.FromMinutes(5));
        lambdaContext.LogGroupName.Returns("/aws/lambda/TestFunction");
        lambdaContext.LogStreamName.Returns("2023/10/15/[$LATEST]test");

        return lambdaContext;
    }
}

/// <summary>
/// Custom specimen builder for ISkillContextAccessor - creates substitutes for testing.
/// </summary>
public class SkillContextAccessorSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is not ParameterInfo parameter || parameter.ParameterType != typeof(ISkillContextAccessor))
            return new NoSpecimen();

        return Substitute.For<ISkillContextAccessor>();
    }
}

/// <summary>
/// Custom specimen builder for Request abstract class based on parameter naming conventions.
/// </summary>
public class RequestSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        switch (request)
        {
            // Handle direct Type requests for Request
            case Type type when type == typeof(Request):
                return CreateLaunchRequest(); // Default to launch request
            // Handle parameter requests for Request type with naming conventions
            case ParameterInfo parameter when parameter.ParameterType == typeof(Request):
            {
                var parameterName = parameter.Name?.ToLowerInvariant() ?? "";
            
                return parameterName switch
                {
                    _ when parameterName.Contains("launch") => CreateLaunchRequest(),
                    _ when parameterName.Contains("intent") => CreateIntentRequest(parameterName),
                    _ when parameterName.Contains("session") && parameterName.Contains("end") => CreateSessionEndedRequest(),
                    _ when parameterName.Contains("audioplayer") || parameterName.Contains("audio") => CreateAudioPlayerRequest(),
                    _ when parameterName.Contains("display") => CreateDisplayElementSelectedRequest(),
                    _ when parameterName.Contains("playback") => CreatePlaybackControllerRequest(),
                    _ when parameterName.Contains("system") => CreateSystemExceptionRequest(),
                    _ => CreateLaunchRequest() // Default to launch request
                };
            }
            default:
                return new NoSpecimen();
        }
    }

    private static LaunchRequest CreateLaunchRequest()
    {
        return new LaunchRequest
        {
            Type = "LaunchRequest",
            RequestId = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            Locale = "en-US"
        };
    }

    private static IntentRequest CreateIntentRequest(string parameterName)
    {
        // Determine intent name from parameter name
        var intentName = parameterName switch
        {
            _ when parameterName.Contains("help") => "AMAZON.HelpIntent",
            _ when parameterName.Contains("stop") => "AMAZON.StopIntent", 
            _ when parameterName.Contains("cancel") => "AMAZON.CancelIntent",
            _ when parameterName.Contains("yes") => "AMAZON.YesIntent",
            _ when parameterName.Contains("no") => "AMAZON.NoIntent",
            _ when parameterName.Contains("custom") => "CustomIntent",
            _ => "TestIntent"
        };
        
        return new IntentRequest
        {
            Type = "IntentRequest",
            RequestId = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            Locale = "en-US",
            Intent = new Intent
            {
                Name = intentName,
                ConfirmationStatus = "NONE"
            }
        };
    }

    private static SessionEndedRequest CreateSessionEndedRequest()
    {
        return new SessionEndedRequest
        {
            Type = "SessionEndedRequest",
            RequestId = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            Locale = "en-US",
            Reason = Reason.UserInitiated
        };
    }

    private static AudioPlayerRequest CreateAudioPlayerRequest()
    {
        return new AudioPlayerRequest
        {
            Type = "AudioPlayer.PlaybackStopped",
            RequestId = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            Locale = "en-US"
        };
    }

    private static DisplayElementSelectedRequest CreateDisplayElementSelectedRequest()
    {
        return new DisplayElementSelectedRequest
        {
            Type = "Display.ElementSelected",
            RequestId = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            Locale = "en-US"
        };
    }

    private static PlaybackControllerRequest CreatePlaybackControllerRequest()
    {
        return new PlaybackControllerRequest
        {
            Type = "PlaybackController.PlayCommandIssued",
            RequestId = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            Locale = "en-US"
        };
    }

    private static SystemExceptionRequest CreateSystemExceptionRequest()
    {
        return new SystemExceptionRequest
        {
            Type = "System.ExceptionEncountered",
            RequestId = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            Locale = "en-US"
        };
    }
}

/// <summary>
/// Custom specimen builder for SkillRequest based on parameter naming conventions.
/// </summary>
public class SkillRequestSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is not ParameterInfo parameter || !typeof(SkillRequest).IsAssignableFrom(parameter.ParameterType))
            return new NoSpecimen();

        // Create real objects instead of substitutes to avoid virtual member issues
        var application = new Application { ApplicationId = "amzn1.ask.skill.test-skill-id" };
        var system = new AlexaSystem { Application = application };
        var skillContext = new AlexaVoxCraft.Model.Request.Context { System = system };
        var session = new Session();
        
        // Create request based on parameter name using the same logic as RequestSpecimenBuilder
        var parameterName = parameter.Name?.ToLowerInvariant() ?? "";
        Request requestType = parameterName switch
        {
            _ when parameterName.Contains("launch") => CreateLaunchRequest(),
            _ when parameterName.Contains("intent") => CreateIntentRequest(parameterName),
            _ when parameterName.Contains("session") && parameterName.Contains("end") => CreateSessionEndedRequest(),
            _ when parameterName.Contains("audioplayer") || parameterName.Contains("audio") => CreateAudioPlayerRequest(),
            _ when parameterName.Contains("display") => CreateDisplayElementSelectedRequest(),
            _ when parameterName.Contains("playback") => CreatePlaybackControllerRequest(),
            _ when parameterName.Contains("system") => CreateSystemExceptionRequest(),
            _ => CreateLaunchRequest() // Default to launch request
        };

        var skillRequest = new SkillRequest 
        { 
            Context = skillContext,
            Request = requestType,
            Session = session
        };
        
        return skillRequest;
    }

    private static LaunchRequest CreateLaunchRequest()
    {
        return new LaunchRequest
        {
            Type = "LaunchRequest",
            RequestId = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            Locale = "en-US"
        };
    }

    private static IntentRequest CreateIntentRequest(string parameterName)
    {
        // Determine intent name from parameter name
        var intentName = parameterName switch
        {
            _ when parameterName.Contains("help") => "AMAZON.HelpIntent",
            _ when parameterName.Contains("stop") => "AMAZON.StopIntent", 
            _ when parameterName.Contains("cancel") => "AMAZON.CancelIntent",
            _ when parameterName.Contains("yes") => "AMAZON.YesIntent",
            _ when parameterName.Contains("no") => "AMAZON.NoIntent",
            _ when parameterName.Contains("custom") => "CustomIntent",
            _ => "TestIntent"
        };
        
        return new IntentRequest
        {
            Type = "IntentRequest",
            RequestId = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            Locale = "en-US",
            Intent = new Intent
            {
                Name = intentName,
                ConfirmationStatus = "NONE"
            }
        };
    }

    private static SessionEndedRequest CreateSessionEndedRequest()
    {
        return new SessionEndedRequest
        {
            Type = "SessionEndedRequest",
            RequestId = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            Locale = "en-US",
            Reason = Reason.UserInitiated
        };
    }

    private static AudioPlayerRequest CreateAudioPlayerRequest()
    {
        return new AudioPlayerRequest
        {
            Type = "AudioPlayer.PlaybackStopped",
            RequestId = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            Locale = "en-US"
        };
    }

    private static DisplayElementSelectedRequest CreateDisplayElementSelectedRequest()
    {
        return new DisplayElementSelectedRequest
        {
            Type = "Display.ElementSelected",
            RequestId = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            Locale = "en-US"
        };
    }

    private static PlaybackControllerRequest CreatePlaybackControllerRequest()
    {
        return new PlaybackControllerRequest
        {
            Type = "PlaybackController.PlayCommandIssued",
            RequestId = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            Locale = "en-US"
        };
    }

    private static SystemExceptionRequest CreateSystemExceptionRequest()
    {
        return new SystemExceptionRequest
        {
            Type = "System.ExceptionEncountered",
            RequestId = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            Locale = "en-US"
        };
    }
}

/// <summary>
/// Custom specimen builder for SkillResponse based on parameter naming conventions.
/// </summary>
public class SkillResponseSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is not ParameterInfo parameter || !typeof(SkillResponse).IsAssignableFrom(parameter.ParameterType))
            return new NoSpecimen();

        // Create real SkillResponse object instead of substitute
        var skillResponse = new SkillResponse();
        return skillResponse;
    }
}