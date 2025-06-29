using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class StyleValue
{
    public StyleValue() { }

    public StyleValue(Dictionary<string, object> properties)
    {
        Properties = properties;
    }


    [JsonPropertyName("when")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string When { get; set; }

    [JsonExtensionData]
    public Dictionary<string,object> Properties { get; set; }
}