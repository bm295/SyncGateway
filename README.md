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
- `src/SyncGateway.Api/`
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
