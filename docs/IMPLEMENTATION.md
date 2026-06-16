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


### Inbound and outbound sync purpose
- **Inbound** is the client-to-gateway side of a sync request. It carries local client changes in `SyncRequest.changes` so the gateway can validate them, apply push or bidirectional updates, detect conflicts, and persist accepted changes to the tenant-scoped backend store. Inbound processing is enabled for `Push` and `BiDirectional` sync directions.
- **Outbound** is the gateway-to-client side of a sync response. It returns backend/server changes in `SyncResult.outboundChanges` so clients can update their local cache from the gateway-controlled source of truth. Outbound processing is enabled for `Pull` and `BiDirectional` sync directions.
- In `BiDirectional` mode, a single request can do both: first accept inbound client changes, then send outbound changes back with the next server cursor and any conflict details.
- Keeping these concepts separate makes the gateway the central synchronization boundary: clients never sync directly with backend systems, while backend-specific reads/writes remain hidden behind application ports and infrastructure adapters.

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
