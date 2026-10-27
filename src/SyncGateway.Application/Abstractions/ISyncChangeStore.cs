using SyncGateway.Domain.Sync;
using SyncGateway.Application.Sync;

namespace SyncGateway.Application.Abstractions;

/// <summary>
/// Persistence boundary used by the generic synchronization engine.
/// Implementations own storage and tenant isolation concerns.
/// </summary>
public interface ISyncChangeStore
{
    // Entries must be visible in sequence order; never advance past an unpublished write.
    Task<SyncChangeBatch> ReadChangesAsync(
        string tenantId, long afterSequence, int limit, CancellationToken cancellationToken);

    Task<SyncChange?> GetAsync(
        string tenantId,
        string entityType,
        string entityId,
        CancellationToken cancellationToken);

    Task UpsertAsync(string tenantId, SyncChange change, CancellationToken cancellationToken);

    Task DeleteAsync(
        string tenantId,
        string entityType,
        string entityId,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<SyncChange>> GetAllAsync(
        string tenantId,
        CancellationToken cancellationToken);
}
