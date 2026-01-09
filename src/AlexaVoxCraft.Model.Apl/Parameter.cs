using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Response.Converters;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl;

[JsonConverter(typeof(ParameterConverter))]
public class Parameter : IJsonSerializable<Parameter>
{
    public Parameter() { }

    public Parameter(string name)
    {
        Name = name;
    }

    [JsonIgnore] public bool WasStringInput { get; set; } = false;

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<ParameterType>))]
    public ParameterType Type { get; set; }

    public bool ShouldSerializeType()
    {
        return Type != ParameterType.any;
    }

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string Description { get; set; }

    [JsonPropertyName("default")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Default { get; set; }

    public static implicit operator Parameter(string parameterName)
    {
        return new Parameter(parameterName)
        {
            WasStringInput = true
        };
    }

    public static void RegisterTypeInfo<T>() where T : Parameter
    {
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var prop = info.Properties.FirstOrDefault(p => p.Name == "type");
            prop?.ShouldSerialize = (obj, _) =>
            {
                var parameter = (T)obj;
                return parameter.Type != ParameterType.any;
            };
        });
    }
}