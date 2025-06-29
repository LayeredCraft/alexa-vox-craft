using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Response.Directive;

public class VoicePlayerSpeakDirective : IProgressiveResponseDirective
{
    public const string DirectiveType = "VoicePlayer.Speak";
    internal VoicePlayerSpeakDirective()
    {
    }

    public VoicePlayerSpeakDirective(Ssml.Speech speech) : this(speech.ToXml())
    {
    }

    public VoicePlayerSpeakDirective(string speech)
    {
        Speech = speech;
    }

    [JsonPropertyName("type")]
    [JsonInclude]
    public string Type => DirectiveType;

    [JsonPropertyName("speech")]
    public string Speech { get; }
}