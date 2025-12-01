using Amazon.Lambda.Core;

namespace AlexaVoxCraft.Lambda.Abstractions;

/// <summary>
/// Represents a delegate for handling AWS Lambda requests.
/// </summary>
/// <typeparam name="TRequest">The type of request to handle.</typeparam>
/// <typeparam name="TResponse">The type of response to return.</typeparam>
/// <param name="request">The incoming request.</param>
/// <param name="context">The Lambda execution context.</param>
/// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
/// <returns>A task that represents the asynchronous operation and contains the response.</returns>
public delegate Task<TResponse> HandlerDelegate<in TRequest, TResponse>(TRequest request, ILambdaContext context, CancellationToken cancellationToken);