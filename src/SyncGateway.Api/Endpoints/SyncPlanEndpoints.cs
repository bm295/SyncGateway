using SyncGateway.Api.Contracts;
using SyncGateway.Application.Abstractions;
using SyncGateway.Application.SyncPlans;

namespace SyncGateway.Api.Endpoints;

public static class SyncPlanEndpoints
{
    public static IEndpointRouteBuilder MapSyncPlanEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/sync-plan/{tenantId}/{clientId}", async (
            string tenantId,
            string clientId,
            [AsParameters] SyncPlanRequest request,
            ISyncPlanProvider planProvider,
            CancellationToken cancellationToken) =>
        {
            var parsedCapabilities = string.IsNullOrWhiteSpace(request.Capabilities)
                ? Array.Empty<string>()
                : request.Capabilities.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var context = new SyncPlanContext(
                TenantId: tenantId,
                ClientId: clientId,
                AppVersion: request.AppVersion,
                Platform: request.Platform,
                Capabilities: parsedCapabilities);

            var plan = await planProvider.GetPlanAsync(context, cancellationToken);

            return plan is null ? Results.NotFound() : Results.Ok(plan);
        });

        return endpoints;
    }
}
