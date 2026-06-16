namespace BrighterTools.ApiKeys;

/// <summary>
/// Represents API key revoke input for management workflows.
/// </summary>
public sealed class ApiKeyRevokeRequest
{
    public Guid Guid { get; init; }
}
