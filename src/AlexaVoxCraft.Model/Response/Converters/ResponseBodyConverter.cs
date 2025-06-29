using System.Text.Json;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Response.Converters;

public class ResponseBodyConverter : JsonConverter<ResponseBody>
{
    public override ResponseBody? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Optional: implement this if you need deserialization
        return JsonSerializer.Deserialize<ResponseBody>(ref reader, options);
    }

    public override void Write(Utf8JsonWriter writer, ResponseBody value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        // outputSpeech
        if (value.OutputSpeech is not null)
        {
            writer.WritePropertyName("outputSpeech");
            JsonSerializer.Serialize(writer, value.OutputSpeech, options);
        }

        // card
        if (value.Card is not null)
        {
            writer.WritePropertyName("card");
            JsonSerializer.Serialize(writer, value.Card, options);
        }

        // reprompt
        if (value.Reprompt is not null)
        {
            writer.WritePropertyName("reprompt");
            JsonSerializer.Serialize(writer, value.Reprompt, options);
        }

        // shouldEndSession (just call the property 👇)
        if (value.ShouldEndSession is not null)
        {
            writer.WriteBoolean("shouldEndSession", value.ShouldEndSession.Value);
        }
        
        // directives
        if (value.Directives is { Count: > 0 })
        {
            writer.WritePropertyName("directives");
            JsonSerializer.Serialize(writer, value.Directives, options);
        }

        writer.WriteEndObject();
    }
}