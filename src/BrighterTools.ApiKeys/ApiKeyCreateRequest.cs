namespace BrighterTools.ApiKeys;

/// <summary>
/// Represents API key creation input for management workflows.
/// </summary>
public sealed class ApiKeyCreateRequest
{
    public string Name { get; init; } = string.Empty;

    public bool WithSecret { get; init; } = true;
}
