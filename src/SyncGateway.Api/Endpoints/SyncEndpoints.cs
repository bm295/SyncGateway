using SyncGateway.Api.Contracts;
using SyncGateway.Api.Mapping;
using SyncGateway.Api.Validation;
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
            SyncRequestValidator validator,
            ISyncEngine engine,
            CancellationToken cancellationToken) =>
        {
            var validationErrors = validator.Validate(request);
            if (validationErrors.Count > 0)
            {
                return Results.BadRequest(new { errors = validationErrors });
            }

            var command = new SyncCommand(
                TenantId: tenantId,
                ClientId: clientId,
                Cursor: request.Cursor,
                Changes: request.Changes,
                Direction: request.Direction,
                CorrelationId: request.CorrelationId);

            var result = await engine.SyncAsync(command, cancellationToken);

            return Results.Ok(SyncResponseMapper.Map(result));
        });

        return endpoints;
    }
}
