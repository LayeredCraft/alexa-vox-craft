using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Response.Directive;

public class SlotTypeValueName
{
    [JsonPropertyName("value")]
    public string Value { get; set; }

    [JsonPropertyName("synonyms")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[] Synonyms { get; set; }
}