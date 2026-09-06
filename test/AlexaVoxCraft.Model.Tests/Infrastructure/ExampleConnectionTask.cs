using System.Text.Json;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.ConnectionTasks;
using AlexaVoxCraft.Model.Request.Type;

namespace AlexaVoxCraft.Model.Tests.Infrastructure;

/// <summary>
/// A custom, non-built-in <see cref="IConnectionTask"/> used to exercise the
/// <see cref="ConnectionTaskConverter.AddToConnectionTaskResolvers"/> extensibility hook.
/// </summary>
public class ExampleConnectionTask : IConnectionTask
{
    public string ConnectionUri { get; set; } = null!;

    [JsonPropertyName("randomParameter")]
    public string RandomParameter { get; set; } = null!;
}

public class ExampleConnectionTaskResolver : IConnectionTaskResolver
{
    public bool CanResolve(JsonElement element) => element.TryGetProperty("randomParameter", out _);

    public Type? Resolve(JsonElement element) => typeof(ExampleConnectionTask);
}
