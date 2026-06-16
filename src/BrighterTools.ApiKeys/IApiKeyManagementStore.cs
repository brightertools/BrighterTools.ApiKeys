namespace BrighterTools.ApiKeys;

/// <summary>
/// Extends API key persistence with management operations used by the orchestrator.
/// </summary>
public interface IApiKeyManagementStore
{
    Task<ApiKeyListResult> ListByOwnerAsync(ApiKeyOwner owner, ApiKeyListRequest request, CancellationToken ct = default);

    Task<ApiKeyRecord?> GetByGuidAsync(ApiKeyOwner owner, Guid keyId, CancellationToken ct = default);

    Task UpdateAsync(ApiKeyRecord record, CancellationToken ct = default);
}
