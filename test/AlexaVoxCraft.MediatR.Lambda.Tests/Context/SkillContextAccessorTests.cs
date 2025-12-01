using AlexaVoxCraft.MediatR.Lambda.Context;

namespace AlexaVoxCraft.MediatR.Lambda.Tests.Context;

public class SkillContextAccessorTests : TestBase
{
    [Fact]
    public void SkillContext_InitiallyNull()
    {
        var accessor = new SkillContextAccessor();

        accessor.SkillContext.Should().BeNull();
    }

    [Theory]
    [MediatRLambdaAutoData]
    public void SkillContext_SetAndGet_WorksCorrectly(SkillContextAccessor accessor, SkillContext context)
    {
        accessor.SkillContext = context;

        accessor.SkillContext.Should().Be(context);
    }

    [Theory]
    [MediatRLambdaAutoData]
    public void SkillContext_SetToNull_ClearsContext(SkillContextAccessor accessor, SkillContext context)
    {
        accessor.SkillContext = context;
        accessor.SkillContext = null;

        accessor.SkillContext.Should().BeNull();
    }

    [Theory]
    [MediatRLambdaAutoData]
    public void SkillContext_OverwriteExisting_UpdatesContext(SkillContextAccessor accessor, SkillContext firstContext,
        SkillContext secondContext)
    {
        accessor.SkillContext = firstContext;
        accessor.SkillContext = secondContext;

        accessor.SkillContext.Should().Be(secondContext);
        accessor.SkillContext.Should().NotBe(firstContext);
    }

    [Theory]
    [MediatRLambdaAutoData]
    public async Task SkillContext_ThreadSafety_IsolatesContextsBetweenTasks(SkillContextAccessor accessor,
        SkillContext context1, SkillContext context2)
    {
        var task1Result = Task.Run(() =>
        {
            accessor.SkillContext = context1;
            Thread.Sleep(100); // Allow other task to potentially interfere
            return accessor.SkillContext;
        });

        var task2Result = Task.Run(() =>
        {
            accessor.SkillContext = context2;
            Thread.Sleep(100); // Allow other task to potentially interfere
            return accessor.SkillContext;
        });

        var results = await Task.WhenAll(task1Result, task2Result);

        results[0].Should().Be(context1);
        results[1].Should().Be(context2);
    }

    [Theory]
    [MediatRLambdaAutoData]
    public async Task SkillContext_AsyncLocal_MaintainsContextAcrossAsyncOperations(SkillContextAccessor accessor,
        SkillContext context)
    {
        accessor.SkillContext = context;

        var asyncResult = await PerformAsyncOperation(accessor);

        asyncResult.Should().Be(context);
    }

    [Theory]
    [MediatRLambdaAutoData]
    public async Task SkillContext_NestedAsyncOperations_UpdatesContext(SkillContextAccessor accessor,
        SkillContext outerContext, SkillContext innerContext)
    {
        // Clear any existing context first
        accessor.SkillContext = null;

        // Set initial context
        accessor.SkillContext = outerContext;
        accessor.SkillContext.Should().Be(outerContext);

        // Perform the nested operation
        accessor.SkillContext = innerContext;
        accessor.SkillContext.Should().Be(innerContext);

        // After setting inner context, it should remain set
        await Task.Delay(10, TestContext.Current.CancellationToken);
        accessor.SkillContext.Should().Be(innerContext);
    }

    [Theory]
    [MediatRLambdaAutoData]
    public async Task SkillContext_ConcurrentAccess_IsolatesContexts(SkillContextAccessor accessor,
        SkillContext[] contexts)
    {
        var tasks = contexts.Select(async (context, index) =>
        {
            accessor.SkillContext = context;
            await Task.Delay(50); // Simulate async work
            return new { Index = index, Context = accessor.SkillContext };
        });

        var results = await Task.WhenAll(tasks);

        for (int i = 0; i < contexts.Length; i++)
        {
            var result = results.First(r => r.Index == i);
            result.Context.Should().Be(contexts[i]);
        }
    }

    [Theory]
    [MediatRLambdaAutoData]
    public void SkillContext_MultipleAccessors_ShareSameStorage(SkillContextAccessor accessor1,
        SkillContextAccessor accessor2, SkillContext context1, SkillContext context2)
    {
        // Multiple SkillContextAccessor instances share the same static AsyncLocal storage
        // This is by design for per-execution-context storage

        accessor1.SkillContext = context1;
        accessor2.SkillContext = context2;

        // Both accessors should have the last set value (context2)
        accessor1.SkillContext.Should().Be(context2);
        accessor2.SkillContext.Should().Be(context2);

        // Setting on one accessor affects the other
        accessor1.SkillContext = context1;
        accessor2.SkillContext.Should().Be(context1);
    }

    [Theory]
    [MediatRLambdaAutoData]
    public async Task SkillContext_ParentChildTasks_ShareContext(SkillContextAccessor accessor, SkillContext context)
    {
        accessor.SkillContext = context;

        var childTaskResult = await Task.Run(async () =>
        {
            // Child task should inherit parent's AsyncLocal context
            var childContext = accessor.SkillContext;
            await Task.Delay(10);
            return childContext;
        });

        childTaskResult.Should().Be(context);
    }

    private async Task<SkillContext?> PerformAsyncOperation(SkillContextAccessor accessor)
    {
        await Task.Delay(10);
        return accessor.SkillContext;
    }
}