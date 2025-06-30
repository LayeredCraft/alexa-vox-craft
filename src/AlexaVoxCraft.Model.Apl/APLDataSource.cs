using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;

namespace AlexaVoxCraft.Model.Apl;

[JsonConverter(typeof(APLDataSourceConverter))]
public abstract class APLDataSource
{
    [JsonPropertyName("type")] public abstract string Type { get; }
}