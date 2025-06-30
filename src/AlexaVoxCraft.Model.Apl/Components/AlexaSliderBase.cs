using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public abstract class AlexaSliderBase : TouchComponent, IJsonSerializable<AlexaSliderBase>
{
    [JsonPropertyName("bufferValue")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? BufferValue { get; set; }

    [JsonPropertyName("isLoading")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? IsLoading { get; set; }

    [JsonPropertyName("progressValue")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? ProgressValue { get; set; }

    [JsonPropertyName("theme")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Theme { get; set; }

    [JsonPropertyName("totalValue")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? TotalValue { get; set; }

    [JsonPropertyName("thumbDisplayedAllStates")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? ThumbDisplayedAllStates { get; set; }

    [JsonPropertyName("thumbColor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ThumbColor { get; set; }

    [JsonPropertyName("progressFillColor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? ProgressFillColor { get; set; }

    [JsonPropertyName("positionPropertyName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? PositionPropertyName { get; set; }

    [JsonPropertyName("onBlurCommand")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>>? OnBlurCommand { get; set; }

    [JsonPropertyName("onUpCommand")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>>? OnUpCommand { get; set; }

    [JsonPropertyName("onDownCommand")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>>? OnDownCommand { get; set; }

    [JsonPropertyName("onFocusCommand")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>>? OnFocusCommand { get; set; }

    [JsonPropertyName("onMoveCommand")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>>? OnMoveCommand { get; set; }

    [JsonPropertyName("handleKeyDownCommand")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>>? HandleKeyDownCommand { get; set; }

    [JsonPropertyName("metadataDisplayed")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool>? MetadataDisplayed { get; set; }

    public static void RegisterTypeInfo<T>() where T : AlexaSliderBase
    {
        TouchComponent.RegisterTypeInfo<T>();
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var onBlurCommandProp = info.Properties.FirstOrDefault(p => p.Name == "onBlurCommand");
            if (onBlurCommandProp is not null)
            {
                onBlurCommandProp.CustomConverter = new APLCommandListConverter(false);
            }

            var onUpCommandProp = info.Properties.FirstOrDefault(p => p.Name == "onUpCommand");
            if (onUpCommandProp is not null)
            {
                onUpCommandProp.CustomConverter = new APLCommandListConverter(false);
            }

            var onDownCommandProp = info.Properties.FirstOrDefault(p => p.Name == "onDownCommand");
            if (onDownCommandProp is not null)
            {
                onDownCommandProp.CustomConverter = new APLCommandListConverter(false);
            }

            var onFocusCommandProp = info.Properties.FirstOrDefault(p => p.Name == "onFocusCommand");
            if (onFocusCommandProp is not null)
            {
                onFocusCommandProp.CustomConverter = new APLCommandListConverter(false);
            }

            var onMoveCommandProp = info.Properties.FirstOrDefault(p => p.Name == "onMoveCommand");
            if (onMoveCommandProp is not null)
            {
                onMoveCommandProp.CustomConverter = new APLCommandListConverter(false);
            }

            var handleKeyDownCommandProp = info.Properties.FirstOrDefault(p => p.Name == "handleKeyDownCommand");
            if (handleKeyDownCommandProp is not null)
            {
                handleKeyDownCommandProp.CustomConverter = new APLCommandListConverter(false);
            }
        });
    }
}