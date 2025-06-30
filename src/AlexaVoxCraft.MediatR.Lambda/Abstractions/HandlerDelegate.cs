using Amazon.Lambda.Core;

namespace AlexaVoxCraft.MediatR.Lambda.Abstractions;

public delegate Task<TResponse> HandlerDelegate<in TRequest, TResponse>(TRequest request, ILambdaContext context);