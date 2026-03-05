namespace AlexaVoxCraft.Model.Apl.Traits;

/// <summary>
/// Defines properties for APL components that support touch and gesture interactions.
/// </summary>
public interface ITouchableComponent
{
    /// <summary>Gets or sets the gesture handlers for this component.</summary>
    APLValueCollection<APLGesture>? Gestures { get; set; }

    /// <summary>Gets or sets the commands to execute when a gesture is cancelled.</summary>
    APLValueCollection<APLCommand>? OnCancel { get; set; }

    /// <summary>Gets or sets the commands to execute on a pointer-down event.</summary>
    APLValueCollection<APLCommand>? OnDown { get; set; }

    /// <summary>Gets or sets the commands to execute on a pointer-up event.</summary>
    APLValueCollection<APLCommand>? OnUp { get; set; }

    /// <summary>Gets or sets the commands to execute on a pointer-move event.</summary>
    APLValueCollection<APLCommand>? OnMove { get; set; }

    /// <summary>Gets or sets the commands to execute when the component is pressed.</summary>
    APLValueCollection<APLCommand>? OnPress { get; set; }
}
