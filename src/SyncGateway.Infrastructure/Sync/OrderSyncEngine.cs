using SyncGateway.Application.Abstractions;
using SyncGateway.Application.Sync;
using SyncGateway.Domain.Sync;

namespace SyncGateway.Infrastructure.Sync;

public sealed class OrderSyncEngine : ISyncEngine
{
    private const string EntityType = "order";

    private readonly IOrderRepository orderRepository;

    public OrderSyncEngine(IOrderRepository orderRepository)
    {
        this.orderRepository = orderRepository;
    }

    public async Task<SyncResult> SyncAsync(SyncCommand command, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var orderChanges = command.Changes
            .Where(change => string.Equals(change.EntityType, EntityType, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        var conflicts = new List<SyncConflict>();
        var appliedChanges = 0;

        if (command.Direction is SyncDirection.Push or SyncDirection.BiDirectional)
        {
            foreach (var change in orderChanges)
            {
                var existingOrder = await orderRepository.GetByIdAsync(command.TenantId, change.EntityId, cancellationToken);

                if (existingOrder is not null
                    && existingOrder.Version != change.Version
                    && change.Operation == ChangeOperation.Upsert)
                {
                    conflicts.Add(new SyncConflict(
                        EntityType,
                        change.EntityId,
                        change.Version,
                        existingOrder.Version,
                        "server_wins"));
                    continue;
                }

                if (change.Operation == ChangeOperation.Delete)
                {
                    await orderRepository.DeleteAsync(command.TenantId, change.EntityId, cancellationToken);
                }
                else
                {
                    await orderRepository.UpsertAsync(MapToOrder(command.TenantId, change), cancellationToken);
                }

                appliedChanges++;
            }
        }

        var outboundChanges = command.Direction is SyncDirection.Pull or SyncDirection.BiDirectional
            ? await GetOutboundChangesAsync(command.TenantId, orderChanges, cancellationToken)
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

    private async Task<IReadOnlyCollection<SyncChange>> GetOutboundChangesAsync(
        string tenantId,
        IReadOnlyCollection<SyncChange> requestedChanges,
        CancellationToken cancellationToken)
    {
        var outboundChanges = new List<SyncChange>();

        foreach (var entityId in requestedChanges.Select(change => change.EntityId).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var order = await orderRepository.GetByIdAsync(tenantId, entityId, cancellationToken);

            if (order is not null)
            {
                outboundChanges.Add(MapToChange(order));
            }
        }

        return outboundChanges;
    }

    private static Order MapToOrder(string tenantId, SyncChange change)
    {
        return new Order(
            Id: change.EntityId,
            TenantId: tenantId,
            Status: GetPayloadString(change, "status") ?? "unknown",
            Version: change.Version,
            UpdatedAtUtc: change.ChangedAtUtc);
    }

    private static SyncChange MapToChange(Order order)
    {
        return new SyncChange(
            EntityType: EntityType,
            EntityId: order.Id,
            Version: order.Version,
            Payload: new Dictionary<string, object?>
            {
                ["tenantId"] = order.TenantId,
                ["status"] = order.Status
            },
            Operation: ChangeOperation.Upsert,
            ChangedAtUtc: order.UpdatedAtUtc);
    }

    private static string? GetPayloadString(SyncChange change, string key)
    {
        return change.Payload.TryGetValue(key, out var value) ? value?.ToString() : null;
    }
}
