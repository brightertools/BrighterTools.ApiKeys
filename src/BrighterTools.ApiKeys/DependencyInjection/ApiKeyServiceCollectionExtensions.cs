using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace BrighterTools.ApiKeys.DependencyInjection;

/// <summary>
/// Provides extension methods for API Key Service Collection.
/// </summary>
public static class ApiKeyServiceCollectionExtensions
{
    /// <summary>
    /// Adds Brighter Tools API Keys.
    /// </summary>
    public static IServiceCollection AddBrighterToolsApiKeys(this IServiceCollection services, IConfiguration configuration, string sectionName = ApiKeyDefaults.ConfigurationSectionName)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<ApiKeyOptions>()
            .Bind(configuration.GetSection(sectionName))
            .ValidateOnStart();

        services.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<ApiKeyOptions>, ApiKeyOptionsValidator>());
        services.TryAddScoped<IApiKeyUsageTracker, NoopApiKeyUsageTracker>();
        services.TryAddScoped<IApiKeyService, ApiKeyService>();
        services.TryAddScoped<IApiKeyOrchestrator, ApiKeyOrchestrator>();
        return services;
    }
}


