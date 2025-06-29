using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;

namespace AlexaVoxCraft.Model.Apl;

[JsonConverter(typeof(APLValueConverterFactory))]
public class APLValue<T> : APLValue
{
    public APLValue() { }

    public APLValue(T value)
    {
        Value = value;
    }

    public T Value { get; set; }

    public override object GetValue()
    {
        return Value;
    }

    public static implicit operator T(APLValue<T> value)
    {
        return value.Value;
    }

    public static implicit operator APLValue<T>?(T value)
    {
        return value == null ? null : new APLValue<T>(value);
    }
}