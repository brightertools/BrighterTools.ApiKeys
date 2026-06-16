using Microsoft.Extensions.Options;

namespace BrighterTools.ApiKeys;

/// <summary>
/// Provides high-level API key management orchestration for admin workflows.
/// </summary>
public sealed class ApiKeyOrchestrator : IApiKeyOrchestrator
{
    private readonly IApiKeyService _apiKeyService;
    private readonly IApiKeyOwnerContext _ownerContext;
    private readonly IApiKeyManagementStore _managementStore;
    private readonly ApiKeyOptions _options;

    public ApiKeyOrchestrator(
        IApiKeyService apiKeyService,
        IApiKeyOwnerContext ownerContext,
        IApiKeyManagementStore managementStore,
        IOptions<ApiKeyOptions> options)
    {
        _apiKeyService = apiKeyService;
        _ownerContext = ownerContext;
        _managementStore = managementStore;
        _options = options.Value;
    }

    public async Task<ApiKeyOperationResult<ApiKeyPlainResult>> CreateClientKeyAsync(ApiKeyCreateRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return ApiKeyOperationResult<ApiKeyPlainResult>.Fail("Name is required.");
        }

        return await ExecutePlainAsync(() => _apiKeyService.CreateClientKeyAsync(request.Name.Trim(), ct));
    }

    public async Task<ApiKeyOperationResult<ApiKeyPlainResult>> CreateServerKeyAsync(ApiKeyCreateRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return ApiKeyOperationResult<ApiKeyPlainResult>.Fail("Name is required.");
        }

        return await ExecutePlainAsync(() => _apiKeyService.CreateServerKeyAsync(request.Name.Trim(), request.WithSecret, ct));
    }

    public async Task<ApiKeyOperationResult<ApiKeyListResult>> ListClientKeysAsync(ApiKeyListRequest request, CancellationToken ct = default)
    {
        var scopedRequest = WithSecurityLevel(request, ApiKeySecurityLevel.Client);
        return await ListAsync(scopedRequest, ct);
    }

    public async Task<ApiKeyOperationResult<ApiKeyListResult>> ListServerKeysAsync(ApiKeyListRequest request, CancellationToken ct = default)
    {
        return await ListAsync(WithServerKeysOnly(request), ct);
    }

    public async Task<ApiKeyOperationResult<ApiKeyPlainResult>> RotateClientKeyAsync(ApiKeyRotateRequest request, CancellationToken ct = default)
    {
        if (request.Guid == Guid.Empty)
        {
            return ApiKeyOperationResult<ApiKeyPlainResult>.Fail("Guid is required.");
        }

        return await ExecutePlainAsync(() => _apiKeyService.RotateClientKeyAsync(request.Guid, request.GraceDays ?? _options.DefaultRotationGraceDays, ct));
    }

    public async Task<ApiKeyOperationResult<ApiKeyPlainResult>> RotateServerKeyAsync(ApiKeyRotateRequest request, CancellationToken ct = default)
    {
        if (request.Guid == Guid.Empty)
        {
            return ApiKeyOperationResult<ApiKeyPlainResult>.Fail("Guid is required.");
        }

        return await ExecutePlainAsync(() => _apiKeyService.RotateServerKeyAsync(request.Guid, request.WithSecret, request.GraceDays ?? _options.DefaultRotationGraceDays, ct));
    }

    public async Task<ApiKeyOperationResult<ApiKeyRecord>> RevokeKeyAsync(ApiKeyRevokeRequest request, CancellationToken ct = default)
    {
        if (request.Guid == Guid.Empty)
        {
            return ApiKeyOperationResult<ApiKeyRecord>.Fail("Guid is required.");
        }

        var owner = await GetCurrentOwnerAsync(ct);
        if (owner == null)
        {
            return ApiKeyOperationResult<ApiKeyRecord>.Fail("No current API key owner is available.");
        }

        var record = await _managementStore.GetByGuidAsync(owner, request.Guid, ct);
        if (record == null)
        {
            return ApiKeyOperationResult<ApiKeyRecord>.Fail("Key not found.");
        }

        record.Status = ApiKeyStatus.Revoked;
        record.ExpiryDate = DateTimeOffset.UtcNow;
        await _managementStore.UpdateAsync(record, ct);

        return ApiKeyOperationResult<ApiKeyRecord>.Success(record);
    }

    private async Task<ApiKeyOperationResult<ApiKeyListResult>> ListAsync(ApiKeyListRequest request, CancellationToken ct)
    {
        var owner = await GetCurrentOwnerAsync(ct);
        if (owner == null)
        {
            return ApiKeyOperationResult<ApiKeyListResult>.Fail("No current API key owner is available.");
        }

        return ApiKeyOperationResult<ApiKeyListResult>.Success(await _managementStore.ListByOwnerAsync(owner, request, ct));
    }

    private async Task<ApiKeyOwner?> GetCurrentOwnerAsync(CancellationToken ct)
    {
        return await _ownerContext.GetCurrentOwnerAsync(ct);
    }

    private static ApiKeyListRequest WithSecurityLevel(ApiKeyListRequest request, ApiKeySecurityLevel securityLevel)
    {
        return new ApiKeyListRequest
        {
            Page = request.Page,
            PageSize = request.PageSize,
            Search = request.Search,
            SecurityLevel = securityLevel,
            Status = request.Status,
            IncludeExpired = request.IncludeExpired,
            ServerKeysOnly = request.ServerKeysOnly,
            SortDescending = request.SortDescending
        };
    }

    private static ApiKeyListRequest WithServerKeysOnly(ApiKeyListRequest request)
    {
        return new ApiKeyListRequest
        {
            Page = request.Page,
            PageSize = request.PageSize,
            Search = request.Search,
            SecurityLevel = null,
            ServerKeysOnly = true,
            Status = request.Status,
            IncludeExpired = request.IncludeExpired,
            SortDescending = request.SortDescending
        };
    }
    private static async Task<ApiKeyOperationResult<ApiKeyPlainResult>> ExecutePlainAsync(Func<Task<ApiKeyPlainResult>> action)
    {
        try
        {
            return ApiKeyOperationResult<ApiKeyPlainResult>.Success(await action());
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return ApiKeyOperationResult<ApiKeyPlainResult>.Fail(exception.Message);
        }
    }
}


