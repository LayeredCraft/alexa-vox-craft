using AlexaVoxCraft.Model.Request;

namespace AlexaVoxCraft.MediatR.Attributes;

public interface IAttributesManager
{
    Task<IDictionary<string, object>> GetRequestAttributes(CancellationToken cancellationToken = default);

    Task<IDictionary<string, object>> GetSessionAttributes(CancellationToken cancellationToken = default);

    Task<IDictionary<string, object>> GetPersistentAttributes(CancellationToken cancellationToken = default);

    Task SetRequestAttributes(IDictionary<string, object> requestAttributes, CancellationToken cancellationToken = default);

    Task SetSessionAttributes(IDictionary<string, object> sessionAttributes, CancellationToken cancellationToken = default);

    Task SetPersistentAttributes(IDictionary<string, object> persistentAttributes, CancellationToken cancellationToken = default);

    Task SavePersistentAttributes(CancellationToken cancellationToken = default);

    Task<Session> GetSession(CancellationToken cancellationToken = default);
}