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

public sealed record SyncChange(
    string EntityType,
    string EntityId,
    string Version,
    IReadOnlyDictionary<string, object?> Payload,
    ChangeOperation Operation,
    DateTimeOffset ChangedAtUtc);

public sealed record SyncConflict(
    string EntityType,
    string EntityId,
    string ClientVersion,
    string ServerVersion,
    string Resolution);

public enum SyncDirection
{
    Push,
    Pull,
    BiDirectional
}

public enum ChangeOperation
{
    Upsert,
    Delete
}
