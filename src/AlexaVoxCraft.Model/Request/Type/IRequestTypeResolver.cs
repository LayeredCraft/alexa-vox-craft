namespace AlexaVoxCraft.Model.Request.Type;

public interface IRequestTypeResolver
{
    bool CanResolve(string requestType);
    System.Type? Resolve(string requestType);
}