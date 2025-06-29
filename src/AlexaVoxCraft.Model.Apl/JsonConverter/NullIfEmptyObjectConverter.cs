using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public sealed class NullIfEmptyObjectConverter<T> : JsonConverter<T?> where T : class
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (root.ValueKind == JsonValueKind.Object && !root.EnumerateObject().Any())
        {
            return null;
        }

        // Avoid recursion by cloning options without this converter
        var safeOptions = new JsonSerializerOptions(options);
        safeOptions.Converters.Remove(
            safeOptions.Converters.FirstOrDefault(c => c is NullIfEmptyObjectConverter<T>)
        );

        return JsonSerializer.Deserialize<T>(root.GetRawText(), safeOptions);
    }

    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}
