using System.Text.Json;
using AlexaVoxCraft.Model.InSkillPurchasing.Directives;
using AlexaVoxCraft.Model.Response.Directive;

namespace AlexaVoxCraft.Model.InSkillPurchasing.RequestHandlers;

/// <summary>
/// Resolves <c>Connections.SendRequest</c> directives with <c>name == "Buy"</c> to <see cref="BuyDirective"/>.
/// </summary>
public class BuyConnectionRequestHandler : IConnectionSendRequestHandler
{
    /// <inheritdoc />
    public bool CanCreate(JsonElement data) =>
        data.TryGetProperty("name", out var name) && name.GetString() == "Buy";

    /// <inheritdoc />
    public Type Create() => typeof(BuyDirective);
}