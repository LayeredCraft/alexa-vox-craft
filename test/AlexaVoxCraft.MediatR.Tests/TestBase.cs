using AutoFixture;
using AutoFixture.Xunit3;
using NSubstitute;

namespace AlexaVoxCraft.MediatR.Tests;

/// <summary>
/// Base class for all MediatR tests providing common setup and utilities.
/// </summary>
public abstract class TestBase
{
    protected IFixture Fixture { get; }

    protected TestBase()
    {
        Fixture = new Fixture();
        Fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => Fixture.Behaviors.Remove(b));
        Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    /// <summary>
    /// Creates a substitute for the specified type.
    /// </summary>
    protected T CreateSubstitute<T>() where T : class => Substitute.For<T>();

    /// <summary>
    /// Creates a substitute for the specified type with constructor arguments.
    /// </summary>
    protected T CreateSubstitute<T>(params object[] constructorArguments) where T : class 
        => Substitute.For<T>(constructorArguments);
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
        return fixture;
    }
}