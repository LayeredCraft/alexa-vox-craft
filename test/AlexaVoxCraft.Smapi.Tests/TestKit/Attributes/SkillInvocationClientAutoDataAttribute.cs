using AlexaVoxCraft.Http.TestKit.Attributes;
using AlexaVoxCraft.Smapi.Tests.TestKit.SpecimenBuilders;

namespace AlexaVoxCraft.Smapi.Tests.TestKit.Attributes;

public sealed class SkillInvocationClientAutoDataAttribute() : ClientAutoDataAttribute(CreateFixture)
{
    internal static IFixture CreateFixture()
    {
        return CreateBaseFixture(fixture =>
        {
            fixture.Customizations.Add(new JsonElementSpecimenBuilder());
            fixture.Customizations.Add(new SkillRequestSpecimenBuilder());
            fixture.Customizations.Add(new SkillInvocationResponseSpecimenBuilder());
        });
    }
}

public sealed class InlineSkillInvocationClientAutoDataAttribute(params object?[] values)
    : InlineAutoDataAttribute(SkillInvocationClientAutoDataAttribute.CreateFixture, values);
