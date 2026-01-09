using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.Commands;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class AlexaTransportControls : APLComponent, IJsonSerializable<AlexaTransportControls>
{
    [JsonPropertyName("type")] public override string Type => nameof(AlexaTransportControls);

    [JsonPropertyName("secondaryControls")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? SecondaryControls { get; set; }

    [JsonPropertyName("primaryControlSize")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue? PrimaryControlSize { get; set; }

    [JsonPropertyName("secondaryControlSize")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDimensionValue? SecondaryControlSize { get; set; }

    [JsonPropertyName("mediaComponentId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? MediaComponentId { get; set; }

    [JsonPropertyName("autoplay")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? Autoplay { get; set; }

    [JsonPropertyName("playPauseToggleButtonId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? PlayPauseToggleButtonId { get; set; }

    [JsonPropertyName("theme")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Theme { get; set; }

    [JsonPropertyName("primaryControlPauseAction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<ControlMediaCommand>? PrimaryControlPauseAction { get; set; }

    [JsonPropertyName("primaryControlPlayAction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<ControlMediaCommand>? PrimaryControlPlayAction { get; set; }

    [JsonPropertyName("secondaryControlsAVGLeft")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? SecondaryControlsAVGLeft { get; set; }

    [JsonPropertyName("secondaryControlsAVGRight")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? SecondaryControlsAVGRight { get; set; }

    [JsonPropertyName("secondaryControlsLeftAction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? SecondaryControlsLeftAction { get; set; }

    [JsonPropertyName("secondaryControlsRightAction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? SecondaryControlsRightAction { get; set; }

    [JsonPropertyName("secondaryControlsLeftAccessibilityLabel")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? SecondaryControlsLeftAccessibilityLabel { get; set; }

    [JsonPropertyName("secondaryControlsRightAccessibilityLabel")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? SecondaryControlsRightAccessibilityLabel { get; set; }

    public new static void RegisterTypeInfo<T>() where T : AlexaTransportControls
    {
        APLComponent.RegisterTypeInfo<T>();
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var secondaryControlsAVGLeftProp =
                info.Properties.FirstOrDefault(p => p.Name == "secondaryControlsAVGLeft");
            secondaryControlsAVGLeftProp?.CustomConverter = new APLCommandListConverter(false);

            var secondaryControlsAVGRightProp =
                info.Properties.FirstOrDefault(p => p.Name == "secondaryControlsAVGRight");
            secondaryControlsAVGRightProp?.CustomConverter = new APLCommandListConverter(false);

            var secondaryControlsLeftActionProp =
                info.Properties.FirstOrDefault(p => p.Name == "secondaryControlsLeftAction");
            secondaryControlsLeftActionProp?.CustomConverter = new APLCommandListConverter(false);

            var secondaryControlsRightActionProp =
                info.Properties.FirstOrDefault(p => p.Name == "secondaryControlsRightAction");
            secondaryControlsRightActionProp?.CustomConverter = new APLCommandListConverter(false);
        });
    }
}