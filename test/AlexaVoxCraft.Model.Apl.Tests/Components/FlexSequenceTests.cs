using AlexaVoxCraft.Model.Apl.Components;

namespace AlexaVoxCraft.Model.Apl.Tests.Components;

/// <summary>
/// Coverage for <see cref="FlexSequence"/>. Added alongside the CS0108 member-hiding fix
/// (Commit 16) for <c>RegisterTypeInfo&lt;T&gt;</c>, since it had no prior coverage.
/// </summary>
public sealed class FlexSequenceTests() : TestBase<FlexSequenceTests>
{
    [Fact]
    public async Task FlexSequence_Serializes_WithItems()
    {
        var flexSequence = new FlexSequence(new Text { Content = "One" }, new Text { Content = "Two" });

        await TestHelper.VerifySerializedObject(flexSequence, AlexaJson, "FlexSequence_WithItems");
    }
}
