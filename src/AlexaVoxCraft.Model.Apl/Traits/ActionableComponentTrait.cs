using System.Linq;
using System.Text.Json.Serialization.Metadata;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Traits;

/// <summary>
/// Provides the concrete property storage for <see cref="IActionableComponent"/> via composition.
/// </summary>
public sealed class ActionableComponentTrait
{
    /// <summary>Gets or sets the commands to execute when the component loses focus.</summary>
    public APLValueCollection<APLCommand>? OnBlur { get; set; }

    /// <summary>Gets or sets the commands to execute when the component receives focus.</summary>
    public APLValueCollection<APLCommand>? OnFocus { get; set; }

    /// <summary>Gets or sets the keyboard handlers for key-down events.</summary>
    public APLValueCollection<APLKeyboardHandler>? HandleKeyDown { get; set; }

    /// <summary>Gets or sets the keyboard handlers for key-up events.</summary>
    public APLValueCollection<APLKeyboardHandler>? HandleKeyUp { get; set; }

    /// <summary>
    /// Registers the JSON metadata modifications required for actionable components.
    /// </summary>
    /// <typeparam name="T">The component type implementing <see cref="IActionableComponent"/>.</typeparam>
    public static void RegisterTypeInfo<T>() where T : IActionableComponent
    {
        AlexaJsonOptions.RegisterTypeModifier<T>(Apply);
    }

    /// <summary>
    /// Applies the custom converters for the actionable component properties.
    /// </summary>
    /// <param name="info">The JSON type info to modify.</param>
    public static void Apply(JsonTypeInfo info)
    {
        var onBlurProp = info.Properties.FirstOrDefault(p => p.Name == "onBlur");
        onBlurProp?.CustomConverter = new APLCommandListConverter(false);

        var onFocusProp = info.Properties.FirstOrDefault(p => p.Name == "onFocus");
        onFocusProp?.CustomConverter = new APLCommandListConverter(false);

        var handleKeyDownProp = info.Properties.FirstOrDefault(p => p.Name == "handleKeyDown");
        handleKeyDownProp?.CustomConverter = new APLKeyboardHandlerConverter(false);

        var handleKeyUpProp = info.Properties.FirstOrDefault(p => p.Name == "handleKeyUp");
        handleKeyUpProp?.CustomConverter = new APLKeyboardHandlerConverter(false);
    }
}
