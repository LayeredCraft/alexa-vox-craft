using AlexaVoxCraft.MediatR.Lambda.Abstractions;
using AlexaVoxCraft.MediatR.Lambda.Context;
using AlexaVoxCraft.MediatR.Lambda.Extensions;
using AlexaVoxCraft.MediatR.Lambda.Serialization;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Serialization;
using Amazon.Lambda.Core;
using LayeredCraft.StructuredLogging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace AlexaVoxCraft.MediatR.Lambda;

/// <summary>
/// Base class for AWS Lambda functions that host Alexa skills using the AlexaVoxCraft framework.
/// This class provides the foundational infrastructure for Lambda-hosted skills including
/// service configuration, dependency injection, logging, and request processing.
/// </summary>
/// <typeparam name="TRequest">The type of Alexa skill request. Must inherit from <see cref="SkillRequest"/>.</typeparam>
/// <typeparam name="TResponse">The type of Alexa skill response. Must inherit from <see cref="SkillResponse"/>.</typeparam>
public abstract class AlexaSkillFunction<TRequest, TResponse>
    where TRequest : SkillRequest where TResponse : SkillResponse
{
    private IServiceProvider _serviceProvider = null!;

    /// <summary>
    /// Gets the service provider used for dependency injection within the Lambda function.
    /// </summary>
    /// <value>The configured service provider instance.</value>
    public IServiceProvider ServiceProvider => _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="AlexaSkillFunction{TRequest, TResponse}"/> class.
    /// The constructor automatically starts the host and configures services.
    /// </summary>
    protected AlexaSkillFunction()
    {
        Start();
    }

    /// <summary>
    /// Creates and configures the host builder with default logging and Serilog configuration.
    /// Override this method to customize the hosting configuration.
    /// </summary>
    /// <returns>A configured host builder instance.</returns>
    protected virtual IHostBuilder CreateHostBuilder()
    {
        var builder = Host.CreateDefaultBuilder().ConfigureLogging((_, logging) =>
        {
            logging.AddJsonConsole();
            logging.AddDebug();
        }).UseSerilog((context, services, configuration) =>
        {
            configuration.ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Destructure.With(new SystemTextDestructuringPolicy())
                .Enrich.FromLogContext()
                .Enrich.WithActivityId();
        });
        Init(builder);
        return builder;
    }

    /// <summary>
    /// Override this method to configure additional services and settings for the skill.
    /// This is called during host builder creation and is the primary extension point
    /// for skill-specific configuration.
    /// </summary>
    /// <param name="builder">The host builder to configure.</param>
    protected virtual void Init(IHostBuilder builder)
    {
    }

    /// <summary>
    /// Starts the host and initializes the service provider with core AlexaVoxCraft services.
    /// </summary>
    protected void Start()
    {
        var builder = CreateHostBuilder();
        builder.ConfigureServices(services =>
        {
            services.AddSkillContextAccessor();
            services.AddSingleton(AlexaJsonOptions.DefaultOptions);
            services.AddSingleton<ILambdaSerializer, AlexaLambdaSerializer>();
        });
        var host = builder.Build();

        host.Start();
        _serviceProvider = host.Services;
    }

    /// <summary>
    /// Creates and sets the skill context for the current request.
    /// </summary>
    /// <param name="request">The incoming Alexa skill request.</param>
    protected void CreateContext(TRequest request)
    {
        var factory = _serviceProvider.GetRequiredService<ISkillContextFactory>();
        factory.Create(request);
    }

    /// <summary>
    /// The main entry point for Lambda function execution. This method handles the complete
    /// request processing lifecycle including context creation, handler execution, and response generation.
    /// </summary>
    /// <param name="request">The incoming Alexa skill request.</param>
    /// <param name="lambdaContext">The AWS Lambda execution context.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the skill response.</returns>
    public virtual async Task<TResponse> FunctionHandlerAsync(TRequest request, ILambdaContext lambdaContext)
    {
        using var serviceScope = _serviceProvider.CreateScope();
        var provider = serviceScope.ServiceProvider;
        var logger = provider.GetRequiredService<ILogger<AlexaSkillFunction<TRequest, TResponse>>>();
        
        var applicationId = request.Context.System.Application.ApplicationId;
        var requestId = lambdaContext.AwsRequestId;
        var remainingTime = lambdaContext.RemainingTime;
        
        using var scope = logger.BeginScope<string, string, string, TimeSpan>(
            "ApplicationId", applicationId,
            "LambdaRequestId", requestId,
            "FunctionName", lambdaContext.FunctionName,
            "RemainingTime", remainingTime);

        logger.Information("Lambda execution started for skill {ApplicationId}", applicationId);

        using var timer = logger.TimeOperation("Lambda handler execution");

        try
        {
            CreateContext(request);
            var handlerAsync = provider.GetRequiredService<HandlerDelegate<TRequest, TResponse>>();
            var response = await handlerAsync(request, lambdaContext);
            
            logger.Information("Lambda execution completed successfully for skill {ApplicationId}", applicationId);
            return response;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Lambda execution failed for skill {ApplicationId}", applicationId);
            throw;
        }
    }
}