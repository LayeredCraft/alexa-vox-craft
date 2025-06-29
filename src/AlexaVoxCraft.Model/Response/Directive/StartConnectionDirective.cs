using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.ConnectionTasks;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Response.Directive;

public class StartConnectionDirective:IDirective
{
    public const string DirectiveType = "Connections.StartConnection";

    [JsonPropertyName("type")]
    public string Type => DirectiveType;

    [JsonPropertyName("uri")]
    public string Uri { get; set; }

    [JsonPropertyName("input")]
    public IConnectionTask Input { get; set; }

    [JsonPropertyName("token")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Token { get; set; }

    [JsonPropertyName("onComplete")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<OnCompleteAction>))]
    public OnCompleteAction? OnComplete { get; set; }

    public StartConnectionDirective(){}

    public StartConnectionDirective(IConnectionTask input, string token)
    {
        Uri = input.ConnectionUri;
        Input = input;
        Token = token;
    }
}