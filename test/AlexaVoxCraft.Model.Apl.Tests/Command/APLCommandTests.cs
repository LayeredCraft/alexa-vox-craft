using AlexaVoxCraft.Model.Apl.Commands;

namespace AlexaVoxCraft.Model.Apl.Tests.Command;

/// <summary>
/// Component-level coverage for the remaining <see cref="APLCommand"/> types not already covered by
/// <c>Directive/ExecuteCommandsDirectiveTests.cs</c> (<c>Sequential</c>/<c>SpeakItem</c>/<c>SpeakList</c>).
/// Commands are response objects the skill only ever builds and sends, so they are tested via
/// serialize, not deserialize (see docs/plans/0002-cloudwatch-driven-verify-tests-and-warning-cleanup.md).
/// </summary>
public sealed class APLCommandTests() : TestBase<APLCommandTests>
{
    [Fact]
    public async Task Idle_Serializes()
    {
        var command = new Idle();

        await TestHelper.VerifySerializedObject(command, AlexaJson, "Idle");
    }

    [Fact]
    public async Task AnimateItem_Serializes()
    {
        var command = new AnimateItem
        {
            Duration = 1000,
            RepeatCount = 9,
            RepeatMode = RepeatMode.Reverse,
            Value = AnimatedOpacity.Single(0, 1)
        };

        await TestHelper.VerifySerializedObject(command, AlexaJson, "AnimateItem");
    }

    [Fact]
    public async Task ControlMedia_Serializes()
    {
        var command = new ControlMedia
        {
            Command = ControlMediaCommand.Seek!,
            ComponentId = "myAudioPlayer",
            Value = 5000
        };

        await TestHelper.VerifySerializedObject(command, AlexaJson, "ControlMedia");
    }

    [Fact]
    public async Task SetValue_Serializes()
    {
        var command = new SetValue
        {
            ComponentId = "jokePunchline",
            Property = "opacity"!,
            Value = "1"!
        };

        await TestHelper.VerifySerializedObject(command, AlexaJson, "SetValue");
    }

    [Fact]
    public async Task Finish_Serializes()
    {
        var command = new Finish();

        await TestHelper.VerifySerializedObject(command, AlexaJson, "Finish");
    }

    [Fact]
    public async Task Reinflate_Serializes()
    {
        var command = new Reinflate { PreservedSequencers = ["test"] };

        await TestHelper.VerifySerializedObject(command, AlexaJson, "Reinflate");
    }

    [Fact]
    public async Task Select_Serializes()
    {
        var command = new Select
        {
            Commands = [new Finish()],
            Otherwise = [new Reinflate()]
        };

        await TestHelper.VerifySerializedObject(command, AlexaJson, "Select");
    }

    [Fact]
    public async Task InsertItem_Serializes()
    {
        var command = new InsertItem { At = 0, ComponentId = "myList", Items = ["itemValue"] };

        await TestHelper.VerifySerializedObject(command, AlexaJson, "InsertItem");
    }

    [Fact]
    public async Task RemoveItem_Serializes()
    {
        var command = new RemoveItem { ComponentId = "myListItem" };

        await TestHelper.VerifySerializedObject(command, AlexaJson, "RemoveItem");
    }

    [Fact]
    public async Task ScrollToComponent_Serializes()
    {
        var command = new ScrollToComponent { ComponentId = "myComponent", Align = ItemAlignment.Center, TargetDuration = 500 };

        await TestHelper.VerifySerializedObject(command, AlexaJson, "ScrollToComponent");
    }

    [Fact]
    public async Task SetPage_Serializes()
    {
        var command = new SetPage { ComponentId = "PagerId", Position = SetPagePosition.Relative, Value = 1! };

        await TestHelper.VerifySerializedObject(command, AlexaJson, "SetPage");
    }
}
