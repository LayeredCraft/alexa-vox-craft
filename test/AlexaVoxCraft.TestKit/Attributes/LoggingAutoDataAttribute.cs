using AlexaVoxCraft.TestKit.SpecimenBuilders;

namespace AlexaVoxCraft.TestKit.Attributes;

/// <summary>
/// AutoData attribute that uses a customized fixture for consistent test data generation.
/// </summary>
public class LoggingAutoDataAttribute() : AutoDataAttribute(CreateFixture)
{
    private static IFixture CreateFixture()
    {
        return BaseFixtureFactory.CreateFixture(fixture =>
        {
            fixture.Customizations.Add(new LogEventSpecimenBuilder());
        });
    }
}