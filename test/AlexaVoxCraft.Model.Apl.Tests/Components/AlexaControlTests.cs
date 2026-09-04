using AlexaVoxCraft.Model.Apl.Commands;
using AlexaVoxCraft.Model.Apl.Components;

namespace AlexaVoxCraft.Model.Apl.Tests.Components;

/// <summary>
/// Component-level coverage for the smaller single-control "Alexa*" design components.
/// </summary>
public class AlexaControlTests : TestBase<AlexaControlTests>
{
    [Fact]
    public async Task AlexaIconButton_Serializes()
    {
        var control = new AlexaIconButton
        {
            ButtonSize = new AbsoluteDimension(72, "dp"),
            VectorSource = "M21.343,8.661l-7.895-7.105c-0.823-0.741-2.073-0.741-2.896,0",
            PrimaryAction = [new SetValue { ComponentId = "textToUpdate", Property = "text", Value = APLValue.To<string>("${exampleData.imageStyleText}") }]
        };

        await TestHelper.VerifySerializedObject(control, AlexaJson, "AlexaIconButton");
    }

    [Fact]
    public async Task AlexaRating_Serializes()
    {
        var control = new AlexaRating
        {
            RatingSlotPadding = new AbsoluteDimension(0, "dp"),
            RatingSlotMode = RatingSlotMode.Multiple,
            RatingNumber = 3.5,
            RatingText = "509 ratings",
            Spacing = "@spacingMedium"
        };

        await TestHelper.VerifySerializedObject(control, AlexaJson, "AlexaRating");
    }

    [Fact]
    public async Task AlexaProgressDots_Serializes()
    {
        var control = new AlexaProgressDots { ComponentId = "dots", DotSize = new AbsoluteDimension(8, "dp"), Theme = "dark" };

        await TestHelper.VerifySerializedObject(control, AlexaJson, "AlexaProgressDots");
    }

    [Fact]
    public async Task AlexaProgressBar_Serializes()
    {
        var control = new AlexaProgressBar { ProgressValue = 50, TotalValue = 100, Theme = "dark" };

        await TestHelper.VerifySerializedObject(control, AlexaJson, "AlexaProgressBar");
    }

    [Fact]
    public async Task AlexaRadioButton_Serializes()
    {
        var control = new AlexaRadioButton { Theme = "dark", RadioButtonColor = "@colorAccent", RadioButtonHeight = new AbsoluteDimension(24, "dp") };

        await TestHelper.VerifySerializedObject(control, AlexaJson, "AlexaRadioButton");
    }

    [Fact]
    public async Task AlexaCheckbox_Serializes()
    {
        var control = new AlexaCheckbox { Theme = "dark", SelectedColor = "@colorAccent", CheckboxHeight = new AbsoluteDimension(24, "dp") };

        await TestHelper.VerifySerializedObject(control, AlexaJson, "AlexaCheckbox");
    }

    [Fact]
    public async Task AlexaSwitch_Serializes()
    {
        var control = new AlexaSwitch { Theme = "dark", ActiveColor = "@colorAccent", SwitchHeight = new AbsoluteDimension(24, "dp") };

        await TestHelper.VerifySerializedObject(control, AlexaJson, "AlexaSwitch");
    }

    [Fact]
    public async Task AlexaIcon_Serializes()
    {
        var control = new AlexaIcon { IconName = "closedCaptioning", IconSize = new AbsoluteDimension(24, "dp"), IconColor = "@colorText" };

        await TestHelper.VerifySerializedObject(control, AlexaJson, "AlexaIcon");
    }
}
