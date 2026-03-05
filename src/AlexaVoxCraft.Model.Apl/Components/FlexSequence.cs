using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Apl.Traits;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

/// <summary>
/// An APL component that arranges children in a scrollable flex layout.
/// </summary>
public class FlexSequence : ActionableComponent, IJsonSerializable<FlexSequence>, IMultiChildComponent
{
    [JsonIgnore]
    internal MultiChildComponentTrait MultiChild { get; } = new();

    /// <summary>Initializes a new instance of <see cref="FlexSequence"/>.</summary>
    public FlexSequence()
    {
    }

    /// <summary>Initializes a new instance of <see cref="FlexSequence"/> with the specified child components.</summary>
    /// <param name="items">The child components to render.</param>
    public FlexSequence(params APLComponent[] items) : this((IEnumerable<APLComponent>)items)
    {
    }

    /// <summary>Initializes a new instance of <see cref="FlexSequence"/> with the specified child components.</summary>
    /// <param name="items">The child components to render.</param>
    public FlexSequence(IEnumerable<APLComponent> items)
    {
        Items = new APLValueCollection<APLComponent>(items);
    }

    /// <inheritdoc/>
    [JsonPropertyName("type")]
    public override string? Type => nameof(FlexSequence);

    /// <summary>Gets or sets the cross-axis alignment of children within the flex container.</summary>
    [JsonPropertyName("alignItems")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? AlignItems { get; set; }

    /// <summary>Gets or sets whether children are automatically numbered.</summary>
    [JsonPropertyName("numbered")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? Numbered { get; set; }

    /// <summary>Gets or sets the commands to execute when the component is scrolled.</summary>
    [JsonPropertyName("onScroll")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand> OnScroll { get; set; }

    /// <summary>Gets or sets the direction in which the component scrolls.</summary>
    [JsonPropertyName("scrollDirection")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> ScrollDirection { get; set; }

    /// <summary>Gets or sets the snap behavior when scrolling stops.</summary>
    [JsonPropertyName("snap")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<Snap?> Snap { get; set; }

    // IMultiChildComponent
    /// <summary>Gets or sets the data collection used to populate the component's children.</summary>
    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<object>? Data
    {
        get => MultiChild.Data;
        set => MultiChild.Data = value;
    }

    /// <summary>Gets or sets the components to render before the first data-bound child.</summary>
    [JsonPropertyName("firstItem")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLComponent>? FirstItem
    {
        get => MultiChild.FirstItem;
        set => MultiChild.FirstItem = value;
    }

    /// <summary>Gets or sets the child components rendered within this component.</summary>
    [JsonPropertyName("items")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLComponent>? Items
    {
        get => MultiChild.Items;
        set => MultiChild.Items = value;
    }

    /// <summary>Gets or sets the components to render after the last data-bound child.</summary>
    [JsonPropertyName("lastItem")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLComponent>? LastItem
    {
        get => MultiChild.LastItem;
        set => MultiChild.LastItem = value;
    }

    /// <summary>Gets or sets the commands to execute when the children collection changes.</summary>
    [JsonPropertyName("onChildrenChanged")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? OnChildrenChanged
    {
        get => MultiChild.OnChildrenChanged;
        set => MultiChild.OnChildrenChanged = value;
    }

    /// <summary>
    /// Registers the JSON metadata modifications required for <see cref="FlexSequence"/> components.
    /// </summary>
    /// <typeparam name="T">The component type deriving from <see cref="FlexSequence"/>.</typeparam>
    public static void RegisterTypeInfo<T>() where T : FlexSequence
    {
        ActionableComponent.RegisterTypeInfo<T>();
        MultiChildComponentTrait.RegisterTypeInfo<T>();
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var onScrollProp = info.Properties.FirstOrDefault(p => p.Name == "onScroll");
            onScrollProp?.CustomConverter = new APLCommandListConverter(false);
        });
    }
}
