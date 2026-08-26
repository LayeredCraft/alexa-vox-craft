namespace AlexaVoxCraft.MediatR.Tests;

/// <summary>
/// Base class for all MediatR tests providing common setup and utilities.
/// </summary>
public abstract class TestBase
{
    /// <summary>
    /// Gets the current test cancellation token.
    /// </summary>
    protected CancellationToken CancellationToken => TestContext.Current.CancellationToken;
}