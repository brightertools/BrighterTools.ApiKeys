namespace BrighterTools.ApiKeys;

/// <summary>
/// Defines operations for API Key Usage Tracker.
/// </summary>
public interface IApiKeyUsageTracker
{
    /// <summary>
    /// Executes the track Successful Use Async operation.
    /// </summary>
    /// <param name="result">The result value.</param>
    /// <param name="ct">The ct value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task TrackSuccessfulUseAsync(ApiKeyAuthenticationResult result, CancellationToken ct = default);
}

