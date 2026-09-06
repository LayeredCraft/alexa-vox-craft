using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class TimeText : TextBase, IJsonSerializable<TimeText>
{
    [JsonPropertyName("type")]
    public override string Type => nameof(TimeText);

    [JsonPropertyName("direction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<TimeTextDirection?> Direction { get; set; } = null!;

    [JsonPropertyName("format")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> Format { get; set; } = null!;

    [JsonPropertyName("start")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public new APLValue<int?> Start { get; set; } = null!;

    public new static void RegisterTypeInfo<T>() where T : TimeText
    {
        TextBase.RegisterTypeInfo<T>();
    }
}