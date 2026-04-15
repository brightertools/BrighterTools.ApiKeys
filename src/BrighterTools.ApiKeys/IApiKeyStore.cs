namespace BrighterTools.ApiKeys;

/// <summary>
/// Defines operations for API Key Store.
/// </summary>
public interface IApiKeyStore
{
    /// <summary>
    /// Executes the name Exists Async operation.
    /// </summary>
    /// <param name="ownerType">The ownerType value.</param>
    /// <param name="ownerId">The ownerId value.</param>
    /// <param name="name">The name value.</param>
    /// <param name="ct">The ct value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    Task<bool> NameExistsAsync(string ownerType, string ownerId, string name, CancellationToken ct = default);
    /// <summary>
    /// Creates the create Async.
    /// </summary>
    /// <param name="record">The record value.</param>
    /// <param name="ct">The ct value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task CreateAsync(ApiKeyRecord record, CancellationToken ct = default);
    /// <summary>
    /// Updates the update Async.
    /// </summary>
    /// <param name="record">The record value.</param>
    /// <param name="ct">The ct value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateAsync(ApiKeyRecord record, CancellationToken ct = default);
    /// <summary>
    /// Gets the get By Guid Async.
    /// </summary>
    /// <param name="ownerType">The ownerType value.</param>
    /// <param name="ownerId">The ownerId value.</param>
    /// <param name="keyId">The keyId value.</param>
    /// <param name="ct">The ct value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    Task<ApiKeyRecord?> GetByGuidAsync(string ownerType, string ownerId, Guid keyId, CancellationToken ct = default);
    /// <summary>
    /// Finds the find By Preview Async.
    /// </summary>
    /// <param name="preview">The preview value.</param>
    /// <param name="ct">The ct value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    Task<IReadOnlyList<ApiKeyRecord>> FindByPreviewAsync(string preview, CancellationToken ct = default);
    /// <summary>
    /// Executes the touch Last Used Async operation.
    /// </summary>
    /// <param name="id">The id value.</param>
    /// <param name="usedAt">The usedAt value.</param>
    /// <param name="ct">The ct value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task TouchLastUsedAsync(int id, DateTimeOffset usedAt, CancellationToken ct = default);
}

