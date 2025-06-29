using System.Text.Json;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.ConnectionTasks.Inputs;

public class PinConfirmationResolver : IConnectionTaskResolver
{
    public bool CanResolve(JsonElement element)
    {
        return element.TryGetProperty("uri", out var uriProp) &&
               uriProp.GetString() == PinConfirmation.AssociatedUri;
    }

    public Type? Resolve(JsonElement element)
    {
        return typeof(PinConfirmation);
    }

    public static PinConfirmationResult? ResultFromSessionResumed(SessionResumedRequest request)
    {
        if (request.Cause.Result is Dictionary<string, object> dict)
        {
            var json = JsonSerializer.Serialize(dict, AlexaJsonOptions.DefaultOptions);
            return JsonSerializer.Deserialize<PinConfirmationResult>(json, AlexaJsonOptions.DefaultOptions);
        }
        return null;
    }
}