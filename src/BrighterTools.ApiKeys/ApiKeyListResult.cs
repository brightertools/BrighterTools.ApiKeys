namespace BrighterTools.ApiKeys;

/// <summary>
/// Represents a paged API key management list.
/// </summary>
public sealed class ApiKeyListResult
{
    public int TotalCount { get; init; }

    public IReadOnlyList<ApiKeyRecord> Items { get; init; } = Array.Empty<ApiKeyRecord>();
}
