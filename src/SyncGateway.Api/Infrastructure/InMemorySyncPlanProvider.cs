using SyncGateway.Api.Contracts;
using SyncGateway.Api.Domain;

namespace SyncGateway.Api.Infrastructure;

public sealed class InMemorySyncPlanProvider : ISyncPlanProvider
{
    public Task<SyncPlanResponse?> GetPlanAsync(SyncPlanContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var plan = new SyncPlanResponse(
            Version: "2026.04.17.1",
            EffectiveFromUtc: DateTimeOffset.Parse("2026-04-17T00:00:00Z"),
            TtlSeconds: 86400,
            Direction: SyncDirection.BiDirectional,
            Interval: new SyncInterval(
                ForegroundSeconds: 30,
                BackgroundSeconds: 300,
                OnReconnect: true,
                OnAppStart: true),
            BatchSize: new SyncBatchSize(
                MaxChangesPerRequest: 200,
                MaxPayloadBytes: 524288),
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
                    Push: new SyncResourcePush(
                        AllowCreate: true,
                        AllowUpdate: true,
                        AllowDelete: false)),
                new SyncPlanResource(
                    EntityType: "note",
                    Enabled: true,
                    OperationTypes: new[] { ChangeOperation.Upsert, ChangeOperation.Delete },
                    Pull: new SyncResourcePull("updatedAt >= now()-P30D"),
                    Push: new SyncResourcePush(
                        AllowCreate: true,
                        AllowUpdate: true,
                        AllowDelete: true))
            },
            Filters: new SyncPlanFilters(
                Global: $"tenantId = '{context.TenantId}'",
                ByEntity: new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["order"] = "region IN ['us-east','us-west']"
                }),
            CursorPolicy: new SyncCursorPolicy(
                Scope: "per_client_per_entity",
                ResetOnVersionChange: false));

        return Task.FromResult<SyncPlanResponse?>(plan);
    }
}
