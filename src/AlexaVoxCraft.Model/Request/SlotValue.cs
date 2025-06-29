using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Request;

public class SlotValue
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<SlotValueType>))]
    public SlotValueType SlotType { get; set; }

    [JsonPropertyName("value"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Value { get; set; }

    [JsonPropertyName("values"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SlotValue[]? Values { get; set; }

    [JsonPropertyName("resolutions"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Resolution? Resolutions { get; set; }
}