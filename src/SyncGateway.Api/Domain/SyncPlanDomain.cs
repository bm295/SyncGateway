using SyncGateway.Api.Contracts;

namespace SyncGateway.Api.Domain;

public sealed record SyncPlanContext(
    string TenantId,
    string ClientId,
    string? AppVersion,
    string? Platform,
    IReadOnlyCollection<string> Capabilities);

public interface ISyncPlanProvider
{
    Task<SyncPlanResponse?> GetPlanAsync(SyncPlanContext context, CancellationToken cancellationToken);
}
