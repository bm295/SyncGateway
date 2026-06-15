namespace SyncGateway.Domain.Sync;

public sealed record Order(
    string Id,
    string TenantId,
    string Status,
    string Version,
    DateTimeOffset UpdatedAtUtc);
