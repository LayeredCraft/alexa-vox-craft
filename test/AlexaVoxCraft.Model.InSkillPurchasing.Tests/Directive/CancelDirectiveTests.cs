using AlexaVoxCraft.Model.InSkillPurchasing.Directives;

namespace AlexaVoxCraft.Model.InSkillPurchasing.Tests.Directive;

public class CancelDirectiveTests : TestBase<CancelDirectiveTests>
{
    [Fact]
    public async Task CancelDirective_Serializes()
    {
        var directive = new CancelDirective("amzn1.adg.product", "correlationToken");
        await TestHelper.VerifySerializedObject(directive, AlexaJson, "CancelDirective");
    }
}