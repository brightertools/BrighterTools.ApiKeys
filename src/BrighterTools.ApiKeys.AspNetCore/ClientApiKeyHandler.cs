using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BrighterTools.ApiKeys.AspNetCore;

/// <summary>
/// Handles Client API Key.
/// </summary>
public sealed class ClientApiKeyHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IApiKeyService _apiKeyService;
    private readonly IApiKeyPrincipalFactory _principalFactory;
    private readonly IApiKeyRequestContextApplier _requestContextApplier;
    private readonly ApiKeyOptions _options;
    private readonly ILogger<ClientApiKeyHandler> _appLogger;

    /// <summary>
    /// Executes Client API Key Handler.
    /// </summary>
    public ClientApiKeyHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> schemeOptions,
        ILoggerFactory loggerFactory,
        UrlEncoder encoder,
        IApiKeyService apiKeyService,
        IApiKeyPrincipalFactory principalFactory,
        IApiKeyRequestContextApplier requestContextApplier,
        IOptions<ApiKeyOptions> options,
        ILogger<ClientApiKeyHandler> appLogger)
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
        var clientKey = Request.Headers[_options.ClientHeaderName].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(clientKey))
        {
            return AuthenticateResult.NoResult();
        }

        var result = await _apiKeyService.AuthenticateClientKeyAsync(clientKey, Context.RequestAborted);
        if (result == null)
        {
            return AuthenticateResult.Fail("Invalid API key");
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
            _appLogger.LogError(ex, "Client API key authorization error for key id {ApiKeyId}", result.Guid);
            return AuthenticateResult.Fail("Authorization Error");
        }
    }
}

