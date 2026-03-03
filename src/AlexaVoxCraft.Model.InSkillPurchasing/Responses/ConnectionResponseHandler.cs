using System.Text.Json;
using AlexaVoxCraft.Model.Request.Type;

namespace AlexaVoxCraft.Model.InSkillPurchasing.Responses;

/// <summary>
/// Registers the in-skill purchasing connection response handler with the <see cref="ConnectionResponseTypeResolver"/>.
/// </summary>
public class ConnectionResponseHandler
{
    /// <summary>
    /// Registers the <see cref="PaymentConnectionResponseHandler"/> so that Buy, Upsell, and Cancel
    /// <c>Connections.Response</c> requests are resolved to <see cref="ConnectionResponseRequest{T}"/> with a <see cref="ConnectionResponsePayload"/>.
    /// Call this once at application startup before processing skill requests.
    /// </summary>
    public static void AddSupport()
    {
        ConnectionResponseTypeResolver.Register(new PaymentConnectionResponseHandler());
    }

    /// <summary>
    /// Resolves <c>Connections.Response</c> requests whose <c>name</c> matches a payment type
    /// to <see cref="ConnectionResponseRequest{ConnectionResponsePayload}"/>.
    /// </summary>
    internal class PaymentConnectionResponseHandler : IConnectionResponseHandler
    {
        private readonly string[] _names = [PaymentType.Buy, PaymentType.Upsell, PaymentType.Cancel];

        /// <inheritdoc />
        public bool CanCreate(JsonElement data) =>
            data.TryGetProperty("name", out var name) && _names.Contains(name.GetString());

        /// <inheritdoc />
        public Type Create(JsonElement element) => typeof(ConnectionResponseRequest<ConnectionResponsePayload>);
    }
}