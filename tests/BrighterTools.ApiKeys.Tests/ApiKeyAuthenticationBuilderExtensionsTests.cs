using Xunit;
using BrighterTools.ApiKeys.AspNetCore;
using BrighterTools.ApiKeys.AspNetCore.DependencyInjection;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace BrighterTools.ApiKeys.Tests;

public class ApiKeyAuthenticationBuilderExtensionsTests
{
    [Fact]
    public async Task AddBrighterToolsApiKeyAuthentication_RegistersExpectedSchemes()
    {
        var services = CreateServices();
        var provider = services.BuildServiceProvider();
        var schemeProvider = provider.GetRequiredService<IAuthenticationSchemeProvider>();

        (await schemeProvider.GetSchemeAsync(ApiKeyDefaults.ClientSchemeName)).Should().NotBeNull();
        (await schemeProvider.GetSchemeAsync(ApiKeyDefaults.ServerSchemeName)).Should().NotBeNull();
        (await schemeProvider.GetSchemeAsync(ApiKeyDefaults.PolicySchemeName)).Should().NotBeNull();
    }

    [Fact]
    public void PolicyScheme_UsesServerClientOrFallbackBasedOnHeaders()
    {
        var services = CreateServices(("ApiKeySettings:FallbackSchemeName", "Bearer"));
        var provider = services.BuildServiceProvider();
        var optionsMonitor = provider.GetRequiredService<IOptionsMonitor<PolicySchemeOptions>>();
        var options = optionsMonitor.Get(ApiKeyDefaults.PolicySchemeName);

        var serverContext = new DefaultHttpContext();
        serverContext.Request.Headers[ApiKeyDefaults.ServerKeyHeaderName] = "server-key";
        serverContext.Request.Headers[ApiKeyDefaults.ServerSecretHeaderName] = "server-secret";
        options.ForwardDefaultSelector!(serverContext).Should().Be(ApiKeyDefaults.ServerSchemeName);

        var clientContext = new DefaultHttpContext();
        clientContext.Request.Headers[ApiKeyDefaults.ClientHeaderName] = "client-key";
        options.ForwardDefaultSelector!(clientContext).Should().Be(ApiKeyDefaults.ClientSchemeName);

        var fallbackContext = new DefaultHttpContext();
        options.ForwardDefaultSelector!(fallbackContext).Should().Be("Bearer");
    }

    private static ServiceCollection CreateServices(params (string Key, string Value)[] values)
    {
        var configurationValues = new Dictionary<string, string?>
        {
            ["ApiKeySettings:Pepper"] = "test-pepper",
            ["ApiKeySettings:ClientHeaderName"] = ApiKeyDefaults.ClientHeaderName,
            ["ApiKeySettings:ServerKeyHeaderName"] = ApiKeyDefaults.ServerKeyHeaderName,
            ["ApiKeySettings:ServerSecretHeaderName"] = ApiKeyDefaults.ServerSecretHeaderName,
            ["ApiKeySettings:ClientSchemeName"] = ApiKeyDefaults.ClientSchemeName,
            ["ApiKeySettings:ServerSchemeName"] = ApiKeyDefaults.ServerSchemeName,
            ["ApiKeySettings:PolicySchemeName"] = ApiKeyDefaults.PolicySchemeName
        };

        foreach (var (key, value) in values)
        {
            configurationValues[key] = value;
        }

        var configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationValues).Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddScoped<IApiKeyStore, EmptyStore>();
        services.AddScoped<IApiKeyOwnerContext, EmptyOwnerContext>();
        services.AddScoped<IApiKeyPrincipalFactory, EmptyPrincipalFactory>();
        services.AddScoped<IApiKeyRequestContextApplier, EmptyRequestContextApplier>();
        services.AddAuthentication().AddBrighterToolsApiKeyAuthentication(configuration);
        return services;
    }

    private sealed class EmptyStore : IApiKeyStore
    {
        public Task<bool> NameExistsAsync(string ownerType, string ownerId, string name, CancellationToken ct = default) => Task.FromResult(false);
        public Task CreateAsync(ApiKeyRecord record, CancellationToken ct = default) => Task.CompletedTask;
        public Task UpdateAsync(ApiKeyRecord record, CancellationToken ct = default) => Task.CompletedTask;
        public Task<ApiKeyRecord?> GetByGuidAsync(string ownerType, string ownerId, Guid keyId, CancellationToken ct = default) => Task.FromResult<ApiKeyRecord?>(null);
        public Task<IReadOnlyList<ApiKeyRecord>> FindByPreviewAsync(string preview, CancellationToken ct = default) => Task.FromResult<IReadOnlyList<ApiKeyRecord>>([]);
        public Task TouchLastUsedAsync(int id, DateTimeOffset usedAt, CancellationToken ct = default) => Task.CompletedTask;
    }

    private sealed class EmptyOwnerContext : IApiKeyOwnerContext
    {
        public Task<ApiKeyOwner?> GetCurrentOwnerAsync(CancellationToken ct = default) => Task.FromResult<ApiKeyOwner?>(null);
    }

    private sealed class EmptyPrincipalFactory : IApiKeyPrincipalFactory
    {
        public Task<ClaimsPrincipal?> CreatePrincipalAsync(ApiKeyAuthenticationResult result, string schemeName, HttpContext httpContext, CancellationToken ct = default)
            => Task.FromResult<ClaimsPrincipal?>(null);
    }

    private sealed class EmptyRequestContextApplier : IApiKeyRequestContextApplier
    {
        public Task ApplyAsync(ApiKeyAuthenticationResult result, ClaimsPrincipal principal, HttpContext httpContext, CancellationToken ct = default)
            => Task.CompletedTask;
    }
}

