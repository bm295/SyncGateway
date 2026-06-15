using SyncGateway.Domain.Sync;

namespace SyncGateway.Application.Sync;

public sealed record SyncCommand(
    string TenantId,
    string ClientId,
    string? Cursor,
    IReadOnlyCollection<SyncChange> Changes,
    SyncDirection Direction,
    string CorrelationId);

public sealed record SyncResult(
    string CorrelationId,
    string NextCursor,
    int AppliedChanges,
    IReadOnlyCollection<SyncChange> OutboundChanges,
    IReadOnlyCollection<SyncConflict> Conflicts,
    DateTimeOffset ServerTimestampUtc);
