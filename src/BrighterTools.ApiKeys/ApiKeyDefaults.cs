namespace BrighterTools.ApiKeys;

/// <summary>
/// Represents API Key Defaults.
/// </summary>
public static class ApiKeyDefaults
{
    /// <summary>
    /// Gets the configuration Section Name value.
    /// </summary>
    public const string ConfigurationSectionName = "ApiKeySettings";
    /// <summary>
    /// Gets the client Scheme Name value.
    /// </summary>
    public const string ClientSchemeName = "ClientApiKeyScheme";
    /// <summary>
    /// Gets the server Scheme Name value.
    /// </summary>
    public const string ServerSchemeName = "ServerApiKeyScheme";
    /// <summary>
    /// Gets the policy Scheme Name value.
    /// </summary>
    public const string PolicySchemeName = "ApiKeyPolicyScheme";
    /// <summary>
    /// Gets the client Header Name value.
    /// </summary>
    public const string ClientHeaderName = "X-Client-Api-Key";
    /// <summary>
    /// Gets the server Key Header Name value.
    /// </summary>
    public const string ServerKeyHeaderName = "X-Api-Key";
    /// <summary>
    /// Gets the server Secret Header Name value.
    /// </summary>
    public const string ServerSecretHeaderName = "X-Api-Secret";
    /// <summary>
    /// Gets the default Server Preview Length value.
    /// </summary>
    public const int DefaultServerPreviewLength = 6;
    /// <summary>
    /// Gets the default Rotation Grace Days value.
    /// </summary>
    public const int DefaultRotationGraceDays = 30;
}

