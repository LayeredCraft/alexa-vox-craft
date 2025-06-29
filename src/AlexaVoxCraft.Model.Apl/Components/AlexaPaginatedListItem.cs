using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public abstract class AlexaPaginatedListItem : AlexaListItem, IJsonSerializable<AlexaPaginatedListItem>
{
    [JsonPropertyName("secondaryText")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> SecondaryText { get; set; }

    [JsonPropertyName("tertiaryText")][JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> TertiaryText { get; set; }

    public static void RegisterTypeInfo<T>() where T : AlexaPaginatedListItem
    {
        AlexaListItem.RegisterTypeInfo<T>();
    }
}