namespace BrighterTools.ApiKeys;

/// <summary>
/// Represents API Key Record.
/// </summary>
public sealed class ApiKeyRecord
{
    /// <summary>
    /// Gets or sets the D.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Gets or sets the guid.
    /// </summary>
    public Guid Guid { get; set; } = Guid.NewGuid();
    /// <summary>
    /// Gets or sets the Owner Type.
    /// </summary>
    public string OwnerType { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the Owner ID.
    /// </summary>
    public string OwnerId { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the Name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the key Hash.
    /// </summary>
    public byte[] KeyHash { get; set; } = Array.Empty<byte>();
    /// <summary>
    /// Gets or sets the Key Preview.
    /// </summary>
    public string KeyPreview { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the Secret Hash.
    /// </summary>
    public byte[]? SecretHash { get; set; }
    /// <summary>
    /// Gets or sets the Security Level.
    /// </summary>
    public ApiKeySecurityLevel SecurityLevel { get; set; } = ApiKeySecurityLevel.Standard;
    /// <summary>
    /// Gets or sets the Status.
    /// </summary>
    public ApiKeyStatus Status { get; set; } = ApiKeyStatus.Active;
    /// <summary>
    /// Gets or sets the Expiry Date.
    /// </summary>
    public DateTimeOffset? ExpiryDate { get; set; }
    /// <summary>
    /// Gets or sets the Last Used Date.
    /// </summary>
    public DateTimeOffset? LastUsedDate { get; set; }
    /// <summary>
    /// Gets or sets the Rotated From Key GUID.
    /// </summary>
    public Guid? RotatedFromKeyGuid { get; set; }
}

