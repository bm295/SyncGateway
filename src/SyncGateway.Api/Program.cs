using SyncGateway.Api.Contracts;
using SyncGateway.Api.Domain;
using SyncGateway.Api.Infrastructure;
using SyncGateway.Api.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ISyncEngine, InMemorySyncEngine>();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

builder.Services.AddOptions<GatewayOptions>()
    .Bind(builder.Configuration.GetSection(GatewayOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

var app = builder.Build();

app.UseMiddleware<ApiKeyMiddleware>();

app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "sync-gateway" }));

app.MapPost("/api/sync/{tenantId}/{clientId}", async (
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

app.Run();
