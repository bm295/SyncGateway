using SyncGateway.Api.Contracts;
using SyncGateway.Domain.Sync;

namespace SyncGateway.Api.Validation;

public sealed class SyncRequestValidator
{
    public IReadOnlyList<string> Validate(SyncRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.CorrelationId))
        {
            errors.Add("CorrelationId is required.");
        }

        if (request.Changes is null)
        {
            errors.Add("Changes is required.");
            return errors;
        }

        foreach (var change in request.Changes)
        {
            ValidateChange(change, errors);
        }

        return errors;
    }

    private static void ValidateChange(SyncChange change, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(change.EntityType))
        {
            errors.Add("Change.EntityType is required.");
        }

        if (string.IsNullOrWhiteSpace(change.EntityId))
        {
            errors.Add("Change.EntityId is required.");
        }

        if (string.IsNullOrWhiteSpace(change.Version))
        {
            errors.Add("Change.Version is required.");
        }
    }
}
