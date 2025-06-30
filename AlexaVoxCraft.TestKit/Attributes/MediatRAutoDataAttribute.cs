using AlexaVoxCraft.TestKit.SpecimenBuilders;
using AutoFixture;
using AutoFixture.Xunit3;

namespace AlexaVoxCraft.TestKit.Attributes;

public class MediatRAutoDataAttribute() : AutoDataAttribute(CreateFixture)
{
    private static IFixture CreateFixture()
    {
        return BaseFixtureFactory.CreateFixture(fixture =>
        {
            // Add specimen builders for SkillRequest and related types
            fixture.Customizations.Add(new SkillRequestSpecimenBuilder());
            fixture.Customizations.Add(new SkillRequestFactorySpecimenBuilder());
        });
    }
}