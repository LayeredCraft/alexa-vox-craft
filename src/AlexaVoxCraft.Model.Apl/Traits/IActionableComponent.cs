namespace AlexaVoxCraft.Model.Apl.Traits;

/// <summary>
/// Defines properties for APL components that support keyboard and focus interactions.
/// </summary>
public interface IActionableComponent
{
    /// <summary>Gets or sets the commands to execute when the component loses focus.</summary>
    APLValueCollection<APLCommand>? OnBlur { get; set; }

    /// <summary>Gets or sets the commands to execute when the component receives focus.</summary>
    APLValueCollection<APLCommand>? OnFocus { get; set; }

    /// <summary>Gets or sets the keyboard handlers for key-down events.</summary>
    APLValueCollection<APLKeyboardHandler>? HandleKeyDown { get; set; }

    /// <summary>Gets or sets the keyboard handlers for key-up events.</summary>
    APLValueCollection<APLKeyboardHandler>? HandleKeyUp { get; set; }
}
