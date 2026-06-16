namespace BrighterTools.ApiKeys;

/// <summary>
/// Represents a standard operation result for API key management workflows.
/// </summary>
public sealed class ApiKeyOperationResult<T>
{
    public bool Succeeded { get; init; }

    public bool Failed => !Succeeded;

    public T? Content { get; init; }

    public IReadOnlyList<string> Messages { get; init; } = Array.Empty<string>();

    public static ApiKeyOperationResult<T> Success(T content)
    {
        return new ApiKeyOperationResult<T>
        {
            Succeeded = true,
            Content = content
        };
    }

    public static ApiKeyOperationResult<T> Fail(params string[] messages)
    {
        return new ApiKeyOperationResult<T>
        {
            Succeeded = false,
            Messages = messages.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray()
        };
    }

    public static ApiKeyOperationResult<T> Fail(IEnumerable<string> messages)
    {
        return Fail(messages.ToArray());
    }
}
