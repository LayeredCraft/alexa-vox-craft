using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Response.Directive;

public class AudioItemMetadata
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    [JsonPropertyName("subtitle")]
    public string Subtitle { get; set; } = null!;

    [JsonPropertyName("art")]
    public AudioItemSources Art { get; set; } = new AudioItemSources();

    [JsonPropertyName("backgroundImage")]
    public AudioItemSources BackgroundImage { get; set; } = new AudioItemSources();
}