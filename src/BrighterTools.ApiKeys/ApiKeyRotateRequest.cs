namespace BrighterTools.ApiKeys;

/// <summary>
/// Represents API key rotation input for management workflows.
/// </summary>
public sealed class ApiKeyRotateRequest
{
    public Guid Guid { get; init; }

    public int? GraceDays { get; init; }

    public bool WithSecret { get; init; } = true;
}
