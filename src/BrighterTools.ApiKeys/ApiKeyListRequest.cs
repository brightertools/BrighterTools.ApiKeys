namespace BrighterTools.ApiKeys;

/// <summary>
/// Represents API key list filtering and paging options.
/// </summary>
public sealed class ApiKeyListRequest
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 25;

    public string? Search { get; init; }

    public ApiKeySecurityLevel? SecurityLevel { get; init; }

    public bool ServerKeysOnly { get; init; }

    public ApiKeyStatus? Status { get; init; }

    public bool IncludeExpired { get; init; }

    public bool SortDescending { get; init; } = true;
}

