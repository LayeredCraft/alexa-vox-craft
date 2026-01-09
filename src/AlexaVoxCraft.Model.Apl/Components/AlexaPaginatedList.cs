using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaPaginatedList : ResponsiveTemplate, IJsonSerializable<AlexaPaginatedList>
{
    [JsonPropertyName("type")]
    public override string Type => nameof(AlexaPaginatedList);

    [JsonPropertyName("primaryAction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? PrimaryAction { get; set; }

    [JsonPropertyName("listItems")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<AlexaPaginatedListItem>? ListItems { get; set; }

    [JsonPropertyName("headerAttributionOpacity")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<double?>? HeaderAttributionOpacity { get; set; }

    [JsonPropertyName("listId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ListId { get; set; }

    [JsonPropertyName("speechItems")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? SpeechItems { get; set; }

    public new static void RegisterTypeInfo<T>() where T : AlexaPaginatedList
    {
        ResponsiveTemplate.RegisterTypeInfo<T>();
    }
}