using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.Components;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class PlayMedia : APLCommand, IJsonSerializable<PlayMedia>
{
    [JsonPropertyName("type")]
    public override string Type => "PlayMedia";

    [JsonPropertyName("audioTrack")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? AudioTrack { get; set; } = "foreground";

    [JsonPropertyName("componentId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ComponentId { get; set; }

    [JsonPropertyName("source")]
    public APLValueCollection<VideoSource> Value { get; set; }

    public static void RegisterTypeInfo<T>() where T : PlayMedia
    {
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var prop = info.Properties.FirstOrDefault(p => p.Name == "source");
            prop?.CustomConverter = new APLValueCollectionConverter<VideoSource>(false);
        });
    }
}