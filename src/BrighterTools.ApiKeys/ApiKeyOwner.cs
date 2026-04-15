namespace BrighterTools.ApiKeys;

/// <summary>
/// Represents API Key Owner.
/// </summary>
public sealed record ApiKeyOwner(string OwnerType, string OwnerId)
{
    /// <summary>
    /// Creates the operation.
    /// </summary>
    public static ApiKeyOwner Create(string ownerType, string ownerId)
    {
        if (string.IsNullOrWhiteSpace(ownerType))
        {
            throw new ArgumentException("OwnerType is required.", nameof(ownerType));
        }

        if (string.IsNullOrWhiteSpace(ownerId))
        {
            throw new ArgumentException("OwnerId is required.", nameof(ownerId));
        }

        return new ApiKeyOwner(ownerType.Trim(), ownerId.Trim());
    }
}

