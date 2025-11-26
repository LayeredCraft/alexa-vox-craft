using AlexaVoxCraft.MediatR.Lambda.Context;
using AlexaVoxCraft.TestKit.SpecimenBuilders;
using AutoFixture;
using AutoFixture.Kernel;
using AutoFixture.Xunit3;

namespace AlexaVoxCraft.TestKit.Attributes;

/// <summary>
/// AutoData attribute that uses a customized fixture for consistent test data generation.
/// </summary>
public class MediatRLambdaAutoDataAttribute() : AutoDataAttribute(CreateFixture)
{
    private static IFixture CreateFixture()
    {
        return BaseFixtureFactory.CreateFixture(fixture =>
        {
            // Add abstract class mappings
            fixture.Customizations.Add(new TypeRelay(typeof(SkillContext), typeof(DefaultSkillContext)));
            
            fixture.Customizations.Add(new LambdaContextSpecimenBuilder());
            fixture.Customizations.Add(new SkillRequestSpecimenBuilder());
            fixture.Customizations.Add(new SkillResponseSpecimenBuilder());
        });
    }
}