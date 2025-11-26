using AlexaVoxCraft.MediatR.Attributes.Persistence;
using AlexaVoxCraft.Model.Request;

namespace Sample.Generated.Function;

public class DynamoDbPersistenceAdapter : IPersistenceAdapter
{
    public Task<IDictionary<string, object>> GetAttributes(SkillRequest requestEnvelope, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task SaveAttribute(SkillRequest requestEnvelope, IDictionary<string, object> attributes, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}