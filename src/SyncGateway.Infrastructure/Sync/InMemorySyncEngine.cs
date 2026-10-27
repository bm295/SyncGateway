using System.Globalization;
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

        var sequence = 0L;
        if (command.Cursor is not null &&
            (!long.TryParse(command.Cursor, NumberStyles.None, CultureInfo.InvariantCulture, out sequence) || sequence < 0))
            throw new ArgumentException("Cursor must be a non-negative sequence.", nameof(command));

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

                await store.UpsertAsync(command.TenantId, change, cancellationToken);

                appliedChanges++;
            }
        }

        var batch = command.Direction is SyncDirection.Pull or SyncDirection.BiDirectional
            ? await store.ReadChangesAsync(command.TenantId, sequence, 200, cancellationToken)
            : new SyncChangeBatch(sequence, Array.Empty<SyncChange>());
        var outboundChanges = batch.Changes;
        var serverTimestampUtc = DateTimeOffset.UtcNow;
        var nextCursor = batch.NextSequence.ToString(CultureInfo.InvariantCulture);

        return new SyncResult(
            CorrelationId: command.CorrelationId,
            NextCursor: nextCursor,
            AppliedChanges: appliedChanges,
            OutboundChanges: outboundChanges,
            Conflicts: conflicts,
            ServerTimestampUtc: serverTimestampUtc);
    }
}
