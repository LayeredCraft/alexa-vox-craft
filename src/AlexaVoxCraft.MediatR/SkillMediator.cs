using System.Collections.Concurrent;
using AlexaVoxCraft.MediatR.DI;
using AlexaVoxCraft.MediatR.Wrappers;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using LayeredCraft.StructuredLogging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AlexaVoxCraft.MediatR;

public class SkillMediator : ISkillMediator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly SkillServiceConfiguration _serviceConfiguration;
    private static readonly ConcurrentDictionary<Type, RequestHandlerWrapper> RequestHandlers = new();

    public SkillMediator(IServiceProvider serviceProvider, IOptions<SkillServiceConfiguration> serviceConfiguration)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _serviceConfiguration = serviceConfiguration.Value ?? throw new ArgumentNullException(nameof(serviceConfiguration));
    }

    public Task<SkillResponse> Send(SkillRequest request, CancellationToken cancellationToken = default)
    {
        var logger = _serviceProvider.GetRequiredService<ILogger<SkillMediator>>();
        
        using var _ = logger.TimeOperation("Handler resolution and execution");
        
        var skillId = request.Context.System.Application.ApplicationId;
        var requestType = request.Request.GetType().Name;
        
        logger.Debug("Mediating skill request {RequestType} for skill {SkillId}", requestType, skillId);
        
        // Skill ID verification
        if (string.IsNullOrWhiteSpace(_serviceConfiguration.SkillId) ||
            request.Context.System.Application.ApplicationId != _serviceConfiguration.SkillId)
        {
            logger.Error("Skill ID verification failed for {SkillId} (Expected: {ExpectedSkillId})", 
                skillId, _serviceConfiguration.SkillId);
            throw new ArgumentException("Skill ID verification failed!");
        }

        var requestTypeInternal = request.Request.GetType();

        var handler = RequestHandlers.GetOrAdd(requestTypeInternal,
            static t => (RequestHandlerWrapper)(Activator.CreateInstance(typeof(RequestHandlerWrapperImpl<>)
                .MakeGenericType(t)) ?? throw new InvalidOperationException($"Could not create wrapper type for {t}")));

        logger.Debug("Successfully resolved handler for {RequestType}", requestType);

        return handler.Handle(request, _serviceProvider, cancellationToken);
    }
}