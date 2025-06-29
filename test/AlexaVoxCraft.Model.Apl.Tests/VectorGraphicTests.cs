using AlexaVoxCraft.Model.Apl.VectorGraphics;

namespace AlexaVoxCraft.Model.Apl.Tests;

public class VectorGraphicTests
{
    [Fact]
    public void AVGGeneratesCorrectJson()
    {
        Utility.AssertSerialization<AVG>("AVG.json");
    }
}