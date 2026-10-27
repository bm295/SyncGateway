using System.Collections.Concurrent;
using SyncGateway.Application.Abstractions;
using SyncGateway.Application.Sync;
using SyncGateway.Domain.Sync;

namespace SyncGateway.Infrastructure.Sync;

public sealed class InMemorySyncChangeStore : ISyncChangeStore
{
    private readonly ConcurrentDictionary<string, TenantStore> stores = new();

    public Task<SyncChange?> GetAsync(string tenantId, string entityType, string entityId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var store = stores.GetOrAdd(tenantId, _ => new TenantStore());
        lock (store)
        {
            store.Entities.TryGetValue((entityType, entityId), out var change);
            return Task.FromResult(change);
        }
    }

    public Task UpsertAsync(string tenantId, SyncChange change, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var store = stores.GetOrAdd(tenantId, _ => new TenantStore());
        lock (store)
        {
            // Publish state, sequence and feed entry together. No reserved sequence can commit late.
            var sequence = checked(store.Sequence + 1);
            var snapshot = change with { Payload = new Dictionary<string, object?>(change.Payload) };
            if (change.Operation == ChangeOperation.Delete)
                store.Entities.Remove((change.EntityType, change.EntityId));
            else
                store.Entities[(change.EntityType, change.EntityId)] = snapshot;
            store.Log.Add(new SyncChangeLog(tenantId, sequence, snapshot));
            store.Sequence = sequence;
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string tenantId, string entityType, string entityId,
        CancellationToken cancellationToken) => UpsertAsync(tenantId,
            new SyncChange(entityType, entityId, "deleted", new Dictionary<string, object?>(),
                ChangeOperation.Delete, DateTimeOffset.UtcNow), cancellationToken);

    public Task<IReadOnlyCollection<SyncChange>> GetAllAsync(string tenantId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var store = stores.GetOrAdd(tenantId, _ => new TenantStore());
        lock (store)
            return Task.FromResult<IReadOnlyCollection<SyncChange>>(store.Entities.Values.ToArray());
    }

    public Task<SyncChangeBatch> ReadChangesAsync(string tenantId, long afterSequence, int limit,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentOutOfRangeException.ThrowIfNegative(afterSequence);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(limit);
        var store = stores.GetOrAdd(tenantId, _ => new TenantStore());
        lock (store)
        {
            var entries = store.Log.Where(x => x.Sequence > afterSequence).Take(limit).ToArray();
            return Task.FromResult(new SyncChangeBatch(
                entries.Length == 0 ? afterSequence : entries[^1].Sequence,
                entries.Select(x => x.Change).ToArray()));
        }
    }

    private sealed record SyncChangeLog(string TenantId, long Sequence, SyncChange Change);

    private sealed class TenantStore
    {
        public long Sequence { get; set; }
        public Dictionary<(string Type, string Id), SyncChange> Entities { get; } = new();
        public List<SyncChangeLog> Log { get; } = new();
    }
}
