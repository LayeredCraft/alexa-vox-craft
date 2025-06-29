namespace AlexaVoxCraft.Model.Request.Type.Converters;

public class DefaultRequestTypeResolver : IRequestTypeResolver
{
    public bool CanResolve(string requestType)
    {
        return requestType is "IntentRequest" or "LaunchRequest" or "SessionEndedRequest" or "System.ExceptionEncountered";
    }

    public System.Type? Resolve(string requestType)
    {
        return requestType switch
        {
            "IntentRequest" => typeof(IntentRequest),
            "LaunchRequest" => typeof(LaunchRequest),
            "SessionEndedRequest" => typeof(SessionEndedRequest),
            "System.ExceptionEncountered" => typeof(SystemExceptionRequest),
            _ => null
        };
    }
}