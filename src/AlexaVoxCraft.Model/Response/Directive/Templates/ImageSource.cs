using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Response.Directive.Templates;

// [JsonConverter(typeof(ImageSourceConverter))]
public class ImageSource
{
    [JsonPropertyName("url"), JsonRequired]
    public string Url { get; set; }

    [JsonPropertyName("size")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Size { get; set; }

    [JsonPropertyName("widthPixels")]
    public int Width { get; set; }

    [JsonPropertyName("heightPixels")]
    public int Height { get; set; }

    public bool ShouldSerializeWidth()
    {
        return Width > 0;
    }

    public bool ShouldSerializeHeight()
    {
        return Height > 0;
    }
}