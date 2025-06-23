using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.MediatR.Lambda.Abstractions;
using AlexaVoxCraft.Model.Apl;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using Amazon.Lambda.Core;
using LayeredCraft.StructuredLogging;
using Microsoft.Extensions.Logging;

namespace Sample.Apl.Function;

public class LambdaHandler : ILambdaHandler<APLSkillRequest, SkillResponse>
{
    private readonly ISkillMediator _mediator;
    private readonly ILogger<LambdaHandler> _logger;

    public LambdaHandler(ISkillMediator mediator, ILogger<LambdaHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<SkillResponse> HandleAsync(APLSkillRequest request, ILambdaContext context)
    {
        _logger.Debug("Received request of type {RequestType}", propertyValue: request.Request.GetType().Name);
        if (request.Request is IntentRequest intent)
        {
            _logger.Debug("Received intent {IntentType}", propertyValue: intent.Intent.Name);
        }

        try
        {
            var response = await _mediator.Send(request);
            return response;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error handling request");
            throw;
        }
    }
}