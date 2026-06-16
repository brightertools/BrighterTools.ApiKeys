namespace BrighterTools.ApiKeys;

/// <summary>
/// Defines high-level API key management operations for application admin flows.
/// </summary>
public interface IApiKeyOrchestrator
{
    Task<ApiKeyOperationResult<ApiKeyPlainResult>> CreateClientKeyAsync(ApiKeyCreateRequest request, CancellationToken ct = default);

    Task<ApiKeyOperationResult<ApiKeyPlainResult>> CreateServerKeyAsync(ApiKeyCreateRequest request, CancellationToken ct = default);

    Task<ApiKeyOperationResult<ApiKeyListResult>> ListClientKeysAsync(ApiKeyListRequest request, CancellationToken ct = default);

    Task<ApiKeyOperationResult<ApiKeyListResult>> ListServerKeysAsync(ApiKeyListRequest request, CancellationToken ct = default);

    Task<ApiKeyOperationResult<ApiKeyPlainResult>> RotateClientKeyAsync(ApiKeyRotateRequest request, CancellationToken ct = default);

    Task<ApiKeyOperationResult<ApiKeyPlainResult>> RotateServerKeyAsync(ApiKeyRotateRequest request, CancellationToken ct = default);

    Task<ApiKeyOperationResult<ApiKeyRecord>> RevokeKeyAsync(ApiKeyRevokeRequest request, CancellationToken ct = default);
}
