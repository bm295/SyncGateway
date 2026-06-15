# Sync Gateway MVP Implementation

This repository now includes an initial implementation of the Sync Gateway plan as a centralized middleware service with Clean Architecture boundaries.

## Implemented in this iteration

### Clean Architecture structure
- `SyncGateway.Domain` contains dependency-free sync entities and enums.
- `SyncGateway.Application` contains use-case request/response models and ports such as `ISyncEngine` and `ISyncPlanProvider`.
- `SyncGateway.Infrastructure` contains adapter implementations, including the in-memory sync engine and sync plan provider.
- `SyncGateway.Api` contains HTTP contracts, endpoint composition, middleware, and application startup only.

### Centralized middleware behavior
- Clients call `GET /api/sync-plan/{tenantId}/{clientId}` to fetch plan metadata.
- Clients call `POST /api/sync/{tenantId}/{clientId}` for sync execution.
- Gateway enforces API key authentication (`X-Api-Key`).
- Backend sync orchestration is encapsulated behind the application `ISyncEngine` port.

### Sync engine capabilities (MVP)
- Supports push, pull, and bidirectional sync modes.
- Applies upsert/delete changes into a tenant-scoped in-memory store.
- Performs basic optimistic-version conflict detection for upserts.
- Returns a server cursor and conflict details.
- Exposes an in-memory sync plan contract (resources, direction, interval, conflict policy, batch size, filters, version).

### Project structure
- `src/SyncGateway.Api` — ASP.NET Core presentation layer.
- `src/SyncGateway.Application` — application layer and ports.
- `src/SyncGateway.Domain` — domain model.
- `src/SyncGateway.Infrastructure` — infrastructure adapters and DI wiring.
- `SyncGateway.sln` — solution entry point.

## Next steps to align with full plan
- Replace in-memory storage with persistent source-of-truth DB + change log.
- Add queue-backed worker and retry/dead-letter policies.
- Add JWT/OIDC auth and role-based authorization.
- Add tests (unit + integration + end-to-end).
