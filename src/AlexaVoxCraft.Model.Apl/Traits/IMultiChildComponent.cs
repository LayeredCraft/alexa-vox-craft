namespace AlexaVoxCraft.Model.Apl.Traits;

/// <summary>
/// Defines properties for APL components that support multiple child items and data binding.
/// </summary>
public interface IMultiChildComponent
{
    /// <summary>Gets or sets the data collection used to populate the component's children.</summary>
    APLValueCollection<object>? Data { get; set; }

    /// <summary>Gets or sets the components to render before the first data-bound child.</summary>
    APLValueCollection<APLComponent>? FirstItem { get; set; }

    /// <summary>Gets or sets the child components rendered within this component.</summary>
    APLValueCollection<APLComponent>? Items { get; set; }

    /// <summary>Gets or sets the components to render after the last data-bound child.</summary>
    APLValueCollection<APLComponent>? LastItem { get; set; }

    /// <summary>Gets or sets the commands to execute when the children collection changes.</summary>
    APLValueCollection<APLCommand>? OnChildrenChanged { get; set; }
}