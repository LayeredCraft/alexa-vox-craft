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