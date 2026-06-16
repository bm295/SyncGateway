using SyncGateway.Domain.Sync;

namespace SyncGateway.Application.Abstractions;

public interface IConflictPolicyResolver
{
    Task<ConflictResolutionResult> ResolveAsync(
        ConflictResolutionContext context,
        CancellationToken cancellationToken);
}

public sealed record ConflictResolutionContext(
    string TenantId,
    string ClientId,
    string PolicyName,
    SyncChange IncomingChange,
    SyncChange ExistingChange);

public sealed record ConflictResolutionResult(
    ConflictResolutionDecision Decision,
    SyncChange? ResolvedChange,
    SyncConflict? Conflict);

public enum ConflictResolutionDecision
{
    ApplyIncomingChange,
    KeepExistingChange,
    RejectAsConflict
}
