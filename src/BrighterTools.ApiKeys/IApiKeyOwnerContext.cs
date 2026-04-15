namespace BrighterTools.ApiKeys;

/// <summary>
/// Defines operations for API Key Owner Context.
/// </summary>
public interface IApiKeyOwnerContext
{
    /// <summary>
    /// Gets the get Current Owner Async.
    /// </summary>
    /// <param name="ct">The ct value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    Task<ApiKeyOwner?> GetCurrentOwnerAsync(CancellationToken ct = default);
}

