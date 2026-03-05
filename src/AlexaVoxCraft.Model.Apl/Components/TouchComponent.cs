using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.Traits;

namespace AlexaVoxCraft.Model.Apl.Components;

/// <summary>
/// Abstract base class for APL components that support touch and gesture interactions.
/// </summary>
public abstract class TouchComponent : ActionableComponent, IJsonSerializable<TouchComponent>, ITouchableComponent
{
    [JsonIgnore] internal TouchableComponentTrait TouchableComponent { get; } = new();

    /// <summary>Gets or sets the gesture handlers for this component.</summary>
    [JsonPropertyName("gestures")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLGesture>? Gestures
    {
        get => TouchableComponent.Gestures;
        set => TouchableComponent.Gestures = value;
    }

    /// <summary>Gets or sets the commands to execute when a gesture is cancelled.</summary>
    [JsonPropertyName("onCancel")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? OnCancel
    {
        get => TouchableComponent.OnCancel;
        set => TouchableComponent.OnCancel = value;
    }

    /// <summary>Gets or sets the commands to execute on a pointer-down event.</summary>
    [JsonPropertyName("onDown")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? OnDown
    {
        get => TouchableComponent.OnDown;
        set => TouchableComponent.OnDown = value;
    }

    /// <summary>Gets or sets the commands to execute on a pointer-up event.</summary>
    [JsonPropertyName("onUp")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? OnUp
    {
        get => TouchableComponent.OnUp;
        set => TouchableComponent.OnUp = value;
    }

    /// <summary>Gets or sets the commands to execute on a pointer-move event.</summary>
    [JsonPropertyName("onMove")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? OnMove
    {
        get => TouchableComponent.OnMove;
        set => TouchableComponent.OnMove = value;
    }

    /// <summary>Gets or sets the commands to execute when the component is pressed.</summary>
    [JsonPropertyName("onPress")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? OnPress
    {
        get => TouchableComponent.OnPress;
        set => TouchableComponent.OnPress = value;
    }

    /// <summary>
    /// Registers the JSON metadata modifications required for touchable components.
    /// </summary>
    /// <typeparam name="T">The component type deriving from <see cref="TouchComponent"/>.</typeparam>
    public static void RegisterTypeInfo<T>() where T : TouchComponent
    {
        ActionableComponent.RegisterTypeInfo<T>();
        TouchableComponentTrait.RegisterTypeInfo<T>();
    }
}
