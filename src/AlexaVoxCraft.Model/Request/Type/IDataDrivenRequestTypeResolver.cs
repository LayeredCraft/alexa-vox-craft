using System.Text.Json;

namespace AlexaVoxCraft.Model.Request.Type;

public interface IDataDrivenRequestTypeResolver : IRequestTypeResolver
{
    System.Type? Resolve(JsonElement data);
}