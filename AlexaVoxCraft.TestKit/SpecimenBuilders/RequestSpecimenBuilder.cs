using System.Reflection;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;
using AutoFixture.Kernel;

namespace AlexaVoxCraft.TestKit.SpecimenBuilders;

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