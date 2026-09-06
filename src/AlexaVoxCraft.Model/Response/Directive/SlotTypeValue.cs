
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Response.Directive;

public class SlotTypeValue
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("name")]
    public SlotTypeValueName Name { get; set; } = null!;
}