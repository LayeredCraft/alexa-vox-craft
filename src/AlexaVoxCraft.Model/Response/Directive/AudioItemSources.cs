using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Response.Directive;

public class AudioItemSources
{
	[JsonPropertyName("sources")]
	public List<AudioItemSource> Sources { get; set; } = [];
}