using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.WebApiGames;

/// <summary>
/// A Web API for Games transformer that rewrites part of the HTML document before it is presented.
/// Prefer the <see cref="TextToSpeech"/> and <see cref="SsmlToSpeech"/> factories over the constructor.
/// </summary>
public sealed class HTMLTransformer
{
    /// <summary>Initializes a new transformer.</summary>
    /// <param name="transformer">The transformer kind; one of <c>textToSpeech</c> or <c>ssmlToSpeech</c>.</param>
    /// <param name="inputPath">The non-empty source path.</param>
    /// <param name="outputName">An optional output path.</param>
    /// <remarks>Internal deserialization entry point; the factories are the public creation API.</remarks>
    [JsonConstructor]
    internal HTMLTransformer(string transformer, string inputPath, string? outputName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(transformer);
        ArgumentException.ThrowIfNullOrWhiteSpace(inputPath);

        Transformer = transformer;
        InputPath = inputPath;
        OutputName = outputName;
    }

    /// <summary>Gets the transformer kind; one of <c>textToSpeech</c> or <c>ssmlToSpeech</c>.</summary>
    [JsonPropertyName("transformer")]
    public string Transformer { get; }

    /// <summary>Gets the path to the source value inside the HTML document that this transformer reads.</summary>
    [JsonPropertyName("inputPath")]
    public string InputPath { get; }

    /// <summary>
    /// Gets the optional output path. When <see langword="null"/> the source value is replaced in place
    /// and the property is omitted from the serialized directive.
    /// </summary>
    [JsonPropertyName("outputName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? OutputName { get; }

    /// <summary>Creates a transformer that converts the text at <paramref name="inputPath"/> to speech.</summary>
    /// <param name="inputPath">The non-empty source path to convert.</param>
    /// <param name="outputName">An optional output path; when omitted the source value is replaced.</param>
    public static HTMLTransformer TextToSpeech(string inputPath, string? outputName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(inputPath);
        return new HTMLTransformer("textToSpeech", inputPath, outputName);
    }

    /// <summary>Creates a transformer that converts the SSML at <paramref name="inputPath"/> to speech.</summary>
    /// <param name="inputPath">The non-empty source path to convert.</param>
    /// <param name="outputName">An optional output path; when omitted the source value is replaced.</param>
    public static HTMLTransformer SsmlToSpeech(string inputPath, string? outputName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(inputPath);
        return new HTMLTransformer("ssmlToSpeech", inputPath, outputName);
    }
}
