using System.Text.Json;

namespace AlexaVoxCraft.Model.Response.Directive;

public interface IConnectionSendRequestHandler
{
    bool CanCreate(JsonElement data);

    Type Create();
}