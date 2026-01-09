using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class Sequence : ActionableComponent, IJsonSerializable<Sequence>
{
    public Sequence()
    {
    }

    public Sequence(params APLComponent[] items) : this((IEnumerable<APLComponent>)items)
    {
    }

    public Sequence(IEnumerable<APLComponent> items)
    {
        Items = [..items];
    }

    [JsonPropertyName("type")] public override string Type => nameof(Sequence);

    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<List<object>> Data { get; set; }

    [JsonPropertyName("scrollDirection")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> ScrollDirection { get; set; }

    [JsonPropertyName("firstItem")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<List<APLComponent>> FirstItem { get; set; }

    [JsonPropertyName("lastItem")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<List<APLComponent>> LastItem { get; set; }

    [JsonPropertyName("items")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLComponent> Items { get; set; }

    [JsonPropertyName("numbered")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?> Numbered { get; set; }

    [JsonPropertyName("onScroll")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand> OnScroll { get; set; }

    [JsonPropertyName("scaling")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?> Scaling { get; set; }

    [JsonPropertyName("snap")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<Snap?> Snap { get; set; }

    public new static void RegisterTypeInfo<T>() where T : Sequence
    {
        ActionableComponent.RegisterTypeInfo<T>();
    }
}