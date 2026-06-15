using SyncGateway.Api.Contracts;
using SyncGateway.Application.Abstractions;
using SyncGateway.Application.Sync;

namespace SyncGateway.Api.Endpoints;

public static class SyncEndpoints
{
    public static IEndpointRouteBuilder MapSyncEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/sync/{tenantId}/{clientId}", async (
            string tenantId,
            string clientId,
            SyncRequest request,
            ISyncEngine engine,
            CancellationToken cancellationToken) =>
        {
            var command = new SyncCommand(
                TenantId: tenantId,
                ClientId: clientId,
                Cursor: request.Cursor,
                Changes: request.Changes,
                Direction: request.Direction,
                CorrelationId: request.CorrelationId);

            var result = await engine.SyncAsync(command, cancellationToken);

            return Results.Ok(new SyncResponse(
                CorrelationId: result.CorrelationId,
                NextCursor: result.NextCursor,
                AppliedChanges: result.AppliedChanges,
                OutboundChanges: result.OutboundChanges,
                Conflicts: result.Conflicts,
                ServerTimestampUtc: result.ServerTimestampUtc));
        });

        return endpoints;
    }
}
