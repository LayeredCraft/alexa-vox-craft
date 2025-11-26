using AlexaVoxCraft.MediatR.Attributes.Persistence;
using AlexaVoxCraft.Model.Request;

namespace Sample.Generated.Function;

public sealed class DynamoDbTriviaAdapter : IPersistenceAdapter
{
    public Task<IDictionary<string, object>> GetAttributes(SkillRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task SaveAttribute(SkillRequest request, IDictionary<string, object> attributes, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}