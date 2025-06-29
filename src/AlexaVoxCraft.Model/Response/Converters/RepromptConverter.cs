using System.Text.Json;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Response.Converters;

public class RepromptConverter : JsonConverter<Reprompt>
{
    public override Reprompt? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<Reprompt>(ref reader, options);
    }

    public override void Write(Utf8JsonWriter writer, Reprompt value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        // outputSpeech
        if (value.OutputSpeech is not null)
        {
            writer.WritePropertyName("outputSpeech");
            JsonSerializer.Serialize(writer, value.OutputSpeech, options);
        }

        // directives (only if non-empty)
        if (value.Directives is { Count: > 0 })
        {
            writer.WritePropertyName("directives");
            JsonSerializer.Serialize(writer, value.Directives, options);
        }

        writer.WriteEndObject();
    }
}