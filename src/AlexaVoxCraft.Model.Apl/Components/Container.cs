using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.Traits;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.Components;

public class Container : APLComponent, IJsonSerializable<Container>, IMultiChildComponent
{
    [JsonIgnore]
    internal MultiChildComponentTrait MultiChild { get; } = new();

    public Container()
    {
    }

    public Container(params APLComponent[] items) : this((IEnumerable<APLComponent>)items)
    {
    }

    public Container(IEnumerable<APLComponent> items)
    {
        Items = new APLValueCollection<APLComponent>(items);
    }

    [JsonPropertyName("type")] public override string Type => nameof(Container);

    [JsonPropertyName("alignItems")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? AlignItems { get; set; }

    [JsonPropertyName("direction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Direction { get; set; }

    [JsonPropertyName("justifyContent")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? JustifyContent { get; set; }

    [JsonPropertyName("numbered")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? Numbered { get; set; }

    [JsonPropertyName("wrap")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<ContainerWrap?> Wrap { get; set; }

    // IMultiChildComponent
    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<object>? Data
    {
        get => MultiChild.Data;
        set => MultiChild.Data = value;
    }

    [JsonPropertyName("firstItem")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLComponent>? FirstItem
    {
        get => MultiChild.FirstItem;
        set => MultiChild.FirstItem = value;
    }

    [JsonPropertyName("items")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLComponent>? Items
    {
        get => MultiChild.Items;
        set => MultiChild.Items = value;
    }

    [JsonPropertyName("lastItem")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLComponent>? LastItem
    {
        get => MultiChild.LastItem;
        set => MultiChild.LastItem = value;
    }

    [JsonPropertyName("onChildrenChanged")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? OnChildrenChanged
    {
        get => MultiChild.OnChildrenChanged;
        set => MultiChild.OnChildrenChanged = value;
    }

    public new static void RegisterTypeInfo<T>() where T : Container
    {
        APLComponent.RegisterTypeInfo<T>();
        MultiChildComponentTrait.RegisterTypeInfo<T>();
    }
}

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<ContainerWrap>))]
public enum ContainerWrap
{
    [EnumMember(Value = "wrapReverse")] WrapReverse,
    [EnumMember(Value = "noWrap")] NoWrap,
    [EnumMember(Value = "wrap")] Wrap
}