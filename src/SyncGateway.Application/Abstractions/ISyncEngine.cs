using SyncGateway.Application.Sync;

namespace SyncGateway.Application.Abstractions;

public interface ISyncEngine
{
    Task<SyncResult> SyncAsync(SyncCommand command, CancellationToken cancellationToken);
}
