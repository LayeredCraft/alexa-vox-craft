using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class APLObjectConverter : JsonConverter<APLValue<object>>
{
    private readonly JsonConverter<object> _innerConverter = new ObjectConverter();
    public override APLValue<object>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var returnValue = new APLValue<object>();
        
        returnValue.Value = _innerConverter.Read(ref reader, typeToConvert, options);

        return returnValue;
    }

    public override void Write(Utf8JsonWriter writer, APLValue<object> value, JsonSerializerOptions options)
    {
        var obj = !string.IsNullOrWhiteSpace(value.Expression) ? value.Expression : value.GetValue();
        JsonSerializer.Serialize(writer, obj, options);
    }
}