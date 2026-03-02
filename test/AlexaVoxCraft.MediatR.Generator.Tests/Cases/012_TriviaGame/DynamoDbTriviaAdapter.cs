using System.Text.Json;
using AlexaVoxCraft.MediatR.Attributes.Persistence;
using AlexaVoxCraft.Model.Request;

namespace Sample.Generated.Function;

public sealed class DynamoDbTriviaAdapter : IPersistenceAdapter
{
    public Task<IDictionary<string, JsonElement>> GetAttributes(SkillRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task SaveAttribute(SkillRequest request, IDictionary<string, JsonElement> attributes, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}