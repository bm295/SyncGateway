using SyncGateway.Api.Contracts;
using SyncGateway.Api.Domain;
using SyncGateway.Api.Infrastructure;
using SyncGateway.Api.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ISyncEngine, InMemorySyncEngine>();
builder.Services.AddSingleton<ISyncPlanProvider, InMemorySyncPlanProvider>();
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

app.MapGet("/api/sync-plan/{tenantId}/{clientId}", async (
    string tenantId,
    string clientId,
    string? appVersion,
    string? platform,
    string? capabilities,
    ISyncPlanProvider planProvider,
    CancellationToken cancellationToken) =>
{
    var parsedCapabilities = string.IsNullOrWhiteSpace(capabilities)
        ? Array.Empty<string>()
        : capabilities.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    var context = new SyncPlanContext(
        TenantId: tenantId,
        ClientId: clientId,
        AppVersion: appVersion,
        Platform: platform,
        Capabilities: parsedCapabilities);

    var plan = await planProvider.GetPlanAsync(context, cancellationToken);

    return plan is null ? Results.NotFound() : Results.Ok(plan);
});


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
