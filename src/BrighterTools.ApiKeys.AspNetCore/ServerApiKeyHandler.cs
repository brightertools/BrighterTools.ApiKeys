using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BrighterTools.ApiKeys.AspNetCore;

/// <summary>
/// Handles Server API Key.
/// </summary>
public sealed class ServerApiKeyHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IApiKeyService _apiKeyService;
    private readonly IApiKeyPrincipalFactory _principalFactory;
    private readonly IApiKeyRequestContextApplier _requestContextApplier;
    private readonly ApiKeyOptions _options;
    private readonly ILogger<ServerApiKeyHandler> _appLogger;

    /// <summary>
    /// Executes Server API Key Handler.
    /// </summary>
    public ServerApiKeyHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> schemeOptions,
        ILoggerFactory loggerFactory,
        UrlEncoder encoder,
        IApiKeyService apiKeyService,
        IApiKeyPrincipalFactory principalFactory,
        IApiKeyRequestContextApplier requestContextApplier,
        IOptions<ApiKeyOptions> options,
        ILogger<ServerApiKeyHandler> appLogger)
        : base(schemeOptions, loggerFactory, encoder)
    {
        _apiKeyService = apiKeyService;
        _principalFactory = principalFactory;
        _requestContextApplier = requestContextApplier;
        _options = options.Value;
        _appLogger = appLogger;
    }

    /// <summary>
    /// Handles the handle Authenticate Async.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var apiKey = Request.Headers[_options.ServerKeyHeaderName].FirstOrDefault();
        var secret = Request.Headers[_options.ServerSecretHeaderName].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(secret))
        {
            return AuthenticateResult.Fail("Missing Server API or Secret Key.");
        }

        var result = await _apiKeyService.AuthenticateServerKeyAsync(apiKey, secret, Context.RequestAborted);
        if (result == null)
        {
            return AuthenticateResult.Fail("Invalid API keys");
        }

        try
        {
            var principal = await _principalFactory.CreatePrincipalAsync(result, Scheme.Name, Context, Context.RequestAborted);
            if (principal == null)
            {
                return AuthenticateResult.Fail("Unable to create principal for API key.");
            }

            await _requestContextApplier.ApplyAsync(result, principal, Context, Context.RequestAborted);
            return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
        }
        catch (Exception ex)
        {
            _appLogger.LogError(ex, "Server API key authorization error for key id {ApiKeyId}", result.Guid);
            return AuthenticateResult.Fail("Authorization Error");
        }
    }
}

