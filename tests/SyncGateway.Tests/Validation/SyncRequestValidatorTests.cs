using SyncGateway.Api.Contracts;
using SyncGateway.Api.Validation;
using SyncGateway.Domain.Sync;

namespace SyncGateway.Tests.Validation;

public sealed class SyncRequestValidatorTests
{
    private readonly SyncRequestValidator validator = new();

    [Fact]
    public void Valid_request_has_no_errors()
    {
        var request = new SyncRequest(null, SyncDirection.Push, "correlation-1", new[]
        {
            Change("order", "order-1", "v1")
        });

        Assert.Empty(validator.Validate(request));
    }

    [Fact]
    public void Missing_request_and_change_identifiers_are_reported()
    {
        var request = new SyncRequest(null, SyncDirection.Push, " ", new[]
        {
            Change("", "", "")
        });

        var errors = validator.Validate(request);

        Assert.Equal(new[]
        {
            "CorrelationId is required.",
            "Change.EntityType is required.",
            "Change.EntityId is required.",
            "Change.Version is required."
        }, errors);
    }

    [Fact]
    public void Missing_changes_collection_is_reported_without_throwing()
    {
        var request = new SyncRequest(null, SyncDirection.Push, "correlation-1", null!);

        Assert.Equal(new[] { "Changes is required." }, validator.Validate(request));
    }

    private static SyncChange Change(string type, string id, string version) => new(
        type, id, version, new Dictionary<string, object?>(), ChangeOperation.Upsert,
        DateTimeOffset.UnixEpoch);
}
