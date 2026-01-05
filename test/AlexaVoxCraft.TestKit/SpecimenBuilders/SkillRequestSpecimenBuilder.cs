using System.Reflection;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.TestKit.RequestSpecifications;
using AutoFixture.Kernel;

namespace AlexaVoxCraft.TestKit.SpecimenBuilders;

/// <summary>
/// Custom specimen builder for SkillRequest - creates valid instances for testing.
/// </summary>
public class SkillRequestSpecimenBuilder(IRequestSpecification requestSpecification) : ISpecimenBuilder
{
    public SkillRequestSpecimenBuilder() : this(new SkillRequestSpecification())
    {
    }

    public object Create(object request, ISpecimenContext context)
    {
        if (!requestSpecification.IsSatisfiedBy(request))
            return new NoSpecimen();

        var (requestType, parameterName) = request switch
        {
            ParameterInfo parameter => (parameter.ParameterType, parameter.Name?.ToLowerInvariant() ?? ""),
            Type type => (type, ""),
            _ => throw new ArgumentException("Invalid request type", nameof(request))
        };
        
        // Create request based on parameter name using as we will always need a Request
        Request requestInstance = requestType switch
        {
            _ when requestType == typeof(LaunchRequest) || parameterName.Contains("launch") => CreateLaunchRequest(),
            _ when requestType == typeof(IntentRequest) || parameterName.Contains("intent") => CreateIntentRequest(parameterName),
            _ when requestType == typeof(SessionEndedRequest) || parameterName.Contains("session") && parameterName.Contains("end") => CreateSessionEndedRequest(),
            _ when requestType == typeof(AudioPlayerRequest) || parameterName.Contains("audioplayer") || parameterName.Contains("audio") => CreateAudioPlayerRequest(),
            _ when requestType == typeof(DisplayElementSelectedRequest) || parameterName.Contains("display") => CreateDisplayElementSelectedRequest(),
            _ when requestType == typeof(PlaybackControllerRequest) || parameterName.Contains("playback") => CreatePlaybackControllerRequest(),
            _ when requestType == typeof(SystemExceptionRequest) || parameterName.Contains("system") => CreateSystemExceptionRequest(),
            _ => CreateLaunchRequest() // Default to launch request
        };
        
        if (requestType == typeof(Request)) return requestInstance;
        
        // Create real objects instead of substitutes to avoid virtual member issues
        var application = new Application { ApplicationId = "amzn1.ask.skill.test-skill-id" };
        var system = new AlexaSystem { Application = application };
        var skillContext = new Context { System = system };
        var session = new Session { Attributes = new Dictionary<string, object>() };

        var skillRequest = new SkillRequest 
        { 
            Context = skillContext,
            Request = requestInstance,
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
