using AlexaVoxCraft.Model.Apl.Package;

namespace AlexaVoxCraft.Model.Apl.Tests;

public class APLPackageTests
{
    [Fact]
    public void ValidPackageDocument()
    {
        Utility.AssertSerialization<APLPackage>("APLPackage.json");
    }
}