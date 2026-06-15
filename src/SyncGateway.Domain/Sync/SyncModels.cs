namespace SyncGateway.Domain.Sync;

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
