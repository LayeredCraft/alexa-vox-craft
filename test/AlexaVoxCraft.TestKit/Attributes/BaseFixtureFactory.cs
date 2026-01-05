using AutoFixture;
using AutoFixture.AutoNSubstitute;

namespace AlexaVoxCraft.TestKit.Attributes;

/// <summary>
/// Base class that provides common fixture configuration for AutoData attributes.
/// </summary>
public static class BaseFixtureFactory
{
    /// <summary>
    /// Creates a fixture with common configuration and allows custom specimen builders to be added.
    /// </summary>
    /// <param name="customizeAction">Action to customize the fixture with specific specimen builders</param>
    /// <returns>Configured fixture</returns>
    public static IFixture CreateFixture(Action<IFixture> customizeAction)
    {
        var fixture = new Fixture();
        
        // Remove throwing recursion behavior and add omit on recursion behavior
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        
        // Add AutoNSubstitute customization for automatic interface mocking
        fixture.Customize(new AutoNSubstituteCustomization { ConfigureMembers = true });

        // Allow customization with specific specimen builders
        customizeAction(fixture);
        
        return fixture;
    }
}