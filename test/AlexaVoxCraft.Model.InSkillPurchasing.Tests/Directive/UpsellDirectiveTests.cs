using AlexaVoxCraft.Model.InSkillPurchasing.Directives;

namespace AlexaVoxCraft.Model.InSkillPurchasing.Tests.Directive;

public class UpsellDirectiveTests : TestBase<UpsellDirectiveTests>
{
    [Fact]
    public async Task UpsellDirective_Serializes()
    {
        var directive = new UpsellDirective("amzn1.adg.product", "correlationToken");
        await TestHelper.VerifySerializedObject(directive, AlexaJson, "UpsellDirective");
    }
}