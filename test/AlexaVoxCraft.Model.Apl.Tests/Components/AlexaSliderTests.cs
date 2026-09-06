using AlexaVoxCraft.Model.Apl.Components;

namespace AlexaVoxCraft.Model.Apl.Tests.Components;

public class AlexaSliderTests : TestBase<AlexaSliderTests>
{
    [Fact]
    public async Task AlexaSlider_Serializes()
    {
        var control = new AlexaSlider
        {
            SliderId = "volumeSlider",
            MetadataPosition = MetadataPosition.AboveRight,
            SliderSize = SliderSize.Medium,
            SliderType = SliderType.Default,
            ProgressValue = 50,
            TotalValue = 100
        };

        await TestHelper.VerifySerializedObject(control, AlexaJson, "AlexaSlider");
    }
}
