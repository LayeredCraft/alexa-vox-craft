using AlexaVoxCraft.TestKit.Customizations;
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
            fixture.Customizations.Add(new SkillServiceConfigurationSpecimenBuilder());
            fixture.Customizations.Add(new ServiceProviderSpecimenBuilder());
            fixture.Customizations.Add(new ServiceCollectionSpecimenBuilder());
            fixture.Customizations.Add(new OptionsSpecimenBuilder());
            fixture.Customizations.Add(new RequestHandlerDelegateSpecimenBuilder());
            
            // Register all ILogger<T> to use TestLogger<T> with Debug level
            fixture.Customize(new TestLoggerCustomization());
        });
    }
}