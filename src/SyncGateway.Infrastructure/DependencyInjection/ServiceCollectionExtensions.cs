using Microsoft.Extensions.DependencyInjection;
using SyncGateway.Application.Abstractions;
using SyncGateway.Infrastructure.Sync;

namespace SyncGateway.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSyncGatewayInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ISyncEngine, InMemorySyncEngine>();
        services.AddSingleton<ISyncPlanProvider, InMemorySyncPlanProvider>();

        return services;
    }
}
