using SyncGateway.Application.Sync;
using SyncGateway.Domain.Sync;
using SyncGateway.Infrastructure.Sync;

namespace SyncGateway.Tests.Sync;

public sealed class InMemorySyncEngineTests
{
    [Fact]
    public async Task Push_then_pull_returns_the_change_for_the_same_tenant()
    {
        var engine = CreateEngine();
        var tenant = $"tenant-{Guid.NewGuid():N}";
        var change = Change("order", "order-1", "v1");

        var push = await engine.SyncAsync(Command(tenant, SyncDirection.Push, change), default);
        var pull = await engine.SyncAsync(Command(tenant, SyncDirection.Pull), default);

        Assert.Equal(1, push.AppliedChanges);
        Assert.Empty(push.OutboundChanges);
        Assert.Equal(new[] { change }, pull.OutboundChanges);
    }

    [Fact]
    public async Task Stores_are_isolated_by_tenant()
    {
        var engine = CreateEngine();
        var suffix = Guid.NewGuid().ToString("N");
        await engine.SyncAsync(Command($"tenant-a-{suffix}", SyncDirection.Push,
            Change("order", "order-1", "v1")), default);

        var otherTenant = await engine.SyncAsync(
            Command($"tenant-b-{suffix}", SyncDirection.Pull), default);

        Assert.Empty(otherTenant.OutboundChanges);
    }

    [Fact]
    public async Task Conflicting_upsert_keeps_the_existing_version()
    {
        var engine = CreateEngine();
        var tenant = $"tenant-{Guid.NewGuid():N}";
        await engine.SyncAsync(Command(tenant, SyncDirection.Push,
            Change("order", "order-1", "v1")), default);

        var conflict = await engine.SyncAsync(Command(tenant, SyncDirection.Push,
            Change("order", "order-1", "v2")), default);
        var pull = await engine.SyncAsync(Command(tenant, SyncDirection.Pull), default);

        Assert.Equal(0, conflict.AppliedChanges);
        Assert.Collection(conflict.Conflicts, item =>
        {
            Assert.Equal("v2", item.ClientVersion);
            Assert.Equal("v1", item.ServerVersion);
            Assert.Equal("server_wins", item.Resolution);
        });
        Assert.Equal("v1", Assert.Single(pull.OutboundChanges).Version);
    }

    [Fact]
    public async Task Engine_instances_do_not_share_hidden_storage_state()
    {
        var writer = CreateEngine();
        var reader = CreateEngine();
        var tenant = $"tenant-{Guid.NewGuid():N}";
        await writer.SyncAsync(Command(tenant, SyncDirection.Push,
            Change("order", "order-1", "v1")), default);

        var pull = await reader.SyncAsync(Command(tenant, SyncDirection.Pull), default);

        Assert.Empty(pull.OutboundChanges);
    }

    [Fact]
    public async Task Delete_removes_an_existing_change_and_counts_as_applied()
    {
        var engine = CreateEngine();
        var tenant = $"tenant-{Guid.NewGuid():N}";
        await engine.SyncAsync(Command(tenant, SyncDirection.Push,
            Change("order", "order-1", "v1")), default);
        var deletion = Change("order", "order-1", "v2") with
        {
            Operation = ChangeOperation.Delete
        };

        var result = await engine.SyncAsync(
            Command(tenant, SyncDirection.BiDirectional, deletion), default);

        Assert.Equal(1, result.AppliedChanges);
        Assert.Empty(result.Conflicts);
        Assert.Empty(result.OutboundChanges);
    }

    private static SyncCommand Command(
        string tenant,
        SyncDirection direction,
        params SyncChange[] changes) =>
        new(tenant, "client-1", null, changes, direction, "correlation-1");

    private static InMemorySyncEngine CreateEngine() =>
        new(new InMemorySyncChangeStore());

    private static SyncChange Change(string type, string id, string version) => new(
        type, id, version, new Dictionary<string, object?>(), ChangeOperation.Upsert,
        DateTimeOffset.UnixEpoch);
}
