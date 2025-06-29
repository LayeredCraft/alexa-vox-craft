using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Response.Directive;

public class AudioItemMetadata
{
	[JsonPropertyName("title")]
	public string Title { get; set; }

	[JsonPropertyName("subtitle")]
	public string Subtitle { get; set; }

	[JsonPropertyName("art")]
	public AudioItemSources Art { get; set; } = new AudioItemSources();

	[JsonPropertyName("backgroundImage")]
	public AudioItemSources BackgroundImage { get; set; } = new AudioItemSources();
}