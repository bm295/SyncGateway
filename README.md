# SyncGateway

A centralized Sync Gateway middleware service between clients and backend systems.

## What's implemented
- ASP.NET Core API scaffold for synchronization.
- Centralized sync endpoint: `POST /api/sync/{tenantId}/{clientId}`.
- Sync plan endpoint: `GET /api/sync-plan/{tenantId}/{clientId}`.
- API key middleware (`X-Api-Key`) for gateway access control.
- In-memory sync engine supporting Push, Pull, and BiDirectional flows.
- Conflict reporting and cursor-based sync response model.

## Project structure
- `SyncGateway.sln`
- `src/SyncGateway.Api/` - presentation layer with HTTP contracts, endpoint mapping, and API key middleware.
- `src/SyncGateway.Application/` - use-case boundary models and application ports.
- `src/SyncGateway.Domain/` - enterprise sync entities and enums with no infrastructure dependencies.
- `src/SyncGateway.Infrastructure/` - adapter implementations and dependency injection wiring.
- `tests/SyncGateway.Tests/` - behavior and architecture regression tests.
- `docs/FIRST_TASK.md`
- `docs/IMPLEMENTATION.md`
- `SYNC_GATEWAY_PROJECT_PLAN.md`
- `Archived/` (legacy sample files)

## Local run
```bash
dotnet run --project src/SyncGateway.Api/SyncGateway.Api.csproj
```

Health check:
```bash
curl http://localhost:5000/health
```

Run the validation suite:

```bash
dotnet test SyncGateway.sln
```


Sync plan request example:
```bash
curl "http://localhost:5000/api/sync-plan/default/client-a?appVersion=1.0.0&platform=android&capabilities=delta-sync,background-sync" \
  -H "X-Api-Key: local-dev-api-key"
```

Use the returned plan to drive sync requests:
- `direction` => `SyncRequest.direction`
- `resources` and `filters` => select which local changes become `SyncRequest.changes`
- `batchSize.maxChangesPerRequest` => chunk outbound `changes`
- `interval` => scheduling cadence for calling `/api/sync/{tenantId}/{clientId}`
- `version` => cache invalidation/telemetry tag

Sync request example:
```bash
curl -X POST "http://localhost:5000/api/sync/default/client-a" \
  -H "Content-Type: application/json" \
  -H "X-Api-Key: local-dev-api-key" \
  -d '{
    "cursor": null,
    "direction": "BiDirectional",
    "correlationId": "corr-001",
    "changes": []
  }'
```

## Pull checkpoints

The generic in-memory engine keeps an ordered change log per tenant, including delete
tombstones and repeated updates. A null cursor starts at sequence 0. Pull and
BiDirectional return at most 200 log entries after the supplied sequence.
`nextCursor` is the last returned sequence; an empty batch keeps the input position.
Push preserves the pull checkpoint. Continue pulling until an empty batch is returned.
Save the checkpoint only after successfully applying the entire batch to the local
database (ideally in the same local transaction). Retrying an unsaved checkpoint
replays the batch; clients must apply changes idempotently.

Sequence allocation, state mutation and log publication share a per-tenant lock,
so a reader cannot skip a write that becomes visible late. A persistent adapter must
provide the same publication ordering and atomic state/log writes; an identity column
and MAX(sequence) alone do not guarantee this across concurrent transactions.
The current log is unbounded and in memory: restarting loses both state and history.
Persisted checkpoints require durable history before production use. Cursors are
scoped to a tenant and must not be reused after its history is reset.

Reference: [Couchbase changes feed](https://docs.couchbase.com/sync-gateway/current/deploy/changes-feed.html).
