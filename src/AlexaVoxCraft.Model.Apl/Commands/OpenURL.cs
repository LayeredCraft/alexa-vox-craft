using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class OpenURL : APLCommand, IJsonSerializable<OpenURL>
{
    [JsonPropertyName("type")] public override string Type => nameof(OpenURL);

    [JsonPropertyName("source")] public APLValue<string> Source { get; set; }

    [JsonPropertyName("onFail")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>>? OnFail { get; set; }

    public static void RegisterTypeInfo<T>() where T : OpenURL
    {
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var onFailProp = info.Properties.FirstOrDefault(p => p.Name == "onFail");
            if (onFailProp is not null)
            {
                onFailProp.CustomConverter = new APLCommandListConverter(false);
            }
        });
    }
}