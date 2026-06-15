using SyncGateway.Api.Endpoints;
using SyncGateway.Api.Security;
using SyncGateway.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSyncGatewayInfrastructure();
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

app.MapHealthEndpoints();
app.MapSyncPlanEndpoints();
app.MapSyncEndpoints();

app.Run();
