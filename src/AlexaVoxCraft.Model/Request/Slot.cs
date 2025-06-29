using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request;

public class Slot
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("value"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Value { get; set; }

    [JsonPropertyName("confirmationStatus"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ConfirmationStatus { get; set; }

    [JsonPropertyName("source"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Source { get; set; }

    [JsonPropertyName("resolutions"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Resolution? Resolution { get; set; }

    [JsonPropertyName("slotValue"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SlotValue? SlotValue { get; set; }
}