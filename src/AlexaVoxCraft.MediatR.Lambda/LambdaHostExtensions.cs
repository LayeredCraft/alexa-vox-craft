using LayeredCraft.Logging.CompactJsonFormatter;
using AlexaVoxCraft.MediatR.DI;
using AlexaVoxCraft.MediatR.Lambda.Serialization;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;

namespace AlexaVoxCraft.MediatR.Lambda;

public static class LambdaHostExtensions
{

    public static async Task<int> RunAlexaSkill<T, TRequest, TResponse>(
        Func<T, IServiceProvider, Func<TRequest, ILambdaContext, CancellationToken, Task<TResponse>>>? handlerBuilder = null,
        Func<IServiceProvider, ILambdaSerializer>? serializerFactory = null
    ) where T : AlexaSkillFunction<TRequest, TResponse>, new()
        where TRequest : SkillRequest
        where TResponse : SkillResponse
    {
        try
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console(new CompactJsonFormatter())
                .Destructure.With(new SystemTextDestructuringPolicy())
                .CreateBootstrapLogger();

            Log.Information("🚀 Starting Lambda Host");

            var function = new T();
            var services = function.ServiceProvider;

            // Build the token-aware handler (caller may supply one, otherwise use the function's)
            Func<TRequest, ILambdaContext, CancellationToken, Task<TResponse>> handler =
                handlerBuilder?.Invoke(function, services)
                ?? function.FunctionHandlerAsync;

            // Lambda bootstrap still expects a 2-arg handler; wrap it to inject the token.
            async Task<TResponse> BootstrapHandler(TRequest req, ILambdaContext ctx)
            {
                // Get timeout buffer from configuration
                var bufferMs = services.GetRequiredService<IOptions<SkillServiceConfiguration>>().Value.CancellationTimeoutBufferMilliseconds;
                var buffer = TimeSpan.FromMilliseconds(bufferMs);
                var timeLeft = ctx.RemainingTime > buffer ? ctx.RemainingTime - buffer : TimeSpan.Zero;

                using var cts = new CancellationTokenSource(timeLeft);

                try
                {
                    return await handler(req, ctx, cts.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (cts.IsCancellationRequested)
                {
                    // graceful: let the runtime end the invocation
                    throw;
                }
            }

            var serializer = serializerFactory?.Invoke(services)
                             ?? services.GetRequiredService<ILambdaSerializer>();

            var bootstrapDelegate = BootstrapHandler;
            await LambdaBootstrapBuilder.Create(bootstrapDelegate, serializer)
                .Build()
                .RunAsync();

            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "💥 Host terminated unexpectedly");
            return 1;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}