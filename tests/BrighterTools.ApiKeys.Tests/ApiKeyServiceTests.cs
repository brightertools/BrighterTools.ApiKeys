using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace BrighterTools.ApiKeys.Tests;

public class ApiKeyServiceTests
{
    [Fact]
    public async Task CreateServerKeyAsync_PersistsHashedServerKeyAndSecret()
    {
        var store = new InMemoryApiKeyStore();
        var tracker = new CapturingUsageTracker();
        var service = CreateService(store, tracker);

        var result = await service.CreateServerKeyAsync("Primary");

        result.PlainKey.Should().NotBeNullOrWhiteSpace();
        result.PlainSecret.Should().NotBeNullOrWhiteSpace();
        store.Records.Should().ContainSingle();
        store.Records[0].SecurityLevel.Should().Be(ApiKeySecurityLevel.Standard);
        store.Records[0].SecretHash.Should().NotBeNull();
        store.Records[0].KeyHash.Should().NotBeEmpty();
        tracker.Count.Should().Be(0);
    }

    [Fact]
    public async Task AuthenticateClientKeyAsync_ReturnsResultAndTracksUsage()
    {
        var store = new InMemoryApiKeyStore();
        var tracker = new CapturingUsageTracker();
        var service = CreateService(store, tracker);
        var created = await service.CreateClientKeyAsync("Browser");

        var result = await service.AuthenticateClientKeyAsync(created.PlainKey);

        result.Should().NotBeNull();
        result!.UsesSecret.Should().BeFalse();
        result.SecurityLevel.Should().Be(ApiKeySecurityLevel.Client);
        tracker.Count.Should().Be(1);
    }

    [Fact]
    public async Task RotateServerKeyAsync_RetiresExistingKeyAndCreatesReplacement()
    {
        var store = new InMemoryApiKeyStore();
        var service = CreateService(store, new CapturingUsageTracker());
        var created = await service.CreateServerKeyAsync("Primary");

        var replacement = await service.RotateServerKeyAsync(created.Guid, withSecret: true, graceDays: 14);

        replacement.Guid.Should().NotBe(created.Guid);
        store.Records.Should().HaveCount(2);
        store.Records.Should().ContainSingle(x => x.Guid == created.Guid && x.Status == ApiKeyStatus.Retiring);
        store.Records.Should().ContainSingle(x => x.Guid == replacement.Guid && x.RotatedFromKeyGuid == created.Guid);
    }

    private static ApiKeyService CreateService(InMemoryApiKeyStore store, CapturingUsageTracker tracker)
    {
        var options = Options.Create(new ApiKeyOptions
        {
            Pepper = "test-pepper",
            ServerKeyPreviewLength = 8,
            DefaultRotationGraceDays = 30
        });

        return new ApiKeyService(
            store,
            new StaticOwnerContext(ApiKeyOwner.Create("tenant", "42")),
            tracker,
            options,
            NullLogger<ApiKeyService>.Instance);
    }

    private sealed class StaticOwnerContext : IApiKeyOwnerContext
    {
        private readonly ApiKeyOwner _owner;

        public StaticOwnerContext(ApiKeyOwner owner)
        {
            _owner = owner;
        }

        public Task<ApiKeyOwner?> GetCurrentOwnerAsync(CancellationToken ct = default) => Task.FromResult<ApiKeyOwner?>(_owner);
    }

    private sealed class CapturingUsageTracker : IApiKeyUsageTracker
    {
        public int Count { get; private set; }

        public Task TrackSuccessfulUseAsync(ApiKeyAuthenticationResult result, CancellationToken ct = default)
        {
            Count++;
            return Task.CompletedTask;
        }
    }

    private sealed class InMemoryApiKeyStore : IApiKeyStore
    {
        public List<ApiKeyRecord> Records { get; } = [];

        public Task<bool> NameExistsAsync(string ownerType, string ownerId, string name, CancellationToken ct = default)
        {
            return Task.FromResult(Records.Any(x => x.OwnerType == ownerType && x.OwnerId == ownerId && x.Name == name));
        }

        public Task CreateAsync(ApiKeyRecord record, CancellationToken ct = default)
        {
            record.Id = Records.Count + 1;
            Records.Add(Clone(record));
            return Task.CompletedTask;
        }

        public Task UpdateAsync(ApiKeyRecord record, CancellationToken ct = default)
        {
            var existing = Records.Single(x => x.Guid == record.Guid);
            Records.Remove(existing);
            Records.Add(Clone(record));
            return Task.CompletedTask;
        }

        public Task<ApiKeyRecord?> GetByGuidAsync(string ownerType, string ownerId, Guid keyId, CancellationToken ct = default)
        {
            var record = Records.SingleOrDefault(x => x.OwnerType == ownerType && x.OwnerId == ownerId && x.Guid == keyId);
            return Task.FromResult(record == null ? null : Clone(record));
        }

        public Task<IReadOnlyList<ApiKeyRecord>> FindByPreviewAsync(string preview, CancellationToken ct = default)
        {
            IReadOnlyList<ApiKeyRecord> records = Records.Where(x => x.KeyPreview == preview).Select(Clone).ToList();
            return Task.FromResult(records);
        }

        public Task TouchLastUsedAsync(int id, DateTimeOffset usedAt, CancellationToken ct = default)
        {
            var record = Records.SingleOrDefault(x => x.Id == id);
            if (record != null)
            {
                record.LastUsedDate = usedAt;
            }

            return Task.CompletedTask;
        }

        private static ApiKeyRecord Clone(ApiKeyRecord record)
        {
            return new ApiKeyRecord
            {
                Id = record.Id,
                Guid = record.Guid,
                OwnerType = record.OwnerType,
                OwnerId = record.OwnerId,
                Name = record.Name,
                KeyHash = [.. record.KeyHash],
                KeyPreview = record.KeyPreview,
                SecretHash = record.SecretHash == null ? null : [.. record.SecretHash],
                SecurityLevel = record.SecurityLevel,
                Status = record.Status,
                ExpiryDate = record.ExpiryDate,
                LastUsedDate = record.LastUsedDate,
                RotatedFromKeyGuid = record.RotatedFromKeyGuid
            };
        }
    }
}

