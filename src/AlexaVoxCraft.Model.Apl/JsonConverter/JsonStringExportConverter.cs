using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class JsonStringExportConverter : JsonConverter<Export>
{
    public override Export? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.String => new Export(reader.GetString()),
            JsonTokenType.StartObject => ReadExportObject(ref reader),
            _ => throw new JsonException($"Unexpected token {reader.TokenType} when deserializing Export.")
        };
    }

    private Export ReadExportObject(ref Utf8JsonReader reader)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        var name = root.TryGetProperty("name", out var nameProp)
            ? nameProp.GetString()
            : throw new JsonException("Missing 'name' property for Export object");

        var export = new Export(name);

        if (root.TryGetProperty("description", out var descProp) && descProp.ValueKind == JsonValueKind.String)
        {
            export.Description = descProp.GetString();
        }

        return export;
    }

    public override void Write(Utf8JsonWriter writer, Export value, JsonSerializerOptions options)
    {
        if (string.IsNullOrWhiteSpace(value.Description))
        {
            writer.WriteStringValue(value.Name);
        }
        else
        {
            writer.WriteStartObject();
            writer.WriteString("name", value.Name);
            writer.WriteString("description", value.Description);
            writer.WriteEndObject();
        }
    }
}