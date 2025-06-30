using System.Text.Json;

namespace AlexaVoxCraft.Model.Request.Type;

public interface IConnectionTaskResolver
{
    bool CanResolve(JsonElement element);
    System.Type? Resolve(JsonElement element);
}