using SyncGateway.Application.Abstractions;
using SyncGateway.Application.SyncPlans;
using SyncGateway.Domain.Sync;

namespace SyncGateway.Infrastructure.Sync;

public sealed class InMemorySyncPlanProvider : ISyncPlanProvider
{
    public Task<SyncPlan?> GetPlanAsync(SyncPlanContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var plan = new SyncPlan(
            Version: "2026.04.17.1",
            EffectiveFromUtc: DateTimeOffset.Parse("2026-04-17T00:00:00Z"),
            TtlSeconds: 86400,
            Direction: SyncDirection.BiDirectional,
            Interval: new SyncInterval(30, 300, true, true),
            BatchSize: new SyncBatchSize(200, 524288),
            ConflictPolicy: new SyncConflictPolicy(
                DefaultPolicy: "server_wins",
                PerResource: new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["order"] = "server_wins",
                    ["note"] = "client_wins"
                }),
            Resources: new[]
            {
                new SyncPlanResource(
                    EntityType: "order",
                    Enabled: true,
                    OperationTypes: new[] { ChangeOperation.Upsert, ChangeOperation.Delete },
                    Pull: new SyncResourcePull("status != 'archived'"),
                    Push: new SyncResourcePush(true, true, false)),
                new SyncPlanResource(
                    EntityType: "note",
                    Enabled: true,
                    OperationTypes: new[] { ChangeOperation.Upsert, ChangeOperation.Delete },
                    Pull: new SyncResourcePull("updatedAt >= now()-P30D"),
                    Push: new SyncResourcePush(true, true, true))
            },
            Filters: new SyncPlanFilters(
                Global: $"tenantId = '{context.TenantId}'",
                ByEntity: new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["order"] = "region IN ['us-east','us-west']"
                }),
            CursorPolicy: new SyncCursorPolicy("per_client_per_entity", false));

        return Task.FromResult<SyncPlan?>(plan);
    }
}
