using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BrighterTools.ApiKeys.AspNetCore;

/// <summary>
/// Defines operations for API Key Principal Factory.
/// </summary>
public interface IApiKeyPrincipalFactory
{
    /// <summary>
    /// Creates the create Principal Async.
    /// </summary>
    /// <param name="result">The result value.</param>
    /// <param name="schemeName">The schemeName value.</param>
    /// <param name="httpContext">The httpContext value.</param>
    /// <param name="ct">The ct value.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    Task<ClaimsPrincipal?> CreatePrincipalAsync(ApiKeyAuthenticationResult result, string schemeName, HttpContext httpContext, CancellationToken ct = default);
}

