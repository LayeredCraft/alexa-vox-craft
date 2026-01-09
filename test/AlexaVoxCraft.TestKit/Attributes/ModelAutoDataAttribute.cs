using AlexaVoxCraft.TestKit.SpecimenBuilders;

namespace AlexaVoxCraft.TestKit.Attributes;

public class ModelAutoDataAttribute() : AutoDataAttribute(CreateFixture)
{
    private static IFixture CreateFixture()
    {
        var fixture = new Fixture();
        
        // Configure seeded RNG for deterministic tests
        // Use environment variable AUTOFIXTURE_SEED for CI reproducibility, fallback to fixed seed
        var seed = Environment.GetEnvironmentVariable("AUTOFIXTURE_SEED") != null 
            ? int.Parse(Environment.GetEnvironmentVariable("AUTOFIXTURE_SEED")!) 
            : 12345; // Fixed seed for local development
        fixture.Register(() => new Random(seed));
        
        // Add specimen builders for Model types
        fixture.Customizations.Add(new CardSpecimenBuilder());
        fixture.Customizations.Add(new DialogDirectiveSpecimenBuilder());
        fixture.Customizations.Add(new SsmlSpecimenBuilder());
        fixture.Customizations.Add(new AudioPlayerDirectiveSpecimenBuilder());
        fixture.Customizations.Add(new VideoAppDirectiveSpecimenBuilder());
        
        // Register URI generator for card images
        fixture.Register(() => new Uri($"https://example.com/{Guid.NewGuid()}"));
        
        return fixture;
    }
}