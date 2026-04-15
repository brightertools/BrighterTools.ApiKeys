namespace BrighterTools.ApiKeys;

internal sealed class NoopApiKeyUsageTracker : IApiKeyUsageTracker
{
    /// <summary>
    /// Tracks Successful Use.
    /// </summary>
    public Task TrackSuccessfulUseAsync(ApiKeyAuthenticationResult result, CancellationToken ct = default) => Task.CompletedTask;
}

