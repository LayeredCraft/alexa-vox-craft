using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.WebApiGames;

/// <summary>
/// A Web API for Games transformer. It converts the text (<see cref="TextToSpeech"/>) or SSML
/// (<see cref="SsmlToSpeech"/>) found at <see cref="InputPath"/> into speech. <see cref="InputPath"/>
/// names a property within the directive's data (<c>Start</c>) or message (<c>HandleMessage</c>) object,
/// and <see cref="OutputName"/>, when present, names the property added to that object with the output.
/// Instances are created through the <see cref="TextToSpeech"/> and <see cref="SsmlToSpeech"/> factories.
/// </summary>
public sealed class HTMLTransformer
{
    /// <summary>Initializes a new transformer.</summary>
    /// <param name="transformer">The transformer kind; one of <c>textToSpeech</c> or <c>ssmlToSpeech</c>.</param>
    /// <param name="inputPath">The non-empty path to the text or SSML property to convert.</param>
    /// <param name="outputName">An optional output property name.</param>
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

    /// <summary>
    /// Gets the path to the property within the directive's data or message object whose text or SSML is
    /// converted to speech.
    /// </summary>
    [JsonPropertyName("inputPath")]
    public string InputPath { get; }

    /// <summary>
    /// Gets the optional property name added to the directive's data or message object with the speech
    /// output. When <see langword="null"/> the output replaces the value at <see cref="InputPath"/> and the
    /// property is omitted from the serialized directive.
    /// </summary>
    [JsonPropertyName("outputName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? OutputName { get; }

    /// <summary>Creates a transformer that converts the text found at <paramref name="inputPath"/> into speech.</summary>
    /// <param name="inputPath">The non-empty path to the text property to convert.</param>
    /// <param name="outputName">An optional output property name; when omitted the value at <paramref name="inputPath"/> is replaced.</param>
    public static HTMLTransformer TextToSpeech(string inputPath, string? outputName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(inputPath);
        return new HTMLTransformer("textToSpeech", inputPath, outputName);
    }

    /// <summary>Creates a transformer that converts the SSML found at <paramref name="inputPath"/> into speech.</summary>
    /// <param name="inputPath">The non-empty path to the SSML property to convert.</param>
    /// <param name="outputName">An optional output property name; when omitted the value at <paramref name="inputPath"/> is replaced.</param>
    public static HTMLTransformer SsmlToSpeech(string inputPath, string? outputName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(inputPath);
        return new HTMLTransformer("ssmlToSpeech", inputPath, outputName);
    }
}
