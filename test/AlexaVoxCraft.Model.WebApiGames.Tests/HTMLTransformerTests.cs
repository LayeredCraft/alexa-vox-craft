using Compono.XunitV3;

namespace AlexaVoxCraft.Model.WebApiGames.Tests;

public sealed class HTMLTransformerTests
{
    [Fact]
    public void TextToSpeech_ProducesExpectedTransformer()
    {
        var transformer = HTMLTransformer.TextToSpeech("card/headline");

        transformer.Transformer.Should().Be("textToSpeech");
        transformer.InputPath.Should().Be("card/headline");
        transformer.OutputName.Should().BeNull();
    }

    [Fact]
    public void SsmlToSpeech_ProducesExpectedTransformerWithOutputName()
    {
        var transformer = HTMLTransformer.SsmlToSpeech("card/body", "spokenBody");

        transformer.Transformer.Should().Be("ssmlToSpeech");
        transformer.InputPath.Should().Be("card/body");
        transformer.OutputName.Should().Be("spokenBody");
    }

    [Theory]
    [Compose]
    public void SsmlToSpeech_PreservesIncidentalOutputName(string outputName)
    {
        var transformer = HTMLTransformer.SsmlToSpeech("card/body", outputName);

        transformer.OutputName.Should().Be(outputName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void TextToSpeech_RejectsMissingInputPath(string? inputPath)
    {
        var act = () => HTMLTransformer.TextToSpeech(inputPath!);

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SsmlToSpeech_RejectsMissingInputPath(string? inputPath)
    {
        var act = () => HTMLTransformer.SsmlToSpeech(inputPath!);

        act.Should().Throw<ArgumentException>();
    }
}
