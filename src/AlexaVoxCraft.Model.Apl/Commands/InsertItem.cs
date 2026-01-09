using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class InsertItem : APLCommand
{
    [JsonPropertyName("type")] public override string Type => nameof(InsertItem);

    [JsonPropertyName("at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? At { get; set; }

    [JsonPropertyName("componentId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ComponentId { get; set; }

    [JsonPropertyName("items")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<object>? Items { get; set; }

}