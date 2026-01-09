using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public abstract class ActionableComponent : APLComponent, IJsonSerializable<ActionableComponent>
{
    [JsonPropertyName("onBlur")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? OnBlur { get; set; }

    [JsonPropertyName("onFocus")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? OnFocus { get; set; }

    [JsonPropertyName("handleKeyDown")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLKeyboardHandler>? HandleKeyDown { get; set; }

    [JsonPropertyName("handleKeyUp")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLKeyboardHandler>? HandleKeyUp { get; set; }

    public new static void RegisterTypeInfo<T>() where T : ActionableComponent
    {
        APLComponent.RegisterTypeInfo<T>();
    }
}