using System.Text.Json;
using AlexaVoxCraft.Model.InSkillPurchasing.Directives;
using AlexaVoxCraft.Model.Response.Directive;

namespace AlexaVoxCraft.Model.InSkillPurchasing.RequestHandlers;

/// <summary>
/// Resolves <c>Connections.SendRequest</c> directives with <c>name == "Upsell"</c> to <see cref="UpsellDirective"/>.
/// </summary>
public class UpsellConnectionRequestHandler : IConnectionSendRequestHandler
{
    /// <inheritdoc />
    public bool CanCreate(JsonElement data) =>
        data.TryGetProperty("name", out var name) && name.GetString() == "Upsell";

    /// <inheritdoc />
    public Type Create() => typeof(UpsellDirective);
}