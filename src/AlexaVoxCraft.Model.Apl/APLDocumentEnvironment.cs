using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.Components;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class APLDocumentEnvironment
{
    [JsonPropertyName("lang")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Lang { get; set; }

    [JsonPropertyName("layoutDirection")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<LayoutDirection?>? LayoutDirection { get; set; }

    [JsonPropertyName("parameters")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<Parameter>? Parameters { get; set; }
    public static void AddSupport()
    {
        AlexaJsonOptions.RegisterTypeModifier<APLDocumentEnvironment>(info =>
        {
            var parametersProp = info.Properties.FirstOrDefault(p => p.Name == "parameters");
            if (parametersProp is not null)
            {
                parametersProp.CustomConverter = new ParameterListConverter(true);
            }
        });
    }
}