using AlexaVoxCraft.TestKit.SpecimenBuilders;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit3;

namespace AlexaVoxCraft.TestKit.Attributes;

public class MediatRAutoDataAttribute() : AutoDataAttribute(CreateFixture)
{
    private static IFixture CreateFixture()
    {
        var fixture = new Fixture();
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        
        // Add AutoNSubstitute customization for automatic interface mocking
        fixture.Customize(new AutoNSubstituteCustomization { ConfigureMembers = true });
        
        // Add specimen builders for SkillRequest and related types
        fixture.Customizations.Add(new RequestSpecimenBuilder());
        fixture.Customizations.Add(new SkillRequestSpecimenBuilder());
        fixture.Customizations.Add(new SkillRequestFactorySpecimenBuilder());
        return fixture;
    }
}