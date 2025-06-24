using AutoFixture;
using AutoFixture.Xunit3;
using NSubstitute;
using Amazon.Lambda.Core;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;

namespace AlexaVoxCraft.MediatR.Lambda.Tests;

/// <summary>
/// Base class for all Lambda tests providing common setup and utilities.
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
    /// Creates a mock Lambda context for testing.
    /// </summary>
    protected ILambdaContext CreateMockLambdaContext(
        string requestId = "test-request-id",
        string functionName = "test-function",
        TimeSpan? remainingTime = null)
    {
        var context = CreateSubstitute<ILambdaContext>();
        context.AwsRequestId.Returns(requestId);
        context.FunctionName.Returns(functionName);
        context.RemainingTime.Returns(remainingTime ?? TimeSpan.FromMinutes(5));
        return context;
    }

    /// <summary>
    /// Creates a test SkillRequest with minimal required data.
    /// </summary>
    protected SkillRequest CreateTestSkillRequest()
    {
        return Fixture.Build<SkillRequest>()
            .With(x => x.Context, Fixture.Build<Model.Request.Context>()
                .With(c => c.System, Fixture.Build<AlexaSystem>()
                    .With(s => s.Application, Fixture.Build<Application>()
                        .With(a => a.ApplicationId, "amzn1.ask.skill.test")
                        .Create())
                    .Create())
                .Create())
            .Create();
    }
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