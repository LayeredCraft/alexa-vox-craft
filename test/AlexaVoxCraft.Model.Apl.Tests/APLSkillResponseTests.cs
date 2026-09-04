using AlexaVoxCraft.Model.Apl.Commands;
using AlexaVoxCraft.Model.Apl.Components;
using AlexaVoxCraft.Model.Response;

namespace AlexaVoxCraft.Model.Apl.Tests;

/// <summary>
/// Envelope-level coverage for a full skill response carrying APL directives (this skill always
/// emits RenderDocument and ExecuteCommands together for a turn that renders a new page). Responses
/// are objects the skill only ever builds and sends, so they are tested via serialize, not
/// deserialize (see docs/plans/0002-cloudwatch-driven-verify-tests-and-warning-cleanup.md).
/// </summary>
public sealed class APLSkillResponseTests : TestBase<APLSkillResponseTests>
{
    [Fact]
    public async Task SkillResponse_Serializes_WithRenderDocumentAndExecuteCommands()
    {
        var document = new APLDocument
        {
            MainTemplate = new Layout(new Text { Content = "Question text" })
        };

        var response = new SkillResponse
        {
            Version = "1.0",
            Response = new ResponseBody
            {
                Directives =
                [
                    new RenderDocumentDirective(document) { Token = "triviaPager" },
                    new ExecuteCommandsDirective("triviaPager", new Sequential(new SpeakItem { ComponentId = "page1" }))
                ]
            }
        };

        await TestHelper.VerifySerializedObject(response, AlexaJson, "SkillResponse_WithRenderDocumentAndExecuteCommands");
    }
}
