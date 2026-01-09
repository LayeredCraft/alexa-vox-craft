using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;

namespace AlexaVoxCraft.Model.Apl;

[JsonConverter(typeof(APLValueConverterFactory))]
public class APLValue<T> : APLValue
{
    static APLValue()
    {
        if (typeof(T) != typeof(string) && typeof(IEnumerable).IsAssignableFrom(typeof(T)))
        {
            var elementType = typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(IList<>)
                ? typeof(T).GetGenericArguments()[0].Name
                : typeof(T).IsArray
                    ? typeof(T).GetElementType()?.Name ?? "T"
                    : "T";

            throw new InvalidOperationException(
                $"APLValue<{typeof(T).Name}> is not supported for collection types. " +
                $"Use APLValueCollection<{elementType}> instead.");
        }
    }

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
        return value is null ? null : new(value);
    }
}