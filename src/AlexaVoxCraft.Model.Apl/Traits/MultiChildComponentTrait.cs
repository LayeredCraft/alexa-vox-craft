using System.Linq;
using System.Text.Json.Serialization.Metadata;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Traits;

/// <summary>
/// Provides the concrete property storage for <see cref="IMultiChildComponent"/> via composition.
/// </summary>
public sealed class MultiChildComponentTrait
{
    /// <summary>Gets or sets the data collection used to populate the component's children.</summary>
    public APLValueCollection<object>? Data { get; set; }

    /// <summary>Gets or sets the components to render before the first data-bound child.</summary>
    public APLValueCollection<APLComponent>? FirstItem { get; set; }

    /// <summary>Gets or sets the child components rendered within this component.</summary>
    public APLValueCollection<APLComponent>? Items { get; set; }

    /// <summary>Gets or sets the components to render after the last data-bound child.</summary>
    public APLValueCollection<APLComponent>? LastItem { get; set; }

    /// <summary>Gets or sets the commands to execute when the children collection changes.</summary>
    public APLValueCollection<APLCommand>? OnChildrenChanged { get; set; }

    /// <summary>
    /// Registers the JSON metadata modifications required for multi-child components.
    /// </summary>
    /// <typeparam name="T">The component type implementing <see cref="IMultiChildComponent"/>.</typeparam>
    public static void RegisterTypeInfo<T>() where T : IMultiChildComponent
    {
        AlexaJsonOptions.RegisterTypeModifier<T>(Apply);
    }

    /// <summary>
    /// Applies the custom converters for the multi-child component properties.
    /// </summary>
    /// <param name="info">The JSON type info to modify.</param>
    public static void Apply(JsonTypeInfo info)
    {
        var dataProp = info.Properties.FirstOrDefault(p => p.Name == "data");
        dataProp?.CustomConverter = new APLValueCollectionConverter<object>(false);

        var itemsProp = info.Properties.FirstOrDefault(p => p.Name == "items");
        itemsProp?.CustomConverter = new APLValueCollectionConverter<APLComponent>(false);

        var firstItemProp = info.Properties.FirstOrDefault(p => p.Name == "firstItem");
        firstItemProp?.CustomConverter = new APLValueCollectionConverter<APLComponent>(false);

        var lastItemProp = info.Properties.FirstOrDefault(p => p.Name == "lastItem");
        lastItemProp?.CustomConverter = new APLValueCollectionConverter<APLComponent>(false);

        var onChildrenChangedProp = info.Properties.FirstOrDefault(p => p.Name == "onChildrenChanged");
        onChildrenChangedProp?.CustomConverter = new APLCommandListConverter(false);
    }
}
