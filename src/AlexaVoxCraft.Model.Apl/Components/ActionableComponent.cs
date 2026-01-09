using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public abstract class ActionableComponent : APLComponent, IJsonSerializable<ActionableComponent>
{
    [JsonPropertyName("onBlur")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>>? OnBlur { get; set; }

    [JsonPropertyName("onFocus")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>>? OnFocus { get; set; }

    [JsonPropertyName("handleKeyDown")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLKeyboardHandler>>? HandleKeyDown { get; set; }

    [JsonPropertyName("handleKeyUp")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLKeyboardHandler>>? HandleKeyUp { get; set; }

    public new static void RegisterTypeInfo<T>() where T : ActionableComponent
    {
        APLComponent.RegisterTypeInfo<T>();
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var onBlurProp = info.Properties.FirstOrDefault(p => p.Name == "onBlur");
            if (onBlurProp is not null)
            {
                onBlurProp.CustomConverter = new APLCommandListConverter(false);
            }

            var onFocusProp = info.Properties.FirstOrDefault(p => p.Name == "onFocus");
            if (onFocusProp is not null)
            {
                onFocusProp.CustomConverter = new APLCommandListConverter(false);
            }

            var handleKeyDownProp = info.Properties.FirstOrDefault(p => p.Name == "handleKeyDown");
            if (handleKeyDownProp is not null)
            {
                handleKeyDownProp.CustomConverter = new APLKeyboardHandlerConverter(false);
            }

            var handleKeyUpProp = info.Properties.FirstOrDefault(p => p.Name == "handleKeyUp");
            if (handleKeyUpProp is not null)
            {
                handleKeyUpProp.CustomConverter = new APLKeyboardHandlerConverter(false);
            }
        });
    }
}