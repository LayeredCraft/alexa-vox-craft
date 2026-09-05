using AlexaVoxCraft.Model.Apl.Commands;

namespace AlexaVoxCraft.Model.Apl.Tests.Directive;

/// <summary>
/// Component-level coverage for <see cref="ExecuteCommandsDirective"/> serialization. This is a
/// response directive the skill only ever builds and sends, so it is tested via serialize, not
/// deserialize (see docs/plans/0002-cloudwatch-driven-verify-tests-and-warning-cleanup.md).
/// </summary>
public sealed class ExecuteCommandsDirectiveTests() : TestBase<ExecuteCommandsDirectiveTests>
{
    [Fact]
    public async Task ExecuteCommandsDirective_Serializes_WithRealisticCommandSequence()
    {
        var directive = new ExecuteCommandsDirective(
            "triviaPager",
            new Sequential(
                new SpeakItem { ComponentId = "page1" },
                new SpeakList { ComponentId = "list1"!, Start = 0!, Count = 3!}));

        await TestHelper.VerifySerializedObject(directive, AlexaJson, "ExecuteCommandsDirective_RealisticCommandSequence");
    }
}
