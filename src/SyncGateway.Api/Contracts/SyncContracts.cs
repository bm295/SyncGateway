using SyncGateway.Domain.Sync;

namespace SyncGateway.Api.Contracts;

public sealed record SyncRequest(
    string? Cursor,
    SyncDirection Direction,
    string CorrelationId,
    IReadOnlyCollection<SyncChange> Changes);

public sealed record SyncResponse(
    string CorrelationId,
    string NextCursor,
    int AppliedChanges,
    IReadOnlyCollection<SyncChange> OutboundChanges,
    IReadOnlyCollection<SyncConflict> Conflicts,
    DateTimeOffset ServerTimestampUtc);
