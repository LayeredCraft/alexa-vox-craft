namespace AlexaVoxCraft.MediatR.Pipeline;

public interface IRequestInterceptor
{
    Task Process(IHandlerInput input, CancellationToken cancellationToken = default);
}