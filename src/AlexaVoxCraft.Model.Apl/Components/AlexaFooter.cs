using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaFooter : APLComponent, IJsonSerializable<AlexaFooter>
{
    public AlexaFooter()
    {
    }

    public AlexaFooter(string hintText) : this()
    {
        HintText = hintText;
    }

    [JsonPropertyName("theme")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Theme { get; set; }

    [JsonPropertyName("hintText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? HintText { get; set; }

    [JsonPropertyName("type")] public override string Type => nameof(AlexaFooter);

    [JsonPropertyName("lang")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Lang { get; set; }

    public new static void RegisterTypeInfo<T>() where T : AlexaFooter
    {
        APLComponent.RegisterTypeInfo<T>();
    }
}