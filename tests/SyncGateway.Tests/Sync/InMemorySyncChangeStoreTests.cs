using SyncGateway.Domain.Sync;
using SyncGateway.Infrastructure.Sync;

namespace SyncGateway.Tests.Sync;

public sealed class InMemorySyncChangeStoreTests
{
    [Fact]
    public async Task Delete_only_removes_the_target_entity()
    {
        var store = new InMemorySyncChangeStore();
        var first = Change("order", "order-1");
        var second = Change("note", "note-1");
        await store.UpsertAsync("tenant-1", first, default);
        await store.UpsertAsync("tenant-1", second, default);

        await store.DeleteAsync("tenant-1", first.EntityType, first.EntityId, default);

        Assert.Null(await store.GetAsync("tenant-1", first.EntityType, first.EntityId, default));
        var remaining = await store.GetAsync("tenant-1", second.EntityType, second.EntityId, default);
        Assert.NotNull(remaining);
        Assert.Equal(second, remaining with { Payload = second.Payload });
    }

    [Fact]
    public async Task Operations_honor_cancellation()
    {
        var store = new InMemorySyncChangeStore();
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            store.GetAllAsync("tenant-1", cancellation.Token));
    }

    [Fact]
    public async Task Concurrent_writes_remain_readable_after_each_checkpoint()
    {
        var store = new InMemorySyncChangeStore();
        var writers = Enumerable.Range(0, 500).Select(i => Task.Run(() =>
            store.UpsertAsync("tenant", Change("order", i.ToString()), default))).ToArray();
        long cursor = 0;
        var ids = new List<string>();
        while (!Task.WhenAll(writers).IsCompleted)
        {
            var batch = await store.ReadChangesAsync("tenant", cursor, 17, default);
            cursor = batch.NextSequence;
            ids.AddRange(batch.Changes.Select(x => x.EntityId));
            await Task.Yield();
        }
        await Task.WhenAll(writers);
        while (true)
        {
            var batch = await store.ReadChangesAsync("tenant", cursor, 17, default);
            if (batch.Changes.Count == 0) break;
            cursor = batch.NextSequence;
            ids.AddRange(batch.Changes.Select(x => x.EntityId));
        }
        Assert.Equal(500, cursor);
        Assert.Equal(500, ids.Count);
        Assert.Equal(500, ids.Distinct().Count());
        Assert.Empty((await store.ReadChangesAsync("other", 0, 200, default)).Changes);
    }

    private static SyncChange Change(string type, string id) => new(
        type, id, "v1", new Dictionary<string, object?>(), ChangeOperation.Upsert,
        DateTimeOffset.UnixEpoch);
}
