namespace AlexaVoxCraft.Model.Apl.Tests.Document;

/// <summary>
/// Coverage for <see cref="APLGradient"/> construction. Response-side serialize only (see
/// docs/plans/0002-cloudwatch-driven-verify-tests-and-warning-cleanup.md).
/// </summary>
public sealed class GradientTests() : TestBase<GradientTests>
{
    [Fact]
    public async Task LinearGradient_Serializes()
    {
        var gradient = new APLGradient
        {
            Type = APLGradientType.Linear,
            ColorRange = ["white", "transparent"],
            InputRange = [0.0, 0.5]
        };

        await TestHelper.VerifySerializedObject(gradient, AlexaJson, "LinearGradient");
    }
}
