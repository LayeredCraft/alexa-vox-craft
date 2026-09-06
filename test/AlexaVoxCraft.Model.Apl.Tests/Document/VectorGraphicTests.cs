using AlexaVoxCraft.Model.Apl.VectorGraphics;

namespace AlexaVoxCraft.Model.Apl.Tests.Document;

/// <summary>
/// Coverage for <see cref="AVG"/> (Alexa Vector Graphic) document construction. Response-side
/// serialize only (see docs/plans/0002-cloudwatch-driven-verify-tests-and-warning-cleanup.md).
/// </summary>
public sealed class VectorGraphicTests() : TestBase<VectorGraphicTests>
{
    [Fact]
    public async Task AVG_Serializes_WithPathAndGroup()
    {
        var avg = new AVG
        {
            Version = AVGVersion.V1_2,
            Width = new APLAbsoluteDimensionValue(24, "dp"),
            Height = new APLAbsoluteDimensionValue(24, "dp"),
            ViewportWidth = 24,
            ViewportHeight = 24,
            Items =
            [
                new AVGPath { PathData = "M12,2L2,7L12,12L22,7L12,2Z", Fill = "#FFFFFF" },
                new AVGGroup { Items = [new AVGPath { PathData = "M2,17L12,22L22,17" }] }
            ]
        };

        await TestHelper.VerifySerializedObject(avg, AlexaJson, "AVG_WithPathAndGroup");
    }
}
