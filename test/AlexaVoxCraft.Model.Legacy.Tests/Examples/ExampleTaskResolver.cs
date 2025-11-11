using System.Text.Json;
using AlexaVoxCraft.Model.Request.Type;

namespace AlexaVoxCraft.Model.Tests.Examples;

public class ExampleTaskResolver : IConnectionTaskResolver
{
    public bool CanResolve(JsonElement element)
    {
        return element.TryGetProperty("randomParameter", out _);
    }

    public Type? Resolve(JsonElement element)
    {
        return  typeof(ExampleTask);
    }
}