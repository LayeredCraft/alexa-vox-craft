using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaHeader : APLComponent, IJsonSerializable<AlexaHeader>
{
    public AlexaHeader()
    {
    }

    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public override string Type => nameof(AlexaHeader);

    [JsonPropertyName("headerTitle")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> HeaderTitle { get; set; }

    [JsonPropertyName("headerSubtitle")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> HeaderSubtitle { get; set; }

    [JsonPropertyName("headerAttributionText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> HeaderAttributionText { get; set; }

    [JsonPropertyName("headerAttributionOpacity")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<double?> HeaderAttributionOpacity { get; set; }

    [JsonPropertyName("headerAttributionImage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> HeaderAttributionImage { get; set; }

    [JsonPropertyName("headerAttributionPrimacy")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?> HeaderAttributionPrimacy { get; set; }

    [JsonPropertyName("headerBackButton")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?> HeaderBackButton { get; set; }

    [JsonPropertyName("headerBackButtonAccessibilityLabel")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> HeaderBackButtonAccessibilityLabel { get; set; }

    [JsonPropertyName("headerBackButtonCommand")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand> HeaderBackButtonCommand { get; set; }

    [JsonPropertyName("headerBackgroundColor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> HeaderBackgroundColor { get; set; }

    [JsonPropertyName("headerDivider")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?> HeaderDivider { get; set; }

    [JsonPropertyName("theme")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> Theme { get; set; }

    public new static void RegisterTypeInfo<T>() where T : AlexaHeader
    {
        APLComponent.RegisterTypeInfo<T>();
    }
}