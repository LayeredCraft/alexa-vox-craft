using AlexaVoxCraft.Model.Apl.Components;

namespace AlexaVoxCraft.Model.Apl.Tests.Components;

public class GridSequenceTests : TestBase<GridSequenceTests>
{
    [Fact]
    public async Task GridSequence_Serializes()
    {
        var sequence = new GridSequence
        {
            ScrollDirection = ScrollDirection.Horizontal,
            Snap = Snap.Center,
            AllowForward = true,
            AllowBackwards = false,
            ChildWidths = ["100dp"],
            Items = [new Text { Content = "Item 1" }, new Text { Content = "Item 2" }]
        };

        await TestHelper.VerifySerializedObject(sequence, AlexaJson, "GridSequence");
    }
}
