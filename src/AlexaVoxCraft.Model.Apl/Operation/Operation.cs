using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;

namespace AlexaVoxCraft.Model.Apl.Operation;

[JsonConverter(typeof(OperationConverter))]
public abstract class Operation
{
    [JsonPropertyName("type")]
    public abstract string Type { get; }
    [JsonPropertyName("index")]
    public int Index { get; set; }
}