using BrighterTools.ApiKeys.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;

namespace BrighterTools.ApiKeys.AspNetCore.DependencyInjection;

/// <summary>
/// Provides extension methods for API Key Authentication Builder.
/// </summary>
public static class ApiKeyAuthenticationBuilderExtensions
{
    /// <summary>
    /// Adds Brighter Tools API Key Authentication.
    /// </summary>
    public static AuthenticationBuilder AddBrighterToolsApiKeyAuthentication(this AuthenticationBuilder builder, IConfiguration configuration, string sectionName = ApiKeyDefaults.ConfigurationSectionName)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configuration);

        builder.Services.AddBrighterToolsApiKeys(configuration, sectionName);

        var options = configuration.GetSection(sectionName).Get<ApiKeyOptions>() ?? new ApiKeyOptions();

        return builder
            .AddScheme<AuthenticationSchemeOptions, ClientApiKeyHandler>(options.ClientSchemeName, "Client API Key", _ => { })
            .AddScheme<AuthenticationSchemeOptions, ServerApiKeyHandler>(options.ServerSchemeName, "Server API Key + Secret", _ => { })
            .AddPolicyScheme(options.PolicySchemeName, "Dynamic API Key Authentication Scheme", policy =>
            {
                policy.ForwardDefaultSelector = context =>
                {
                    if (context.Request.Headers.ContainsKey(options.ServerKeyHeaderName) && context.Request.Headers.ContainsKey(options.ServerSecretHeaderName))
                    {
                        return options.ServerSchemeName;
                    }

                    if (context.Request.Headers.ContainsKey(options.ClientHeaderName))
                    {
                        return options.ClientSchemeName;
                    }

                    return string.IsNullOrWhiteSpace(options.FallbackSchemeName) ? null : options.FallbackSchemeName;
                };
            });
    }
}

