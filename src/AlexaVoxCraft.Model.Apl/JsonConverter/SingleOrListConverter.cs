using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public abstract class SingleOrListConverter<TValue> : JsonConverter<object>
{
    protected SingleOrListConverter(bool alwaysOutputArray)
    {
        AlwaysOutputArray = alwaysOutputArray;
    }
    protected virtual bool AlwaysOutputArray { get; }

    protected virtual object OutputArrayItem(TValue value) => value;
    
    protected virtual JsonTokenType SingleTokenType => JsonTokenType.StartObject;

    protected virtual void ReadSingle(ref Utf8JsonReader reader, JsonSerializerOptions options, List<TValue> list)
    {
        var value = JsonSerializer.Deserialize<TValue>(ref reader, options);
        if (value is not null)
        {
            list.Add(value);
        }
    }

    protected virtual bool IsStringExpression(ref Utf8JsonReader reader) =>
        reader.TokenType == JsonTokenType.String;

    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert == typeof(APLValue<IList<TValue>>) || typeToConvert == typeof(IList<TValue>);

    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var list = new List<TValue>();

        if (reader.TokenType == SingleTokenType)
        {
            ReadSingle(ref reader, options, list);
        }
        else if (IsStringExpression(ref reader))
        {
            var expr = reader.GetString();
            return APLValue.To<IList<TValue>>(expr);
        }

        if (reader.TokenType == JsonTokenType.StartArray)
        {
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                list.Add(JsonSerializer.Deserialize<TValue>(ref reader, options)!);
            }
        }

        if (typeToConvert == typeof(APLValue<IList<TValue>>))
        {
            return new APLValue<IList<TValue>>(list);
        }

        return list;
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        IList<TValue>? list;

        switch (value)
        {
            case APLValue<IList<TValue>> { Expression: not null } apl:
                JsonSerializer.Serialize(writer, apl.Expression, options);
                return; // ✅ Return early since expression was written directly
            case APLValue<IList<TValue>> apl:
                list = apl.Value;
                break;
            case IList<TValue> normalList:
                list = normalList;
                break;
            default:
                throw new JsonException($"Unexpected type for {nameof(SingleOrListConverter<TValue>)}");
        }

        if (!AlwaysOutputArray && list is { Count: 1 })
        {
            JsonSerializer.Serialize(writer, OutputArrayItem(list[0]), options);
            return;
        }

        writer.WriteStartArray();
        foreach (var item in list)
        {
            JsonSerializer.Serialize(writer, OutputArrayItem(item), options);
        }

        writer.WriteEndArray();
    }
}