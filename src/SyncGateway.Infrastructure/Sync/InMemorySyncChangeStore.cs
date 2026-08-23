using System.Collections.Concurrent;
using SyncGateway.Application.Abstractions;
using SyncGateway.Domain.Sync;

namespace SyncGateway.Infrastructure.Sync;

public sealed class InMemorySyncChangeStore : ISyncChangeStore
{
    private readonly ConcurrentDictionary<string, TenantStore> stores = new();

    public Task<SyncChange?> GetAsync(
        string tenantId,
        string entityType,
        string entityId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var key = BuildKey(entityType, entityId);
        SyncChange? result = stores.TryGetValue(tenantId, out var store)
            && store.Entities.TryGetValue(key, out var storedChange)
                ? storedChange
                : null;
        return Task.FromResult(result);
    }

    public Task UpsertAsync(string tenantId, SyncChange change, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var store = stores.GetOrAdd(tenantId, _ => new TenantStore());
        store.Entities[BuildKey(change.EntityType, change.EntityId)] = change;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(
        string tenantId,
        string entityType,
        string entityId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (stores.TryGetValue(tenantId, out var store))
        {
            store.Entities.TryRemove(BuildKey(entityType, entityId), out _);
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<SyncChange>> GetAllAsync(
        string tenantId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyCollection<SyncChange> changes = stores.TryGetValue(tenantId, out var store)
            ? store.Entities.Values.ToArray()
            : Array.Empty<SyncChange>();
        return Task.FromResult(changes);
    }

    private static string BuildKey(string entityType, string entityId) => $"{entityType}:{entityId}";

    private sealed class TenantStore
    {
        public ConcurrentDictionary<string, SyncChange> Entities { get; } = new();
    }
}
