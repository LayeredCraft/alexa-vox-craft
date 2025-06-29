using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class DocumentBackgroundColorConverter : JsonConverter<DocumentBackgroundColor>
{
    public override void Write(Utf8JsonWriter writer, DocumentBackgroundColor value, JsonSerializerOptions options)
    {
        if (!string.IsNullOrWhiteSpace(value.Color))
        {
            writer.WriteStringValue(value.Color);
        }
        else if (value.Gradient is not null)
        {
            JsonSerializer.Serialize(writer, value.Gradient, options);
        }
        else
        {
            writer.WriteNullValue();
        }
    }

    public override DocumentBackgroundColor? Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        // Handle string input (e.g., "white")
        if (reader.TokenType == JsonTokenType.String)
        {
            var color = reader.GetString();
            return new DocumentBackgroundColor(color!);
        }

        // Handle object input (e.g., gradient JSON)
        using var doc = JsonDocument.ParseValue(ref reader);
        var gradient = doc.RootElement.Deserialize<APLGradient>(options);
        return new DocumentBackgroundColor(gradient!);
    }
}