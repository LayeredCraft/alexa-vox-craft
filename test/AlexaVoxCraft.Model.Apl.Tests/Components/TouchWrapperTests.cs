using AlexaVoxCraft.Model.Apl.Components;

namespace AlexaVoxCraft.Model.Apl.Tests.Components;

/// <summary>
/// Coverage for <see cref="TouchWrapper"/> (and, since it's abstract and can only be exercised
/// through a concrete subclass, its base <see cref="TouchComponent"/>). Added alongside the
/// CS0108 member-hiding fix (Commit 16) for <c>RegisterTypeInfo&lt;T&gt;</c> in this class's
/// registration chain, since neither had any prior coverage.
/// </summary>
public sealed class TouchWrapperTests() : TestBase<TouchWrapperTests>
{
    [Fact]
    public async Task TouchWrapper_Serializes_WithItem()
    {
        var wrapper = new TouchWrapper(new Text { Content = "Tap me" });

        await TestHelper.VerifySerializedObject(wrapper, AlexaJson, "TouchWrapper_WithItem");
    }
}
