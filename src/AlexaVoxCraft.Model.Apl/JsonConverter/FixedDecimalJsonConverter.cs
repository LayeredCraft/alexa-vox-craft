using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public sealed class FixedDecimalJsonConverter : JsonConverter<double>
{
    public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.GetDouble();

    public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
    {
        writer.WriteRawValue(value.ToString("0.0####", CultureInfo.InvariantCulture));
    }

    public static void AddSupport()
    {
        AlexaJsonOptions.RegisterConverter(new FixedDecimalJsonConverter());
    }
}