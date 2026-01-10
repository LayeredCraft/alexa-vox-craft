using System;
using System.Collections.Generic;
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

    protected virtual object OutputArrayItem(T value) => value!;

    protected virtual JsonTokenType SingleTokenType => JsonTokenType.StartObject;

    protected virtual void ReadSingle(ref Utf8JsonReader reader, JsonSerializerOptions options, IList<T> list)
    {
        var value = JsonSerializer.Deserialize<T>(ref reader, options);
        if (value is not null)
        {
            list.Add(value);
        }
    }

    public override APLValueCollection<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        var collection = new APLValueCollection<T>();

        // Handle expression strings: "${data.items}"
        if (reader.TokenType == JsonTokenType.String)
        {
            collection.Expression = reader.GetString();
            return collection;
        }

        // Handle single item using virtual method
        if (reader.TokenType == SingleTokenType)
        {
            ReadSingle(ref reader, options, collection);
            return collection;
        }

        // Handle array
        if (reader.TokenType == JsonTokenType.StartArray)
        {
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                var item = JsonSerializer.Deserialize<T>(ref reader, options);
                if (item is not null)
                {
                    collection.Add(item);
                }
            }
            return collection;
        }

        throw new JsonException($"Unexpected token {reader.TokenType} for APLValueCollection<{typeof(T).Name}>");
    }

    public override void Write(Utf8JsonWriter writer, APLValueCollection<T> value, JsonSerializerOptions options)
    {
        // Expression takes precedence
        if (!string.IsNullOrEmpty(value.Expression))
        {
            writer.WriteStringValue(value.Expression);
            return;
        }

        if (value.Count == 0)
        {
            writer.WriteNullValue();
            return;
        }

        // Write single item using virtual method if AlwaysOutputArray is false
        if (!_alwaysOutputArray && value.Count == 1)
        {
            JsonSerializer.Serialize(writer, OutputArrayItem(value[0]), options);
            return;
        }

        // Write array using virtual method for each item
        writer.WriteStartArray();
        foreach (var item in value)
        {
            JsonSerializer.Serialize(writer, OutputArrayItem(item), options);
        }
        writer.WriteEndArray();
    }
}