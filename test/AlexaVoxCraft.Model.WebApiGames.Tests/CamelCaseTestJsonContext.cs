using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.WebApiGames.Tests;

internal sealed class CamelCaseGameData
{
    public int Score { get; set; }
}

// A second caller-owned context with a distinct naming policy, used to prove the typed payload helpers
// honor the caller-supplied JsonTypeInfo<T> (not any ambient/reflection-derived metadata).
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(CamelCaseGameData))]
internal partial class CamelCaseTestJsonContext : JsonSerializerContext
{
}
