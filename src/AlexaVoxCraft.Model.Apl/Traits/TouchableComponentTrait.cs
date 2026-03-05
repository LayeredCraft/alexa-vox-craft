using System.Linq;
using System.Text.Json.Serialization.Metadata;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Traits;

/// <summary>
/// Provides the concrete property storage for <see cref="ITouchableComponent"/> via composition.
/// </summary>
public sealed class TouchableComponentTrait
{
    /// <summary>Gets or sets the gesture handlers for this component.</summary>
    public APLValueCollection<APLGesture>? Gestures { get; set; }

    /// <summary>Gets or sets the commands to execute when a gesture is cancelled.</summary>
    public APLValueCollection<APLCommand>? OnCancel { get; set; }

    /// <summary>Gets or sets the commands to execute on a pointer-down event.</summary>
    public APLValueCollection<APLCommand>? OnDown { get; set; }

    /// <summary>Gets or sets the commands to execute on a pointer-up event.</summary>
    public APLValueCollection<APLCommand>? OnUp { get; set; }

    /// <summary>Gets or sets the commands to execute on a pointer-move event.</summary>
    public APLValueCollection<APLCommand>? OnMove { get; set; }

    /// <summary>Gets or sets the commands to execute when the component is pressed.</summary>
    public APLValueCollection<APLCommand>? OnPress { get; set; }

    /// <summary>
    /// Registers the JSON metadata modifications required for touchable components.
    /// </summary>
    /// <typeparam name="T">The component type implementing <see cref="ITouchableComponent"/>.</typeparam>
    public static void RegisterTypeInfo<T>() where T : ITouchableComponent
    {
        AlexaJsonOptions.RegisterTypeModifier<T>(Apply);
    }

    /// <summary>
    /// Applies the custom converters for the touchable component properties.
    /// </summary>
    /// <param name="info">The JSON type info to modify.</param>
    public static void Apply(JsonTypeInfo info)
    {
        var gesturesProp = info.Properties.FirstOrDefault(p => p.Name == "gestures");
        gesturesProp?.CustomConverter = new APLGestureListConverter(false);

        var onCancelProp = info.Properties.FirstOrDefault(p => p.Name == "onCancel");
        onCancelProp?.CustomConverter = new APLCommandListConverter(false);

        var onDownProp = info.Properties.FirstOrDefault(p => p.Name == "onDown");
        onDownProp?.CustomConverter = new APLCommandListConverter(false);

        var onUpProp = info.Properties.FirstOrDefault(p => p.Name == "onUp");
        onUpProp?.CustomConverter = new APLCommandListConverter(false);

        var onMoveProp = info.Properties.FirstOrDefault(p => p.Name == "onMove");
        onMoveProp?.CustomConverter = new APLCommandListConverter(false);

        var onPressProp = info.Properties.FirstOrDefault(p => p.Name == "onPress");
        onPressProp?.CustomConverter = new APLCommandListConverter(false);
    }
}
