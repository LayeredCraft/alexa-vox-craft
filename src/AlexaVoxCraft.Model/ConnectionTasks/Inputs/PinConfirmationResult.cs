using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.ConnectionTasks.Inputs;

public class PinConfirmationResult
{
    [JsonPropertyName("status")]
    [JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<PinConfirmationStatus>))]
    public PinConfirmationStatus Status { get; set; }

    [JsonPropertyName("reason")]
    [JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<PinConfirmationReason>))]
    public PinConfirmationReason Reason { get; set; }
}