using AlexaVoxCraft.Model.Apl.Components;

namespace AlexaVoxCraft.Model.Apl.Tests.Components;

public class AlexaTextListItemTests : TestBase<AlexaTextListItemTests>
{
    [Fact]
    public async Task AlexaTextListItem_WithComponentSlot_Serializes()
    {
        var control = new AlexaTextListItem
        {
            ComponentSlot = new APLValue<APLComponent>(new Container())
        };

        await TestHelper.VerifySerializedObject(control, AlexaJson, "AlexaTextListItem_WithComponentSlot");
    }
}
