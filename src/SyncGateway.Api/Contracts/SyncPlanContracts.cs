namespace SyncGateway.Api.Contracts;

public sealed record SyncPlanRequest(
    string? AppVersion,
    string? Platform,
    string? Capabilities);
