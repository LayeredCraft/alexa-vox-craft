using AlexaVoxCraft.Model.InSkillPurchasing.Directives;

namespace AlexaVoxCraft.Model.InSkillPurchasing.Tests.Directive;

public class BuyDirectiveTests : TestBase<BuyDirectiveTests>
{
    [Fact]
    public async Task BuyDirective_Serializes()
    {
        var directive = new BuyDirective("amzn1.adg.product", "correlationToken");
        await TestHelper.VerifySerializedObject(directive, AlexaJson, "BuyDirective");
    }
}