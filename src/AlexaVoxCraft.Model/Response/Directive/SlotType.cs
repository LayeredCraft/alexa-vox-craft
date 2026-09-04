using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Response.Directive;

public class SlotType
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("values")]
    public SlotTypeValue[] Values { get; set; } = null!;
}