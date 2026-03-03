using AlexaVoxCraft.InSkillPurchasing.Models;

namespace AlexaVoxCraft.InSkillPurchasing.Clients;

/// <summary>
/// Client for interacting with the Alexa In-Skill Purchasing API.
/// </summary>
public interface IInSkillPurchasingClient
{
    /// <summary>
    /// Retrieves a list of in-skill products available to the current user.
    /// </summary>
    /// <param name="filter">Optional filter parameters to narrow the product list.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="ProductResponse"/> containing matching products, or <c>null</c> if not found.</returns>
    Task<ProductResponse?> GetProductsAsync(ProductFilter? filter = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a specific in-skill product by its identifier.
    /// </summary>
    /// <param name="productId">The unique identifier of the product.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The <see cref="Product"/>, or <c>null</c> if not found.</returns>
    Task<Product?> GetProductAsync(string productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves whether voice purchasing is enabled for the current skill.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="PurchasingEnabled"/> record, or <c>null</c> if not found.</returns>
    Task<PurchasingEnabled?> GetPurchasingEnabledAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of in-skill purchase transactions for the current user.
    /// </summary>
    /// <param name="filter">Optional filter parameters to narrow the transaction list.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="TransactionResponse"/> containing matching transactions, or <c>null</c> if not found.</returns>
    Task<TransactionResponse?> GetTransactionsAsync(TransactionFilter? filter = null,
        CancellationToken cancellationToken = default);
}