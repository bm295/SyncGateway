using SyncGateway.Api.Contracts;
using SyncGateway.Application.Sync;

namespace SyncGateway.Api.Mapping;

public static class SyncResponseMapper
{
    public static SyncResponse Map(SyncResult result)
    {
        return new SyncResponse(
            CorrelationId: result.CorrelationId,
            NextCursor: result.NextCursor,
            AppliedChanges: result.AppliedChanges,
            OutboundChanges: result.OutboundChanges,
            Conflicts: result.Conflicts,
            ServerTimestampUtc: result.ServerTimestampUtc);
    }
}
