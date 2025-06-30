using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Response.Directive;

public class ClearQueueDirective : IDirective
{
    public const string DirectiveType = "AudioPlayer.ClearQueue";

    [JsonPropertyName("type")]
    public string Type => DirectiveType;

    [JsonPropertyName("clearBehavior")]
    [JsonRequired]
    [JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<ClearBehavior>))]
    public ClearBehavior ClearBehavior { get; set; }
}