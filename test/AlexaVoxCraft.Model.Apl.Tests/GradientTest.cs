using System.Text.Json;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Tests;

public class GradientTest
{
    private readonly ITestOutputHelper _output;

    public GradientTest(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [Fact]
    public void LinearGradient()
    {
        // APLSupport.Add();
        
        var expectedRaw = @"{
        ""type"": ""linear"",
        ""colorRange"": [""white"", ""transparent""],
        ""inputRange"": [0.0, 0.5]
    }";

        using var expectedDoc = JsonDocument.Parse(expectedRaw);
        var expectedElement = expectedDoc.RootElement;

        var gradient = new APLGradient
        {
            Type = APLGradientType.Linear,
            ColorRange = new[] { "white", "transparent" },
            InputRange = new[] { 0.0, 0.5 }
        };

        var json = JsonSerializer.Serialize(gradient, AlexaJsonOptions.DefaultOptions);
        using var actualDoc = JsonDocument.Parse(json);
        var actualElement = actualDoc.RootElement;

        var diffs = new List<string>();

        var areEqual = expectedElement.JsonElementDeepEquals(actualElement, "", diffs);
        
        if (!areEqual)
        {
            _output.WriteLine("❌ JSON DeepEquals failed. Differences:");
            foreach (var diff in diffs)
            {
                _output.WriteLine($"• {diff}");
            }
        }
        Assert.True(areEqual);
    }
}