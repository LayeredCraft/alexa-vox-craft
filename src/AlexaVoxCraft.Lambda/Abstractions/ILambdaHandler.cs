using Amazon.Lambda.Core;

namespace AlexaVoxCraft.Lambda.Abstractions;

/// <summary>
/// Defines a handler for processing AWS Lambda requests.
/// </summary>
/// <typeparam name="TRequest">The type of request to handle.</typeparam>
/// <typeparam name="TResponse">The type of response to return.</typeparam>
public interface ILambdaHandler<in TRequest, TResponse>
{
    /// <summary>
    /// Handles an incoming Lambda request asynchronously.
    /// </summary>
    /// <param name="request">The incoming request.</param>
    /// <param name="context">The Lambda execution context.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>The response to return to the Lambda runtime.</returns>
    Task<TResponse> HandleAsync(TRequest request, ILambdaContext context, CancellationToken cancellationToken);
}