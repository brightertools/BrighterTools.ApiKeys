using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BrighterTools.ApiKeys.AspNetCore;

/// <summary>
/// Defines operations for API Key Request Context Applier.
/// </summary>
public interface IApiKeyRequestContextApplier
{
    /// <summary>
    /// Applies the apply Async.
    /// </summary>
    /// <param name="result">The result value.</param>
    /// <param name="principal">The principal value.</param>
    /// <param name="httpContext">The httpContext value.</param>
    /// <param name="ct">The ct value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ApplyAsync(ApiKeyAuthenticationResult result, ClaimsPrincipal principal, HttpContext httpContext, CancellationToken ct = default);
}

