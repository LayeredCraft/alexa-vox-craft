using LayeredCraft.Logging.CompactJsonFormatter;
using AlexaVoxCraft.MediatR.Lambda.Serialization;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace AlexaVoxCraft.MediatR.Lambda;

public static class LambdaHostExtensions
{
    public static async Task<int> RunAlexaSkill<T, TRequest, TResponse>(
        Func<T, IServiceProvider, Func<TRequest, ILambdaContext, Task<TResponse>>>? handlerBuilder = null,
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

            var handler = handlerBuilder?.Invoke(function, services) ?? function.FunctionHandlerAsync;

            var serializer = serializerFactory?.Invoke(services)
                             ?? services.GetRequiredService<ILambdaSerializer>();

            await LambdaBootstrapBuilder.Create(handler, serializer)
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