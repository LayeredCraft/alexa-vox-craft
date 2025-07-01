using AlexaVoxCraft.TestKit.SpecimenBuilders;
using AutoFixture;
using AutoFixture.Xunit3;

namespace AlexaVoxCraft.TestKit.Attributes;

public class ModelAutoDataAttribute() : AutoDataAttribute(CreateFixture)
{
    private static IFixture CreateFixture()
    {
        var fixture = new Fixture();
        
        // Add specimen builders for Model types
        fixture.Customizations.Add(new CardSpecimenBuilder());
        fixture.Customizations.Add(new DialogDirectiveSpecimenBuilder());
        
        // Register URI generator for card images
        fixture.Register(() => new Uri($"https://example.com/{Guid.NewGuid()}"));
        
        return fixture;
    }
}