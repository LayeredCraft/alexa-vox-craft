using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Response.Directive;

public class AudioPlayerPlayDirective : IDirective
{
    public const string DirectiveType = "AudioPlayer.Play";

    [JsonPropertyName("playBehavior")]
    [JsonRequired]
    [JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<PlayBehavior>))]
    public PlayBehavior PlayBehavior { get; set; }

    [JsonPropertyName("audioItem")]
    [JsonRequired]
    public AudioItem AudioItem { get; set; }

    [JsonPropertyName("type")]
    public string Type => DirectiveType;
}