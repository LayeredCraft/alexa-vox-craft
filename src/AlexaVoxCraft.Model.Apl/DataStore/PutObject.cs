using System.Text.Json;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.DataStore;

public class PutObject : DataStoreCommand
{
    public const string CommandType = "PUT_OBJECT";

    public PutObject() : base(CommandType) { }

    [JsonPropertyName("content")]
    public JsonElement Content { get; set; }

    [JsonPropertyName("namespace")]
    public string Namespace { get; set; } = null!;

    [JsonPropertyName("key")]
    public string Key { get; set; } = null!;
}

public class PutObjectArray : DataStoreCommand
{
    public PutObjectArray() : base(PutObject.CommandType) { }

    [JsonPropertyName("content")]
    public JsonElement[] Content { get; set; } = null!;

    [JsonPropertyName("namespace")]
    public string Namespace { get; set; } = null!;

    [JsonPropertyName("key")]
    public string Key { get; set; } = null!;
}