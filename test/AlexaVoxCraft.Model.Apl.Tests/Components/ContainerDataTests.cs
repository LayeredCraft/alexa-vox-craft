using AlexaVoxCraft.Model.Apl.Components;

namespace AlexaVoxCraft.Model.Apl.Tests.Components;

/// <summary>
/// Coverage for <see cref="Container.Data"/> accepting either a raw literal data list or a
/// data-binding expression string. This is purely an authoring-time convenience for building the
/// outgoing document (the expression is evaluated by the Alexa APL renderer, not read back by the
/// skill), so — like every other component in this project — it's tested via serialize only, even
/// though `Container` itself already has pre-existing deserialize coverage for other properties in
/// ContainerTests.cs (that coverage predates this session's serialize-only convention and isn't
/// being changed here).
/// </summary>
public class ContainerDataTests : TestBase<ContainerDataTests>
{
    [Fact]
    public async Task Container_WithLiteralData_Serializes()
    {
        var container = new Container
        {
            Data = [new Dictionary<string, object> { { "test", "thing" } }]
        };

        await TestHelper.VerifySerializedObject(container, AlexaJson, "LiteralData");
    }

    [Fact]
    public async Task Container_WithDataBindingExpression_Serializes()
    {
        var container = new Container
        {
            Data = "$data.random.stuff"
        };

        await TestHelper.VerifySerializedObject(container, AlexaJson, "DataBindingExpression");
    }
}
