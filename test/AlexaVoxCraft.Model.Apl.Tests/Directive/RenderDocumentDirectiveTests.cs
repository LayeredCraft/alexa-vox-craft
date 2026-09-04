using AlexaVoxCraft.Model.Apl.Components;

namespace AlexaVoxCraft.Model.Apl.Tests.Directive;

/// <summary>
/// Component-level coverage for <see cref="RenderDocumentDirective"/> serialization. This is a
/// response directive the skill only ever builds and sends, so it is tested via serialize, not
/// deserialize (see docs/plans/0002-cloudwatch-driven-verify-tests-and-warning-cleanup.md).
/// </summary>
public sealed class RenderDocumentDirectiveTests() : TestBase<RenderDocumentDirectiveTests>
{
    [Fact]
    public async Task RenderDocumentDirective_Serializes_WithRealisticToken()
    {
        var document = new APLDocument
        {
            MainTemplate = new Layout(new Text { Content = "Question text" })
        };
        var directive = new RenderDocumentDirective(document) { Token = "triviaPager" };

        await TestHelper.VerifySerializedObject(directive, AlexaJson, "RenderDocumentDirective_RealisticToken");
    }
}
