using AlexaVoxCraft.Model.Request.Type;

namespace AlexaVoxCraft.Model.WebApiGames.RequestHandlers;

internal sealed class MessageRequestTypeResolver : IRequestTypeResolver
{
    public bool CanResolve(string requestType) => requestType == MessageRequest.RequestType;

    public System.Type? Resolve(string requestType) => typeof(MessageRequest);
}
