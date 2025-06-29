using System.Text.Json;

namespace AlexaVoxCraft.Model.Request.Type;

public interface IConnectionResponseHandler
{
    bool CanCreate(JsonElement element);
    System.Type Create(JsonElement element);
}