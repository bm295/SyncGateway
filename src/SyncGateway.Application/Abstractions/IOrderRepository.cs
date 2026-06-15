using SyncGateway.Domain.Sync;

namespace SyncGateway.Application.Abstractions;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(string tenantId, string orderId, CancellationToken cancellationToken);

    Task UpsertAsync(Order order, CancellationToken cancellationToken);

    Task DeleteAsync(string tenantId, string orderId, CancellationToken cancellationToken);
}
