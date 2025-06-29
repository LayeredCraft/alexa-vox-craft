using Amazon.Lambda.Core;

namespace AlexaVoxCraft.MediatR.Lambda.Abstractions;

public interface ILambdaHandler<in TRequest, TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, ILambdaContext context);
}