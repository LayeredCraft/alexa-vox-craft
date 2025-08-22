using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace AlexaVoxCraft.Model.Response;

public class Reprompt
{
    public Reprompt()
    {
    }

    public Reprompt(string text)
    {
        OutputSpeech = new PlainTextOutputSpeech {Text = text};
    }

    public Reprompt(Ssml.Speech speech)
    {
        OutputSpeech = new SsmlOutputSpeech {Ssml = speech.ToXml()};
    }

    [JsonPropertyName("outputSpeech")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IOutputSpeech? OutputSpeech { get; set; }

    [JsonPropertyName("directives")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<IDirective> Directives { get; set; } = new List<IDirective>();

    public bool ShouldSerializeDirectives()
    {
        return Directives.Count > 0;
    }

    public static List<Action<JsonTypeInfo>> GetJsonSerializationOptions() =>
    [
        ti =>
        {
            var prop = ti.Properties.FirstOrDefault(p => p.Name == "directives");
            if (prop is not null)
            {
                prop.ShouldSerialize = (obj, _) =>
                {
                    var response = (ResponseBody)obj;
                    return response.Directives is { Count: > 0 };
                };
            }
        }
    ];
}