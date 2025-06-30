using System.Text.Json;

namespace AlexaVoxCraft.Model.Response.Directive;

public class AskForPermissionDirectiveHandler : IConnectionSendRequestHandler
{
    public bool CanCreate(JsonElement data)
    {
        return data.TryGetProperty("name", out var nameProp) &&
               nameProp.ValueKind == JsonValueKind.String &&
               nameProp.GetString() == "AskFor";
    }

    public Type Create()
    {
        return typeof(AskForPermissionDirective);
    }
}