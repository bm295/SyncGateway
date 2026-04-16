using SyncGateway.Api.Contracts;

namespace SyncGateway.Api.Domain;

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

public interface ISyncEngine
{
    Task<SyncResult> SyncAsync(SyncCommand command, CancellationToken cancellationToken);
}
