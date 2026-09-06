using AlexaVoxCraft.Model.Apl.Components;

namespace AlexaVoxCraft.Model.Apl.Tests.Components;

public class PagerTests : TestBase<PagerTests>
{
    [Fact]
    public async Task Pager_WithConstructor_Serializes()
    {
        var pager = new Pager(new Text { Content = "Page 1" }, new Text { Content = "Page 2" })
        {
            InitialPage = 0!,
            Navigation = "wrap"!
        };

        await TestHelper.VerifySerializedObject(pager, AlexaJson, "Constructor");
    }
}
