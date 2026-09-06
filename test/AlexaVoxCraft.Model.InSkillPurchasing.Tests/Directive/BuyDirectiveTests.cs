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

    /// <summary>
    /// Response directives are serialize-only in this SDK (the skill builds and sends them; it
    /// never reads them back), so this verifies serialization using a product id and token shaped
    /// like a real captured Connections.SendRequest/Buy payload rather than round-tripping through
    /// deserialization.
    /// </summary>
    [Fact]
    public async Task BuyDirective_Serializes_WithRealisticProductId()
    {
        var directive = new BuyDirective("amzn1.adg.product.09ec74a2-745a-407f-bd4c-960bf5a97d0e", "8e8fb7f858ca45729aa822d43fdb585e");
        await TestHelper.VerifySerializedObject(directive, AlexaJson, "BuyDirective_RealisticProductId");
    }
}