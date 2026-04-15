namespace BrighterTools.ApiKeys;

/// <summary>
/// Represents the result of API Key Plain.
/// </summary>
public sealed class ApiKeyPlainResult
{
    /// <summary>
    /// Gets or sets the D.
    /// </summary>
    public int Id { get; init; }
    /// <summary>
    /// Gets or sets the GUID.
    /// </summary>
    public Guid Guid { get; init; }
    /// <summary>
    /// Gets or sets the Owner Type.
    /// </summary>
    public string OwnerType { get; init; } = string.Empty;
    /// <summary>
    /// Gets or sets the Owner ID.
    /// </summary>
    public string OwnerId { get; init; } = string.Empty;
    /// <summary>
    /// Gets or sets the Name.
    /// </summary>
    public string Name { get; init; } = string.Empty;
    /// <summary>
    /// Gets or sets the Plain Key.
    /// </summary>
    public string PlainKey { get; init; } = string.Empty;
    /// <summary>
    /// Gets or sets the Plain Secret.
    /// </summary>
    public string? PlainSecret { get; init; }
    /// <summary>
    /// Gets or sets the Key Preview.
    /// </summary>
    public string KeyPreview { get; init; } = string.Empty;
    /// <summary>
    /// Gets or sets the Security Level.
    /// </summary>
    public ApiKeySecurityLevel SecurityLevel { get; init; } = ApiKeySecurityLevel.Standard;
    /// <summary>
    /// Gets or sets the Status.
    /// </summary>
    public ApiKeyStatus Status { get; init; } = ApiKeyStatus.Revoked;
}

