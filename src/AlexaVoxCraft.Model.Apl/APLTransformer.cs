using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class APLTransformer
{
    public APLTransformer() { }

    public APLTransformer(string transformerType, string outputName)
    {
        Transformer = transformerType;
        OutputName = outputName;
    }

    public APLTransformer(string transformerType, string inputPath, string outputName)
    {
        Transformer = transformerType;
        InputPath = inputPath;
        OutputName = outputName;
    }

    [JsonPropertyName("inputPath")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? InputPath { get; set; }

    [JsonPropertyName("outputName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? OutputName { get; set; }

    [JsonPropertyName("transformer")]
    public string Transformer { get; set; }

    [JsonPropertyName("template")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Template { get; set; }

    public static APLTransformer SsmlToSpeech(string inputPath, string outputName)
    {
        return new APLTransformer("ssmlToSpeech",inputPath,outputName);
    }

    public static APLTransformer SsmlToText(string inputPath, string outputName)
    {
        return new APLTransformer("ssmlToText", inputPath, outputName);
    }

    public static APLTransformer TextToHint(string inputPath, string outputName)
    {
        return new APLTransformer("textToHint", inputPath, outputName);
    }

    public static APLTransformer TextToSpeech(string inputPath, string outputName)
    {
        return new APLTransformer("textToSpeech",inputPath,outputName);
    }

    public static APLTransformer AplAudioToSpeech(string template, string outputName)
    {
        return new APLTransformer("aplAudioToSpeech", outputName){Template=template};
    }
}