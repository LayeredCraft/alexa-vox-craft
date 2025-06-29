using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;

namespace AlexaVoxCraft.Model.Apl;

public class VisibleComponentTags
{
    [JsonPropertyName("focused")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Focused { get; set; }

    [JsonPropertyName("clickable")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Clickable { get; set; }

    [JsonPropertyName("checked")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Checked { get; set; }

    [JsonPropertyName("disabled")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Disabled { get; set; }

    [JsonPropertyName("spoken")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Spoken { get; set; }

    [JsonPropertyName("ordinal")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Ordinal { get; set; }

    [JsonPropertyName("list")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public VisibleComponentListTag? List { get; set; }

    [JsonPropertyName("list_item")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public VisibleComponentListItemTag? ListItem { get; set; }

    [JsonPropertyName("media")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public VisibleComponentMediaTag? Media { get; set; }

    [JsonPropertyName("pager")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public VisibleComponentPagerTag? Pager { get; set; }

    [JsonPropertyName("scrollable")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public VisibleComponentScrollableTag? Scrollable { get; set; }

    [JsonPropertyName("viewport")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(NullIfEmptyObjectConverter<APLViewport>))]
    public APLViewport? Viewport { get; set; }
}