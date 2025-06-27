using AutoFixture;
using AutoFixture.Kernel;
using AutoFixture.Xunit3;
using AutoFixture.AutoNSubstitute;
using NSubstitute;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using System.Reflection;

namespace AlexaVoxCraft.MediatR.Tests;

/// <summary>
/// Base class for all MediatR tests providing common setup and utilities.
/// </summary>
public abstract class TestBase
{
    /// <summary>
    /// Creates a substitute for the specified type.
    /// </summary>
    protected T CreateSubstitute<T>() where T : class => Substitute.For<T>();
    
    /// <summary>
    /// Gets the current test cancellation token.
    /// </summary>
    protected CancellationToken CancellationToken => TestContext.Current.CancellationToken;
}

/// <summary>
/// AutoData attribute that uses a customized fixture for consistent test data generation.
/// </summary>
public class AutoDataAttribute : AutoFixture.Xunit3.AutoDataAttribute
{
    public AutoDataAttribute() : base(() => CreateFixture()) { }

    private static IFixture CreateFixture()
    {
        var fixture = new Fixture();
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        
        // Add AutoNSubstitute customization for automatic interface mocking
        fixture.Customize(new AutoNSubstituteCustomization { ConfigureMembers = true });
        
        // Add SkillRequest specimen builder
        fixture.Customizations.Add(new SkillRequestSpecimenBuilder());
        fixture.Customizations.Add(new SkillRequestFactorySpecimenBuilder());
        
        return fixture;
    }
}


/// <summary>
/// Custom specimen builder for SkillRequest - creates valid instances for testing.
/// </summary>
public class SkillRequestSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is not ParameterInfo parameter || !typeof(SkillRequest).IsAssignableFrom(parameter.ParameterType))
            return new NoSpecimen();

        // Create real objects instead of substitutes to avoid virtual member issues
        var application = new Application { ApplicationId = "amzn1.ask.skill.test-skill-id" };
        var system = new AlexaSystem { Application = application };
        var skillContext = new AlexaVoxCraft.Model.Request.Context { System = system };
        var session = new Session { Attributes = new Dictionary<string, object>() };
        
        var launchRequest = new LaunchRequest
        {
            Type = "LaunchRequest",
            RequestId = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            Locale = "en-US"
        };

        var skillRequest = new SkillRequest 
        { 
            Context = skillContext,
            Request = launchRequest,
            Session = session
        };
        
        return skillRequest;
    }
}

public class SkillRequestFactorySpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is not ParameterInfo parameter || !typeof(SkillRequestFactory).IsAssignableFrom(parameter.ParameterType))
            return new NoSpecimen();

        // Return a factory that creates a valid SkillRequest
        return Substitute.For<SkillRequestFactory>();
    }
}