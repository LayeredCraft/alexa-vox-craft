using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Response.Converters;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class Container : APLComponent, IJsonSerializable<Container>
{
    public Container()
    {
    }

    public Container(params APLComponent[] items) : this((IEnumerable<APLComponent>)items)
    {
    }

    public Container(IEnumerable<APLComponent> items)
    {
        Items = new APLCollection<APLComponent>(items);
    }

    [JsonPropertyName("type")] public override string Type => nameof(Container);

    [JsonPropertyName("alignItems")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? AlignItems { get; set; }

    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<object>> Data { get; set; }

    [JsonPropertyName("direction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Direction { get; set; }

    [JsonPropertyName("firstItem")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLCollection<APLComponent>? FirstItem { get; set; }

    [JsonPropertyName("lastItem")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLCollection<APLComponent> LastItem { get; set; }

    [JsonPropertyName("items")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLCollection<APLComponent>? Items { get; set; }

    [JsonPropertyName("justifyContent")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? JustifyContent { get; set; }

    [JsonPropertyName("numbered")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? Numbered { get; set; }

    [JsonPropertyName("wrap")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<ContainerWrap?> Wrap { get; set; }

    public new static void RegisterTypeInfo<T>() where T : Container
    {
        APLComponent.RegisterTypeInfo<T>();
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var dataProp = info.Properties.FirstOrDefault(p => p.Name == "data");
            dataProp?.CustomConverter = new GenericSingleOrListConverter<object>(false);
        });
    }
}

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<ContainerWrap>))]
public enum ContainerWrap
{
    [EnumMember(Value = "wrapReverse")] WrapReverse,
    [EnumMember(Value = "noWrap")] NoWrap,
    [EnumMember(Value = "wrap")] Wrap
}