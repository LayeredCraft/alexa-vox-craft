using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaPageCounter : APLComponent, IJsonSerializable<AlexaPageCounter>
{
    [JsonPropertyName("type")] public override string Type => nameof(AlexaPageCounter);

    [JsonPropertyName("currentPageComponentId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? CurrentPageComponentId { get; set; }

    [JsonPropertyName("currentPage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? CurrentPage { get; set; }

    [JsonPropertyName("theme")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Theme { get; set; }

    [JsonPropertyName("totalPages")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? TotalPages { get; set; }

    public new static void RegisterTypeInfo<T>() where T : AlexaPageCounter
    {
        APLComponent.RegisterTypeInfo<T>();
    }
}