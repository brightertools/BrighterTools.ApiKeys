namespace BrighterTools.ApiKeys;

/// <summary>
/// Defines operations for API Key Service.
/// </summary>
public interface IApiKeyService
{
    /// <summary>
    /// Creates the create Client Key Async.
    /// </summary>
    /// <param name="name">The name value.</param>
    /// <param name="ct">The ct value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    Task<ApiKeyPlainResult> CreateClientKeyAsync(string name, CancellationToken ct = default);
    /// <summary>
    /// Creates the create Client Key Async.
    /// </summary>
    /// <param name="owner">The owner value.</param>
    /// <param name="name">The name value.</param>
    /// <param name="ct">The ct value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    Task<ApiKeyPlainResult> CreateClientKeyAsync(ApiKeyOwner owner, string name, CancellationToken ct = default);
    /// <summary>
    /// Creates the create Server Key Async.
    /// </summary>
    /// <param name="name">The name value.</param>
    /// <param name="withSecret">The withSecret value.</param>
    /// <param name="ct">The ct value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    Task<ApiKeyPlainResult> CreateServerKeyAsync(string name, bool withSecret = true, CancellationToken ct = default);
    /// <summary>
    /// Creates the create Server Key Async.
    /// </summary>
    /// <param name="owner">The owner value.</param>
    /// <param name="name">The name value.</param>
    /// <param name="withSecret">The withSecret value.</param>
    /// <param name="ct">The ct value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    Task<ApiKeyPlainResult> CreateServerKeyAsync(ApiKeyOwner owner, string name, bool withSecret = true, CancellationToken ct = default);
    /// <summary>
    /// Rotates the rotate Client Key Async.
    /// </summary>
    /// <param name="keyId">The keyId value.</param>
    /// <param name="graceDays">The graceDays value.</param>
    /// <param name="ct">The ct value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    Task<ApiKeyPlainResult> RotateClientKeyAsync(Guid keyId, int? graceDays = null, CancellationToken ct = default);
    /// <summary>
    /// Rotates the rotate Client Key Async.
    /// </summary>
    /// <param name="owner">The owner value.</param>
    /// <param name="keyId">The keyId value.</param>
    /// <param name="graceDays">The graceDays value.</param>
    /// <param name="ct">The ct value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    Task<ApiKeyPlainResult> RotateClientKeyAsync(ApiKeyOwner owner, Guid keyId, int? graceDays = null, CancellationToken ct = default);
    /// <summary>
    /// Rotates the rotate Server Key Async.
    /// </summary>
    /// <param name="keyId">The keyId value.</param>
    /// <param name="withSecret">The withSecret value.</param>
    /// <param name="graceDays">The graceDays value.</param>
    /// <param name="ct">The ct value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    Task<ApiKeyPlainResult> RotateServerKeyAsync(Guid keyId, bool withSecret = true, int? graceDays = null, CancellationToken ct = default);
    /// <summary>
    /// Rotates the rotate Server Key Async.
    /// </summary>
    /// <param name="owner">The owner value.</param>
    /// <param name="keyId">The keyId value.</param>
    /// <param name="withSecret">The withSecret value.</param>
    /// <param name="graceDays">The graceDays value.</param>
    /// <param name="ct">The ct value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    Task<ApiKeyPlainResult> RotateServerKeyAsync(ApiKeyOwner owner, Guid keyId, bool withSecret = true, int? graceDays = null, CancellationToken ct = default);
    /// <summary>
    /// Authenticates the authenticate Client Key Async.
    /// </summary>
    /// <param name="plainApiKey">The plainApiKey value.</param>
    /// <param name="ct">The ct value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    Task<ApiKeyAuthenticationResult?> AuthenticateClientKeyAsync(string plainApiKey, CancellationToken ct = default);
    /// <summary>
    /// Authenticates the authenticate Server Key Async.
    /// </summary>
    /// <param name="plainApiKey">The plainApiKey value.</param>
    /// <param name="plainSecret">The plainSecret value.</param>
    /// <param name="ct">The ct value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    Task<ApiKeyAuthenticationResult?> AuthenticateServerKeyAsync(string plainApiKey, string plainSecret, CancellationToken ct = default);
}

