using SyncGateway.Api.Mapping;
using SyncGateway.Application.Sync;
using SyncGateway.Domain.Sync;

namespace SyncGateway.Tests.Mapping;

public sealed class SyncResponseMapperTests
{
    [Fact]
    public void Map_preserves_the_application_result()
    {
        var timestamp = new DateTimeOffset(2026, 8, 23, 10, 0, 0, TimeSpan.Zero);
        var outbound = new[] { Change("order", "order-1", "v2") };
        var conflicts = new[] { new SyncConflict("order", "order-2", "v1", "v2", "server_wins") };
        var result = new SyncResult("correlation-1", "cursor-2", 3, outbound, conflicts, timestamp);

        var response = SyncResponseMapper.Map(result);

        Assert.Equal(result.CorrelationId, response.CorrelationId);
        Assert.Equal(result.NextCursor, response.NextCursor);
        Assert.Equal(result.AppliedChanges, response.AppliedChanges);
        Assert.Same(outbound, response.OutboundChanges);
        Assert.Same(conflicts, response.Conflicts);
        Assert.Equal(timestamp, response.ServerTimestampUtc);
    }

    private static SyncChange Change(string type, string id, string version) => new(
        type, id, version, new Dictionary<string, object?>(), ChangeOperation.Upsert,
        DateTimeOffset.UnixEpoch);
}
