using System.Collections.Concurrent;
using SyncGateway.Application.Abstractions;
using SyncGateway.Application.Sync;
using SyncGateway.Domain.Sync;

namespace SyncGateway.Infrastructure.Sync;

public sealed class InMemorySyncEngine : ISyncEngine
{
    private static readonly ConcurrentDictionary<string, TenantStore> Stores = new();

    public Task<SyncResult> SyncAsync(SyncCommand command, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var store = Stores.GetOrAdd(command.TenantId, _ => new TenantStore());
        var conflicts = new List<SyncConflict>();
        var appliedChanges = 0;

        if (command.Direction is SyncDirection.Push or SyncDirection.BiDirectional)
        {
            foreach (var change in command.Changes)
            {
                var key = BuildKey(change.EntityType, change.EntityId);

                if (store.Entities.TryGetValue(key, out var existing)
                    && existing.Version != change.Version
                    && change.Operation == ChangeOperation.Upsert)
                {
                    conflicts.Add(new SyncConflict(
                        change.EntityType,
                        change.EntityId,
                        change.Version,
                        existing.Version,
                        "server_wins"));
                    continue;
                }

                if (change.Operation == ChangeOperation.Delete)
                {
                    store.Entities.TryRemove(key, out _);
                }
                else
                {
                    store.Entities[key] = change;
                }

                appliedChanges++;
            }
        }

        var outboundChanges = command.Direction switch
        {
            SyncDirection.Pull => store.Entities.Values.ToArray(),
            SyncDirection.BiDirectional => store.Entities.Values.ToArray(),
            _ => Array.Empty<SyncChange>()
        };

        var serverTimestampUtc = DateTimeOffset.UtcNow;
        var nextCursor = serverTimestampUtc.ToUnixTimeMilliseconds().ToString();

        return Task.FromResult(new SyncResult(
            CorrelationId: command.CorrelationId,
            NextCursor: nextCursor,
            AppliedChanges: appliedChanges,
            OutboundChanges: outboundChanges,
            Conflicts: conflicts,
            ServerTimestampUtc: serverTimestampUtc));
    }

    private static string BuildKey(string entityType, string entityId) => $"{entityType}:{entityId}";

    private sealed class TenantStore
    {
        public ConcurrentDictionary<string, SyncChange> Entities { get; } = new();
    }
}
