using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Response.Directive;

public class StopDirective : IDirective
{
    public const string DirectiveType = "AudioPlayer.Stop";

    [JsonPropertyName("type")]
    public string Type => DirectiveType;
}