using System.Text.Json;

namespace AlexaVoxCraft.Model.Request.Type;

//https://developer.amazon.com/en-US/docs/alexa/smapi/voice-permissions-for-reminders.html#send-a-connectionssendrequest-directive
public class AskForRequestHandler : IConnectionResponseHandler
{
    public bool CanCreate(JsonElement element)
    {
        return element.TryGetProperty("name", out var nameProp) &&
               nameProp.ValueKind == JsonValueKind.String &&
               nameProp.GetString() == "AskFor";
    }

    public System.Type Create(JsonElement element)
    {
        return typeof(AskForPermissionRequest);
    }

}