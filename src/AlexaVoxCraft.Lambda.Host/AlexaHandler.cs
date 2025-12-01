using AlexaVoxCraft.Lambda.Abstractions;
using Amazon.Lambda.Core;
using AwsLambda.Host.Builder;

namespace AlexaVoxCraft.Lambda.Host;

/// <summary>
/// Provides the Lambda handler entry point for Alexa skill requests using AWS Lambda Host framework.
/// </summary>
public static class AlexaHandler
{
    /// <summary>
    /// Invokes the Alexa skill handler to process a request.
    /// </summary>
    /// <typeparam name="TRequest">The type of request to process.</typeparam>
    /// <typeparam name="TResponse">The type of response to return.</typeparam>
    /// <param name="skillRequest">The incoming Alexa skill request.</param>
    /// <param name="handler">The handler delegate to process the request.</param>
    /// <param name="context">The Lambda execution context.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation and contains the skill response.</returns>
    public static Task<TResponse> Invoke<TRequest, TResponse>(
        [Event] TRequest skillRequest,
        HandlerDelegate<TRequest, TResponse> handler,
        ILambdaContext context,
        CancellationToken cancellationToken) => handler(skillRequest, context, cancellationToken);
}