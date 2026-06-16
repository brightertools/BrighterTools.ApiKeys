using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BrighterTools.ApiKeys;

/// <summary>
/// Provides API Key operations.
/// </summary>
public sealed class ApiKeyService : IApiKeyService
{
    private readonly IApiKeyStore _store;
    private readonly IApiKeyOwnerContext _ownerContext;
    private readonly IApiKeyUsageTracker _usageTracker;
    private readonly ILogger<ApiKeyService> _logger;
    private readonly ApiKeyOptions _options;
    private readonly byte[] _pepper;

    /// <summary>
    /// Executes API Key Service.
    /// </summary>
    public ApiKeyService(IApiKeyStore store, IApiKeyOwnerContext ownerContext, IApiKeyUsageTracker usageTracker, IOptions<ApiKeyOptions> options, ILogger<ApiKeyService> logger)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _ownerContext = ownerContext ?? throw new ArgumentNullException(nameof(ownerContext));
        _usageTracker = usageTracker ?? throw new ArgumentNullException(nameof(usageTracker));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

        if (string.IsNullOrWhiteSpace(_options.Pepper))
        {
            throw new InvalidOperationException("ApiKeySettings:Pepper is required.");
        }

        _pepper = System.Text.Encoding.UTF8.GetBytes(_options.Pepper);
    }

    /// <summary>
    /// Creates Client Key.
    /// </summary>
    public Task<ApiKeyPlainResult> CreateClientKeyAsync(string name, CancellationToken ct = default) => WithCurrentOwnerAsync(owner => CreateClientKeyAsync(owner, name, ct), ct);
    /// <summary>
    /// Creates Server Key.
    /// </summary>
    public Task<ApiKeyPlainResult> CreateServerKeyAsync(string name, bool withSecret = true, CancellationToken ct = default) => WithCurrentOwnerAsync(owner => CreateServerKeyAsync(owner, name, withSecret, ct), ct);
    /// <summary>
    /// Rotates Client Key.
    /// </summary>
    public Task<ApiKeyPlainResult> RotateClientKeyAsync(Guid keyId, int? graceDays = null, CancellationToken ct = default) => WithCurrentOwnerAsync(owner => RotateClientKeyAsync(owner, keyId, graceDays, ct), ct);
    /// <summary>
    /// Rotates Server Key.
    /// </summary>
    public Task<ApiKeyPlainResult> RotateServerKeyAsync(Guid keyId, bool withSecret = true, int? graceDays = null, CancellationToken ct = default) => WithCurrentOwnerAsync(owner => RotateServerKeyAsync(owner, keyId, withSecret, graceDays, ct), ct);

    /// <summary>
    /// Creates Client Key.
    /// </summary>
    public async Task<ApiKeyPlainResult> CreateClientKeyAsync(ApiKeyOwner owner, string name, CancellationToken ct = default)
    {
        return await CreateKeyInternalAsync(owner, name, ApiKeySecurityLevel.Client, withSecret: false, rotatedFrom: null, ct);
    }

    /// <summary>
    /// Creates Server Key.
    /// </summary>
    public async Task<ApiKeyPlainResult> CreateServerKeyAsync(ApiKeyOwner owner, string name, bool withSecret = true, CancellationToken ct = default)
    {
        return await CreateKeyInternalAsync(owner, name, ApiKeySecurityLevel.Standard, withSecret, rotatedFrom: null, ct);
    }

    /// <summary>
    /// Rotates Client Key.
    /// </summary>
    public async Task<ApiKeyPlainResult> RotateClientKeyAsync(ApiKeyOwner owner, Guid keyId, int? graceDays = null, CancellationToken ct = default)
    {
        var current = await _store.GetByGuidAsync(owner.OwnerType, owner.OwnerId, keyId, ct) ?? throw new InvalidOperationException("Key not found.");
        if (current.SecurityLevel != ApiKeySecurityLevel.Client)
        {
            throw new InvalidOperationException("Key is not a client API key.");
        }

        current.Status = ApiKeyStatus.Retiring;
        current.ExpiryDate = DateTimeOffset.UtcNow.AddDays(graceDays ?? _options.DefaultRotationGraceDays);
        await _store.UpdateAsync(current, ct);

        return await CreateKeyInternalAsync(owner, current.Name, ApiKeySecurityLevel.Client, withSecret: false, rotatedFrom: current.Guid, ct);
    }

    /// <summary>
    /// Rotates Server Key.
    /// </summary>
    public async Task<ApiKeyPlainResult> RotateServerKeyAsync(ApiKeyOwner owner, Guid keyId, bool withSecret = true, int? graceDays = null, CancellationToken ct = default)
    {
        var current = await _store.GetByGuidAsync(owner.OwnerType, owner.OwnerId, keyId, ct) ?? throw new InvalidOperationException("Key not found.");
        if (current.SecurityLevel == ApiKeySecurityLevel.Client)
        {
            throw new InvalidOperationException("Key is not a server API key.");
        }

        current.Status = ApiKeyStatus.Retiring;
        current.ExpiryDate = DateTimeOffset.UtcNow.AddDays(graceDays ?? _options.DefaultRotationGraceDays);
        await _store.UpdateAsync(current, ct);

        return await CreateKeyInternalAsync(owner, current.Name, current.SecurityLevel, withSecret, rotatedFrom: current.Guid, ct);
    }

    /// <summary>
    /// Authenticates Client Key.
    /// </summary>
    public async Task<ApiKeyAuthenticationResult?> AuthenticateClientKeyAsync(string plainApiKey, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(plainApiKey))
        {
            return null;
        }

        var incomingHash = ApiKeyGenerator.Hash(plainApiKey, _pepper);
        var now = DateTimeOffset.UtcNow;
        var candidates = await _store.FindByPreviewAsync(plainApiKey, ct);

        var record = candidates.FirstOrDefault(x =>
            x.SecurityLevel == ApiKeySecurityLevel.Client &&
            IsAuthenticatable(x, now) &&
            CryptographicOperations.FixedTimeEquals(x.KeyHash, incomingHash));

        if (record == null)
        {
            return null;
        }

        var result = ToAuthenticationResult(record, usesSecret: false);
        await _usageTracker.TrackSuccessfulUseAsync(result, ct);
        return result;
    }

    /// <summary>
    /// Authenticates Server Key.
    /// </summary>
    /// <summary>
    /// Authenticates no-secret Server Key.
    /// </summary>
    public async Task<ApiKeyAuthenticationResult?> AuthenticateServerKeyAsync(string plainApiKey, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(plainApiKey))
        {
            return null;
        }

        var previewLength = Math.Max(1, _options.ServerKeyPreviewLength);
        var preview = plainApiKey.Length > previewLength ? plainApiKey[..previewLength] : plainApiKey;
        var incomingKeyHash = ApiKeyGenerator.Hash(plainApiKey, _pepper);
        var now = DateTimeOffset.UtcNow;
        var candidates = await _store.FindByPreviewAsync(preview, ct);

        var record = candidates.FirstOrDefault(x =>
            x.SecurityLevel != ApiKeySecurityLevel.Client &&
            x.SecretHash == null &&
            IsAuthenticatable(x, now) &&
            CryptographicOperations.FixedTimeEquals(x.KeyHash, incomingKeyHash));

        if (record == null)
        {
            return null;
        }

        var result = ToAuthenticationResult(record, usesSecret: false);
        await _usageTracker.TrackSuccessfulUseAsync(result, ct);
        return result;
    }
    public async Task<ApiKeyAuthenticationResult?> AuthenticateServerKeyAsync(string plainApiKey, string plainSecret, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(plainApiKey) || string.IsNullOrWhiteSpace(plainSecret))
        {
            return null;
        }

        var previewLength = Math.Max(1, _options.ServerKeyPreviewLength);
        var preview = plainApiKey.Length > previewLength ? plainApiKey[..previewLength] : plainApiKey;
        var incomingKeyHash = ApiKeyGenerator.Hash(plainApiKey, _pepper);
        var incomingSecretHash = ApiKeyGenerator.Hash(plainSecret, _pepper);
        var now = DateTimeOffset.UtcNow;
        var candidates = await _store.FindByPreviewAsync(preview, ct);

        var record = candidates.FirstOrDefault(x =>
            x.SecurityLevel != ApiKeySecurityLevel.Client &&
            x.SecretHash != null &&
            IsAuthenticatable(x, now) &&
            CryptographicOperations.FixedTimeEquals(x.KeyHash, incomingKeyHash) &&
            CryptographicOperations.FixedTimeEquals(x.SecretHash, incomingSecretHash));

        if (record == null)
        {
            return null;
        }

        var result = ToAuthenticationResult(record, usesSecret: true);
        await _usageTracker.TrackSuccessfulUseAsync(result, ct);
        return result;
    }

    private async Task<ApiKeyPlainResult> CreateKeyInternalAsync(ApiKeyOwner owner, string name, ApiKeySecurityLevel securityLevel, bool withSecret, Guid? rotatedFrom, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        if (!rotatedFrom.HasValue && await _store.NameExistsAsync(owner.OwnerType, owner.OwnerId, name, ct))
        {
            throw new InvalidOperationException("An API key with that name already exists.");
        }

        var (plainKey, keyHash, preview) = GenerateKey(securityLevel == ApiKeySecurityLevel.Client);
        var secret = GenerateSecret(withSecret);

        var record = new ApiKeyRecord
        {
            OwnerType = owner.OwnerType,
            OwnerId = owner.OwnerId,
            Name = name,
            SecurityLevel = securityLevel,
            Status = ApiKeyStatus.Active,
            KeyHash = keyHash,
            KeyPreview = preview,
            SecretHash = secret?.hash,
            RotatedFromKeyGuid = rotatedFrom
        };

        await _store.CreateAsync(record, ct);

        return new ApiKeyPlainResult
        {
            Id = record.Id,
            Guid = record.Guid,
            OwnerType = owner.OwnerType,
            OwnerId = owner.OwnerId,
            Name = name,
            PlainKey = plainKey,
            PlainSecret = secret?.plain,
            KeyPreview = preview,
            SecurityLevel = securityLevel,
            Status = record.Status
        };
    }

    private static bool IsAuthenticatable(ApiKeyRecord record, DateTimeOffset now)
    {
        var statusAllowsUse = record.Status == ApiKeyStatus.Active || record.Status == ApiKeyStatus.Retiring;
        return statusAllowsUse && (record.ExpiryDate == null || record.ExpiryDate > now);
    }
    private async Task<ApiKeyPlainResult> WithCurrentOwnerAsync(Func<ApiKeyOwner, Task<ApiKeyPlainResult>> action, CancellationToken ct)
    {
        var owner = await _ownerContext.GetCurrentOwnerAsync(ct) ?? throw new InvalidOperationException("No current API key owner is available.");
        return await action(owner);
    }

    private ApiKeyAuthenticationResult ToAuthenticationResult(ApiKeyRecord record, bool usesSecret)
    {
        return new ApiKeyAuthenticationResult
        {
            Id = record.Id,
            Guid = record.Guid,
            OwnerType = record.OwnerType,
            OwnerId = record.OwnerId,
            Name = record.Name,
            SecurityLevel = record.SecurityLevel,
            UsesSecret = usesSecret
        };
    }

    private (string plain, byte[] hash, string preview) GenerateKey(bool fullPreview)
    {
        var plain = ApiKeyGenerator.NewKey();
        var preview = fullPreview ? plain : plain[..Math.Min(_options.ServerKeyPreviewLength, plain.Length)];
        var hash = ApiKeyGenerator.Hash(plain, _pepper);
        return (plain, hash, preview);
    }

    private (string plain, byte[] hash)? GenerateSecret(bool withSecret)
    {
        if (!withSecret)
        {
            return null;
        }

        var plain = ApiKeyGenerator.NewKey(64);
        return (plain, ApiKeyGenerator.Hash(plain, _pepper));
    }
}




