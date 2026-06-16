namespace BrighterTools.ApiKeys;

/// <summary>
/// Represents configuration options for API Key.
/// </summary>
public sealed class ApiKeyOptions
{
    /// <summary>
    /// Gets or sets the Pepper.
    /// </summary>
    public string Pepper { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the Client Header Name.
    /// </summary>
    public string ClientHeaderName { get; set; } = ApiKeyDefaults.ClientHeaderName;
    /// <summary>
    /// Gets or sets a value indicating whether client API keys are enabled for this application.
    /// </summary>
    public bool ClientKeysEnabled { get; set; } = true;
    /// <summary>
    /// Gets or sets the Server Key Header Name.
    /// </summary>
    public string ServerKeyHeaderName { get; set; } = ApiKeyDefaults.ServerKeyHeaderName;
    /// <summary>
    /// Gets or sets the Server Secret Header Name.
    /// </summary>
    public string ServerSecretHeaderName { get; set; } = ApiKeyDefaults.ServerSecretHeaderName;
    /// <summary>
    /// Gets or sets the Client Scheme Name.
    /// </summary>
    public string ClientSchemeName { get; set; } = ApiKeyDefaults.ClientSchemeName;
    /// <summary>
    /// Gets or sets the Server Scheme Name.
    /// </summary>
    public string ServerSchemeName { get; set; } = ApiKeyDefaults.ServerSchemeName;
    /// <summary>
    /// Gets or sets the Policy Scheme Name.
    /// </summary>
    public string PolicySchemeName { get; set; } = ApiKeyDefaults.PolicySchemeName;
    /// <summary>
    /// Gets or sets the Fallback Scheme Name.
    /// </summary>
    public string? FallbackSchemeName { get; set; }
    /// <summary>
    /// Gets or sets the Server Key Preview Length.
    /// </summary>
    public int ServerKeyPreviewLength { get; set; } = ApiKeyDefaults.DefaultServerPreviewLength;
    /// <summary>
    /// Gets or sets the Default Rotation Grace Days.
    /// </summary>
    public int DefaultRotationGraceDays { get; set; } = ApiKeyDefaults.DefaultRotationGraceDays;
}

