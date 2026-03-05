using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

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

    [JsonPropertyName("onTextLayout")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? OnTextLayout { get; set; }

    public new static void RegisterTypeInfo<T>() where T : Text
    {
        TextBase.RegisterTypeInfo<T>();
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var onTextLayoutProp = info.Properties.FirstOrDefault(p => p.Name == "onTextLayout");
            onTextLayoutProp?.CustomConverter = new APLCommandListConverter(false);
        });
    }
}