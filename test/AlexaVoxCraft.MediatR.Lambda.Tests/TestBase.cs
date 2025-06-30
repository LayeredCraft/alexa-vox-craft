using NSubstitute;

namespace AlexaVoxCraft.MediatR.Lambda.Tests;

/// <summary>
/// Base class for all Lambda tests providing common setup and utilities.
/// </summary>
public abstract class TestBase
{ 
    protected T CreateSubstitute<T>() where T : class => Substitute.For<T>();
}