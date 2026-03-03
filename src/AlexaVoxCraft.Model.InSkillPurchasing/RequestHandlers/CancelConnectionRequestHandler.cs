using System.Text.Json;
using AlexaVoxCraft.Model.InSkillPurchasing.Directives;
using AlexaVoxCraft.Model.Response.Directive;

namespace AlexaVoxCraft.Model.InSkillPurchasing.RequestHandlers;

/// <summary>
/// Resolves <c>Connections.SendRequest</c> directives with <c>name == "Cancel"</c> to <see cref="CancelDirective"/>.
/// </summary>
public class CancelConnectionRequestHandler : IConnectionSendRequestHandler
{
    /// <inheritdoc />
    public bool CanCreate(JsonElement data) =>
        data.TryGetProperty("name", out var name) && name.GetString() == "Cancel";

    /// <inheritdoc />
    public Type Create() => typeof(CancelDirective);
}