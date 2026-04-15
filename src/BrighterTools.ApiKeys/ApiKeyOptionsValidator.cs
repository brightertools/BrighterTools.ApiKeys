using Microsoft.Extensions.Options;

namespace BrighterTools.ApiKeys;

/// <summary>
/// Represents API Key Options Validator.
/// </summary>
public sealed class ApiKeyOptionsValidator : IValidateOptions<ApiKeyOptions>
{
    /// <summary>
    /// Validates the operation.
    /// </summary>
    public ValidateOptionsResult Validate(string? name, ApiKeyOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.Pepper))
        {
            failures.Add($"{ApiKeyDefaults.ConfigurationSectionName}:Pepper is required.");
        }

        if (string.IsNullOrWhiteSpace(options.ClientHeaderName))
        {
            failures.Add("ClientHeaderName is required.");
        }

        if (string.IsNullOrWhiteSpace(options.ServerKeyHeaderName))
        {
            failures.Add("ServerKeyHeaderName is required.");
        }

        if (string.IsNullOrWhiteSpace(options.ServerSecretHeaderName))
        {
            failures.Add("ServerSecretHeaderName is required.");
        }

        if (string.IsNullOrWhiteSpace(options.ClientSchemeName))
        {
            failures.Add("ClientSchemeName is required.");
        }

        if (string.IsNullOrWhiteSpace(options.ServerSchemeName))
        {
            failures.Add("ServerSchemeName is required.");
        }

        if (string.IsNullOrWhiteSpace(options.PolicySchemeName))
        {
            failures.Add("PolicySchemeName is required.");
        }

        if (options.ServerKeyPreviewLength <= 0)
        {
            failures.Add("ServerKeyPreviewLength must be greater than zero.");
        }

        if (options.DefaultRotationGraceDays <= 0)
        {
            failures.Add("DefaultRotationGraceDays must be greater than zero.");
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }
}

