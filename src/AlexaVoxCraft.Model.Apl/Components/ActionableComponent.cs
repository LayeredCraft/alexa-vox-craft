using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.Traits;

namespace AlexaVoxCraft.Model.Apl.Components;

/// <summary>
/// Abstract base class for APL components that support keyboard and focus interactions.
/// </summary>
public abstract class ActionableComponent : APLComponent, IJsonSerializable<ActionableComponent>, IActionableComponent
{
    [JsonIgnore]
    internal ActionableComponentTrait IntActionableComponent { get; } = new();

    /// <summary>Gets or sets the commands to execute when the component loses focus.</summary>
    [JsonPropertyName("onBlur")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? OnBlur
    {
        get => IntActionableComponent.OnBlur;
        set => IntActionableComponent.OnBlur = value;
    }

    /// <summary>Gets or sets the commands to execute when the component receives focus.</summary>
    [JsonPropertyName("onFocus")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? OnFocus
    {
        get => IntActionableComponent.OnFocus;
        set => IntActionableComponent.OnFocus = value;
    }

    /// <summary>Gets or sets the keyboard handlers for key-down events.</summary>
    [JsonPropertyName("handleKeyDown")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLKeyboardHandler>? HandleKeyDown
    {
        get => IntActionableComponent.HandleKeyDown;
        set => IntActionableComponent.HandleKeyDown = value;
    }

    /// <summary>Gets or sets the keyboard handlers for key-up events.</summary>
    [JsonPropertyName("handleKeyUp")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLKeyboardHandler>? HandleKeyUp
    {
        get => IntActionableComponent.HandleKeyUp;
        set => IntActionableComponent.HandleKeyUp = value;
    }

    /// <summary>
    /// Registers the JSON metadata modifications required for actionable components.
    /// </summary>
    /// <typeparam name="T">The component type deriving from <see cref="ActionableComponent"/>.</typeparam>
    public new static void RegisterTypeInfo<T>() where T : ActionableComponent
    {
        APLComponent.RegisterTypeInfo<T>();
        ActionableComponentTrait.RegisterTypeInfo<T>();
    }
}
