using SyncGateway.Application.SyncPlans;

namespace SyncGateway.Application.Abstractions;

public interface ISyncPlanProvider
{
    Task<SyncPlan?> GetPlanAsync(SyncPlanContext context, CancellationToken cancellationToken);
}
