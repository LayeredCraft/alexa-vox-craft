using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaHeader : APLComponent, IJsonSerializable<AlexaHeader>
{
    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public override string Type => nameof(AlexaHeader);

    [JsonPropertyName("headerTitle")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> HeaderTitle { get; set; } = null!;

    [JsonPropertyName("headerSubtitle")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> HeaderSubtitle { get; set; } = null!;

    [JsonPropertyName("headerAttributionText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> HeaderAttributionText { get; set; } = null!;

    [JsonPropertyName("headerAttributionOpacity")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<double?> HeaderAttributionOpacity { get; set; } = null!;

    [JsonPropertyName("headerAttributionImage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> HeaderAttributionImage { get; set; } = null!;

    [JsonPropertyName("headerAttributionPrimacy")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?> HeaderAttributionPrimacy { get; set; } = null!;

    [JsonPropertyName("headerBackButton")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?> HeaderBackButton { get; set; } = null!;

    [JsonPropertyName("headerBackButtonAccessibilityLabel")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> HeaderBackButtonAccessibilityLabel { get; set; } = null!;

    [JsonPropertyName("headerBackButtonCommand")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand> HeaderBackButtonCommand { get; set; } = null!;

    [JsonPropertyName("headerBackgroundColor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> HeaderBackgroundColor { get; set; } = null!;

    [JsonPropertyName("headerDivider")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?> HeaderDivider { get; set; } = null!;

    [JsonPropertyName("theme")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> Theme { get; set; } = null!;

    public new static void RegisterTypeInfo<T>() where T : AlexaHeader
    {
        APLComponent.RegisterTypeInfo<T>();
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var prop = info.Properties.FirstOrDefault(p => p.Name == "headerBackButtonCommand");
            prop?.CustomConverter = new APLCommandListConverter(false);
        });
    }
}