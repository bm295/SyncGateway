using SyncGateway.Application.Abstractions;
using SyncGateway.Application.Sync;
using SyncGateway.Domain.Sync;

namespace SyncGateway.Infrastructure.Sync;

public sealed class InMemorySyncEngine : ISyncEngine
{
    private readonly ISyncChangeStore store;

    public InMemorySyncEngine(ISyncChangeStore store)
    {
        this.store = store;
    }

    public async Task<SyncResult> SyncAsync(SyncCommand command, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var conflicts = new List<SyncConflict>();
        var appliedChanges = 0;

        if (command.Direction is SyncDirection.Push or SyncDirection.BiDirectional)
        {
            foreach (var change in command.Changes)
            {
                var existing = await store.GetAsync(
                    command.TenantId, change.EntityType, change.EntityId, cancellationToken);
                if (existing is not null
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
                    await store.DeleteAsync(
                        command.TenantId, change.EntityType, change.EntityId, cancellationToken);
                }
                else
                {
                    await store.UpsertAsync(command.TenantId, change, cancellationToken);
                }

                appliedChanges++;
            }
        }

        var outboundChanges = command.Direction is SyncDirection.Pull or SyncDirection.BiDirectional
            ? await store.GetAllAsync(command.TenantId, cancellationToken)
            : Array.Empty<SyncChange>();

        var serverTimestampUtc = DateTimeOffset.UtcNow;
        var nextCursor = serverTimestampUtc.ToUnixTimeMilliseconds().ToString();

        return new SyncResult(
            CorrelationId: command.CorrelationId,
            NextCursor: nextCursor,
            AppliedChanges: appliedChanges,
            OutboundChanges: outboundChanges,
            Conflicts: conflicts,
            ServerTimestampUtc: serverTimestampUtc);
    }
}
