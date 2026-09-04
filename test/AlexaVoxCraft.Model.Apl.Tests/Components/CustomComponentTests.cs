using AlexaVoxCraft.Model.Apl.Components;

namespace AlexaVoxCraft.Model.Apl.Tests.Components;

/// <summary>
/// Coverage for <see cref="CustomComponent"/>, the escape hatch for a component type this SDK
/// doesn't have a dedicated class for. Only the serialize direction is covered — legacy's
/// <c>RandomClassTest</c> exercised the polymorphic-converter deserialize-fallback path instead,
/// which is a converter-infrastructure concern (not a specific component's own behavior) and is out
/// of scope here per the response-side-serialize rule.
/// </summary>
public class CustomComponentTests : TestBase<CustomComponentTests>
{
    [Fact]
    public async Task CustomComponent_Serializes()
    {
        var component = new CustomComponent("MyProprietaryWidget");

        await TestHelper.VerifySerializedObject(component, AlexaJson, "CustomComponent");
    }
}
