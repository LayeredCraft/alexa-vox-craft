using AlexaVoxCraft.Model.Request;

namespace AlexaVoxCraft.MediatR.Attributes.Persistence;

public interface IPersistenceAdapter
{
    Task<IDictionary<string, object>> GetAttributes(SkillRequest requestEnvelope,
        CancellationToken cancellationToken = default);

    Task SaveAttribute(SkillRequest requestEnvelope, IDictionary<string, object> attributes,
        CancellationToken cancellationToken = default);
}