using SyncGateway.Domain.Sync;

namespace SyncGateway.Application.SyncPlans;

public sealed record SyncPlanContext(
    string TenantId,
    string ClientId,
    string? AppVersion,
    string? Platform,
    IReadOnlyCollection<string> Capabilities);

public sealed record SyncPlan(
    string Version,
    DateTimeOffset EffectiveFromUtc,
    int TtlSeconds,
    SyncDirection Direction,
    SyncInterval Interval,
    SyncBatchSize BatchSize,
    SyncConflictPolicy ConflictPolicy,
    IReadOnlyCollection<SyncPlanResource> Resources,
    SyncPlanFilters Filters,
    SyncCursorPolicy CursorPolicy);

public sealed record SyncInterval(
    int ForegroundSeconds,
    int BackgroundSeconds,
    bool OnReconnect,
    bool OnAppStart);

public sealed record SyncBatchSize(
    int MaxChangesPerRequest,
    int MaxPayloadBytes);

public sealed record SyncConflictPolicy(
    string DefaultPolicy,
    IReadOnlyDictionary<string, string> PerResource);

public sealed record SyncPlanResource(
    string EntityType,
    bool Enabled,
    IReadOnlyCollection<ChangeOperation> OperationTypes,
    SyncResourcePull Pull,
    SyncResourcePush Push);

public sealed record SyncResourcePull(string? Filter);

public sealed record SyncResourcePush(bool AllowCreate, bool AllowUpdate, bool AllowDelete);

public sealed record SyncPlanFilters(string? Global, IReadOnlyDictionary<string, string> ByEntity);

public sealed record SyncCursorPolicy(string Scope, bool ResetOnVersionChange);
