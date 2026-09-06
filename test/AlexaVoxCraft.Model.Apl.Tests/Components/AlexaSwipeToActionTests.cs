using AlexaVoxCraft.Model.Apl.Components;

namespace AlexaVoxCraft.Model.Apl.Tests.Components;

public class AlexaSwipeToActionTests : TestBase<AlexaSwipeToActionTests>
{
    [Fact]
    public async Task AlexaSwipeToAction_Serializes()
    {
        var control = new AlexaSwipeToAction
        {
            PrimaryText = "Swipe me",
            SecondaryText = "Details",
            Button1Text = "Archive",
            Button2Text = "Delete",
            Direction = "right",
            Theme = "dark"
        };

        await TestHelper.VerifySerializedObject(control, AlexaJson, "AlexaSwipeToAction");
    }
}
