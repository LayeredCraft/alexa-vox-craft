using System.Text.Json;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Serialization;

public class ObjectConverter : JsonConverter<object>
{
    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Deserialize(ref reader, options);
    }

    private object? Deserialize(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var value = reader.TokenType switch
        {
            JsonTokenType.Number => DeserializeNumber(ref reader),
            JsonTokenType.String => reader.GetString(),
            JsonTokenType.False or JsonTokenType.True => reader.GetBoolean(),
            JsonTokenType.Null => null,
            JsonTokenType.StartObject => DeserializeObject(ref reader, options),
            JsonTokenType.StartArray => DeserializeEnumerable(ref reader, options),
            _ => throw new JsonException($"Unsupported JSON value kind: {reader.TokenType}")
        };
        return value;
    }

    private object? DeserializeEnumerable(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        reader.Read();

        var list = new List<object>();

        while (reader.TokenType != JsonTokenType.EndArray)
        {
            var item = Deserialize(ref reader, options);
            list.Add(item);
            reader.Read();
        }

        return list;
    }

    private object? DeserializeObject(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        reader.Read();

        var dictionary = new Dictionary<string, object>();

        while (reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("Expected property name token.");
            }

            var propertyName = reader.GetString();
            reader.Read();
            var propertyValue = Deserialize(ref reader, options);
            dictionary[propertyName] = propertyValue;
            reader.Read();
        }

        return dictionary;
    }

    private static object? DeserializeNumber(ref Utf8JsonReader reader)
    {
        return reader.TokenType switch
        {
            _ when reader.TryGetInt32(out var intValue) => intValue,
            _ when reader.TryGetInt64(out var longValue) => longValue,
            _ when reader.TryGetDouble(out var doubleValue) => doubleValue,
            _ when reader.TryGetDecimal(out var decimalValue) => decimalValue,
            _ => throw new JsonException("Failed to determine numeric type.")
        };
    }
    
    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
