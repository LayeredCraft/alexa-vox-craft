using System;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class TextTrack
{
    [JsonPropertyName("type")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Type { get; } = "caption";

    [JsonPropertyName("url")] public APLValue<Uri> Uri { get; set; }

    [JsonPropertyName("description")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Description { get; set; }
}