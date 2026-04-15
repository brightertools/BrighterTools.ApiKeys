namespace BrighterTools.ApiKeys;

/// <summary>
/// Represents the result of API Key Authentication.
/// </summary>
public sealed class ApiKeyAuthenticationResult
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
    /// Gets or sets the Security Level.
    /// </summary>
    public ApiKeySecurityLevel SecurityLevel { get; init; }
    /// <summary>
    /// Gets or sets a value indicating whether Secret.
    /// </summary>
    public bool UsesSecret { get; init; }
}

