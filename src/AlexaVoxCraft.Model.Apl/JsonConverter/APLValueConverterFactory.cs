using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Helpers;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class APLValueConverter<T> : JsonConverter<APLValue<T>>
{
    private static IList<JsonValueKind> _simpleTypes =
        [JsonValueKind.String, JsonValueKind.Number, JsonValueKind.False, JsonValueKind.True];

    public override APLValue<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null && (reader.TokenType != JsonTokenType.StartArray &&
                                                       reader.TokenType != JsonTokenType.StartObject))
        {
            return null;
        }

        var returnValue = new APLValue<T>();

        var genericType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        genericType = Nullable.GetUnderlyingType(genericType) ?? genericType;

        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        if (_simpleTypes.Contains(root.ValueKind) && !TypeMatches(root.ValueKind, genericType))
        {
            returnValue.Expression = root.ValueKind == JsonValueKind.String
                ? root.GetString()
                : root.GetRawText();
            return returnValue;
        }
        // Enum with [EnumMember] support
        if (genericType.IsEnum && root.ValueKind == JsonValueKind.String)
        {
            var str = root.GetString();
            if (EnumHelper.TryParseEnumWithEnumMemberSupport(genericType, str, out var parsedEnum))
            {
                returnValue.Value = (T)parsedEnum!;
            }
            else
            {
                returnValue.Expression = str;
            }

            return returnValue;
        }

        if (genericType == typeof(object))
        {
            returnValue.Value = (T)new ObjectConverter().Read(ref reader, typeToConvert, options);
        }
        else
        {
            returnValue.Value = (T)document.Deserialize(genericType, options);
        }

        return returnValue;
    }

    public override void Write(Utf8JsonWriter writer, APLValue<T> value, JsonSerializerOptions options)
    {
        var obj = !string.IsNullOrWhiteSpace(value.Expression) ? value.Expression : value.GetValue();
        JsonSerializer.Serialize(writer, obj, options);
    }

    private bool TypeMatches(JsonValueKind jsonValueKind, Type t)
    {
        return jsonValueKind switch
        {
            JsonValueKind.String when t.IsStringType() => true,
            JsonValueKind.False or JsonValueKind.True when typeof(bool) == t => true,
            JsonValueKind.Number when t.IsNumberType() => true,
            _ => false
        };
    }
}

internal static partial class TypeExtensions
{
    private static readonly IList<Type> NumberTypes =
        [typeof(int), typeof(short), typeof(long), typeof(double), typeof(float), typeof(decimal)];

    internal static bool IsStringType(this Type type) => type == typeof(string) || type.IsEnum;

    internal static bool IsNumberType(this Type type) => NumberTypes.Contains(type);
}

public class APLEnumerableValueConverter<TValue, TList> : JsonConverter<APLValue<TList>>
    where TList : IEnumerable<TValue>
{
    public override APLValue<TList>? Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        var returnValue = new APLValue<TList>();

        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;
        switch (root.ValueKind)
        {
            case JsonValueKind.Object:
                returnValue.Value = (TList)new List<TValue> { root.Deserialize<TValue>(options) }.AsEnumerable();
                returnValue.IsSingle = true;
                break;
            case JsonValueKind.Array:
                returnValue.Value = (TList)root.EnumerateArray()
                    .Select(element => JsonSerializer.Deserialize<TValue>(element.GetRawText(), options))
                    // The ToList() is required here to iterate the array as the JsonDcoument above will be disposed
                    .ToList()
                    .AsEnumerable();
                break;
            case JsonValueKind.String:
                returnValue.Expression = root.GetString();
                break;
            default:
                throw new JsonException("Invalid JSON for Enumeration");
        }

        return returnValue;
    }

    public override void Write(Utf8JsonWriter writer, APLValue<TList> value, JsonSerializerOptions options)
    {
        object? obj;
        if (!string.IsNullOrEmpty(value.Expression))
        {
            obj = value.Expression;
        } else if (value.IsSingle)
        {
            obj = value.Value!.First();
        }
        else
        {
            obj = value.Value;
        }
        JsonSerializer.Serialize(writer, obj, options);
    }
}

public class APLDimensionValueConverter : JsonConverter<APLDimensionValue>
{
    public override APLDimensionValue? Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        return reader.TokenType == JsonTokenType.Number
            ? new APLDimensionValue(reader.GetInt32().ToString())
            : new APLDimensionValue(reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, APLDimensionValue value, JsonSerializerOptions options)
    {
        var obj = !string.IsNullOrWhiteSpace(value.Expression) ? value.Expression : value.GetValue();
        JsonSerializer.Serialize(writer, obj, options);
    }
}

public class APLAbsoluteDimensionValueConverter : JsonConverter<APLAbsoluteDimensionValue>
{
    public override APLAbsoluteDimensionValue? Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        return reader.TokenType == JsonTokenType.Number
            ? new APLAbsoluteDimensionValue(reader.GetInt32().ToString())
            : new APLAbsoluteDimensionValue(reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, APLAbsoluteDimensionValue value, JsonSerializerOptions options)
    {
        var obj = !string.IsNullOrWhiteSpace(value.Expression) ? value.Expression : value.GetValue();
        JsonSerializer.Serialize(writer, obj, options);
    }
}

public class APLValueConverterFactory : JsonConverterFactory
{
    private static Type _aplDimensionType = typeof(APLDimensionValue);
    private static Type _aplAbsoluteDimensionType = typeof(APLAbsoluteDimensionValue);
    private static Type _aplObjectType = typeof(APLValue<object>);
    private static List<Type> _dimensionTypes = [_aplDimensionType, _aplAbsoluteDimensionType, _aplObjectType];
    private static readonly ConcurrentDictionary<Type, System.Text.Json.Serialization.JsonConverter?> _converterCache = new();
    public override bool CanConvert(Type typeToConvert)
    {
        if (_dimensionTypes.Contains(typeToConvert))
            return true;

        if (!typeToConvert.IsGenericType)
            return false;

        if (typeToConvert.GetGenericTypeDefinition() != typeof(APLValue<>))
            return false;

        return true;
    }

    public override System.Text.Json.Serialization.JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return _converterCache.GetOrAdd(typeToConvert, CreateConverterInternal);
    }
    
    private System.Text.Json.Serialization.JsonConverter? CreateConverterInternal(Type typeToConvert)
    {
        if (_dimensionTypes.Contains(typeToConvert))
        {
            return typeToConvert switch
            {
                var t when t == _aplDimensionType => new APLDimensionValueConverter(),
                var t when t == _aplAbsoluteDimensionType => new APLAbsoluteDimensionValueConverter(),
                var t when t == _aplObjectType => new APLObjectConverter(),
                _ => throw new JsonException("Should never happen!")
            };
        }

        var typeArguments = typeToConvert.GetGenericArguments();
        var valueType = typeArguments[0];

        if (valueType.IsGenericType && valueType.IsEnumerableOfType())
        {
            var innerValueType = valueType.GetGenericArguments().First();
            return (System.Text.Json.Serialization.JsonConverter)Activator.CreateInstance(
                typeof(APLEnumerableValueConverter<,>).MakeGenericType(innerValueType, valueType),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: null,
                culture: null)!;
        }

        var converter = (System.Text.Json.Serialization.JsonConverter)Activator.CreateInstance(
            typeof(APLValueConverter<>).MakeGenericType(valueType), BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: null,
            culture: null)!;

        return converter;
    }
}

internal static partial class TypeExtensions
{
    internal static bool IsEnumerableOfType(this Type type) =>
        type.GetGenericTypeDefinition() == typeof(IEnumerable<>) || type.GetInterfaces().Any(i =>
            i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
            i.GetGenericArguments().First() == type.GetGenericArguments().First());
}