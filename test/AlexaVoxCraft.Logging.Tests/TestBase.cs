using NSubstitute;

namespace AlexaVoxCraft.Logging.Tests;

/// <summary>
/// Base class for all Logging tests providing common setup and utilities.
/// </summary>
public abstract class TestBase
{

    /// <summary>
    /// Creates a substitute for the specified type.
    /// </summary>
    protected T CreateSubstitute<T>() where T : class => Substitute.For<T>();
}