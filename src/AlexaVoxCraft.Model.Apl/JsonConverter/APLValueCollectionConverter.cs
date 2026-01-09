using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class APLValueCollectionConverter<T> : JsonConverter<APLValueCollection<T>>
{
    private readonly bool _alwaysOutputArray;

    public APLValueCollectionConverter(bool alwaysOutputArray)
    {
        _alwaysOutputArray = alwaysOutputArray;
    }

    public override APLValueCollection<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        var collection = new APLValueCollection<T>();

        switch (reader.TokenType)
        {
            // Handle expression strings: "${data.items}"
            case JsonTokenType.String:
                collection.Expression = reader.GetString();
                return collection;
            // Handle single object (Alexa allows single item instead of array)
            case JsonTokenType.StartObject:
            {
                var item = JsonSerializer.Deserialize<T>(ref reader, options);
                if (item is not null)
                {
                    collection.Items!.Add(item);
                }
                return collection;
            }
            // Handle array
            case JsonTokenType.StartArray:
            {
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    var item = JsonSerializer.Deserialize<T>(ref reader, options);
                    if (item is not null)
                    {
                        collection.Items!.Add(item);
                    }
                }
                return collection;
            }
            default:
                throw new JsonException($"Unexpected token {reader.TokenType} for APLValueCollection<{typeof(T).Name}>");
        }
    }

    public override void Write(Utf8JsonWriter writer, APLValueCollection<T> value, JsonSerializerOptions options)
    {
        // Expression takes precedence
        if (!string.IsNullOrEmpty(value.Expression))
        {
            writer.WriteStringValue(value.Expression);
            return;
        }

        var items = value.Items;
        if (items is null || items.Count == 0)
        {
            writer.WriteNullValue();
            return;
        }

        // Write single item as object if AlwaysOutputArray is false
        if (!_alwaysOutputArray && items.Count == 1)
        {
            JsonSerializer.Serialize(writer, items[0], options);
            return;
        }

        // Write array
        writer.WriteStartArray();
        foreach (var item in items)
        {
            JsonSerializer.Serialize(writer, item, options);
        }
        writer.WriteEndArray();
    }
}