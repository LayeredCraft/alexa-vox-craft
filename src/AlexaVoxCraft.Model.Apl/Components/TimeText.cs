using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class TimeText : TextBase, IJsonSerializable<TimeText>
{
    [JsonPropertyName("type")]
    public override string Type => nameof(TimeText);

    [JsonPropertyName("direction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<TimeTextDirection?> Direction { get; set; }

    [JsonPropertyName("format")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> Format { get; set; }

    [JsonPropertyName("start")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public new APLValue<int?> Start { get; set; }

    public new static void RegisterTypeInfo<T>() where T : TimeText
    {
        TextBase.RegisterTypeInfo<T>();
    }
}