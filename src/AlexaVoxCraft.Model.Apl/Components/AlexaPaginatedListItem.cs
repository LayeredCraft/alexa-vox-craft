using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public abstract class AlexaPaginatedListItem : AlexaListItem, IJsonSerializable<AlexaPaginatedListItem>
{
    [JsonPropertyName("secondaryText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> SecondaryText { get; set; } = null!;

    [JsonPropertyName("tertiaryText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> TertiaryText { get; set; } = null!;

    public new static void RegisterTypeInfo<T>() where T : AlexaPaginatedListItem
    {
        AlexaListItem.RegisterTypeInfo<T>();
    }
}