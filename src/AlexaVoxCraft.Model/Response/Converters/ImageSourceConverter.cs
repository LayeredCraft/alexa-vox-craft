using System.Text.Json;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Directive.Templates;

namespace AlexaVoxCraft.Model.Response.Converters;

public class ImageSourceConverter : JsonConverter<ImageSource>
{
    public override ImageSource? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Optional: implement if you need deserialization support
        return JsonSerializer.Deserialize<ImageSource>(ref reader, options);
    }

    public override void Write(Utf8JsonWriter writer, ImageSource value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString("url", value.Url);

        if (!string.IsNullOrEmpty(value.Size))
            writer.WriteString("size", value.Size);

        if (value.Width > 0)
            writer.WriteNumber("widthPixels", value.Width);

        if (value.Height > 0)
            writer.WriteNumber("heightPixels", value.Height);

        writer.WriteEndObject();
    }
}