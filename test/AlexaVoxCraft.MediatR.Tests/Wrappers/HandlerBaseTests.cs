using AlexaVoxCraft.MediatR.Wrappers;
using AlexaVoxCraft.TestKit.Attributes;
using AwesomeAssertions;
using AutoFixture.Xunit3;
using Microsoft.Extensions.DependencyInjection;

namespace AlexaVoxCraft.MediatR.Tests.Wrappers;

public class HandlerBaseTests : TestBase
{
    // Test implementation to access protected method
    private class TestHandlerBase : HandlerBase
    {
        public static IEnumerable<THandler> TestGetHandlers<THandler>(IServiceProvider serviceProvider)
        {
            return GetHandlers<THandler>(serviceProvider);
        }
    }

    [Theory]
    [MediatRAutoData]
    public void GetHandlers_WithRegisteredServices_ReturnsHandlers(
        [Frozen] IServiceCollection services,
        ITestHandler handler1,
        ITestHandler handler2)
    {
        // Arrange
        services.AddSingleton(handler1);
        services.AddSingleton(handler2);
        
        var serviceProvider = services.BuildServiceProvider();
        
        // Act
        var result = TestHandlerBase.TestGetHandlers<ITestHandler>(serviceProvider);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().Contain(handler1);
        result.Should().Contain(handler2);
    }

    [Theory]
    [MediatRAutoData]
    public void GetHandlers_WithNoRegisteredServices_ReturnsEmptyCollection(
        [Frozen] IServiceCollection services)
    {
        // Arrange - No handlers registered
        var serviceProvider = services.BuildServiceProvider();
        
        // Act
        var result = TestHandlerBase.TestGetHandlers<ITestHandler>(serviceProvider);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Theory]
    [MediatRAutoData]
    public void GetHandlers_WithServiceConstructionException_ThrowsInvalidOperationException(
        [Frozen] IServiceCollection services)
    {
        // Arrange
        services.AddSingleton<ITestHandler, FailingTestHandler>();
        
        var serviceProvider = services.BuildServiceProvider();
        
        // Act & Assert
        var exception = Record.Exception(() => 
            TestHandlerBase.TestGetHandlers<ITestHandler>(serviceProvider).ToList());
        
        exception.Should().BeOfType<InvalidOperationException>();
        exception.Message.Should().Contain("Error constructing handler for request of type");
        exception.InnerException.Should().NotBeNull();
    }

    // Test interfaces and implementations
    public interface ITestHandler
    {
        void Handle();
    }

    public class FailingTestHandler : ITestHandler
    {
        public FailingTestHandler()
        {
            throw new InvalidOperationException("Construction failed");
        }

        public void Handle()
        {
            // Implementation not needed for this test
        }
    }
}