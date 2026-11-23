using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.WebApiGames.Tests;

internal sealed class GameData
{
    public int Level { get; set; }
}

// A caller-owned source-generated context used to prove the typed payload helpers take caller-supplied
// JsonTypeInfo<T> and need no reflection.
[JsonSerializable(typeof(GameData))]
internal partial class WebApiGamesTestJsonContext : JsonSerializerContext
{
}
