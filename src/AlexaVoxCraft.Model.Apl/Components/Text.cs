using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class Text : TextBase, IJsonSerializable<Text>
{
    public Text()
    {
    }

    public Text(string text)
    {
        Content = new APLValue<string>(text);
    }

    [JsonPropertyName("type")]
    public override string Type => nameof(Text);

    [JsonPropertyName("text")] public APLValue<string>? Content { get; set; }

    [JsonPropertyName("lang")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Lang { get; set; }

    public new static void RegisterTypeInfo<T>() where T : Text
    {
        TextBase.RegisterTypeInfo<T>();
    }
}