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
        Assert.Same(second, await store.GetAsync(
            "tenant-1", second.EntityType, second.EntityId, default));
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

    private static SyncChange Change(string type, string id) => new(
        type, id, "v1", new Dictionary<string, object?>(), ChangeOperation.Upsert,
        DateTimeOffset.UnixEpoch);
}
