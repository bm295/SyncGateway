# Sync Gateway First Task

## Purpose
The first task for this Sync Gateway project is to confirm what the gateway is meant to do and what it is not meant to do.

This project is positioned as a centralized synchronization middleware service between clients and backend systems. Before building more features, the team should lock down the initial scope and operating rules.

## Already Covered In The Repo
- The Sync Gateway is already implemented as the single sync entry point for clients.
- The initial sync directions are already modeled as `Push`, `Pull`, and `BiDirectional`.
- The API contract already exists for:
  - `POST /api/sync/{tenantId}/{clientId}`
  - `GET /api/sync-plan/{tenantId}/{clientId}`
- API key access control is already in place through `X-Api-Key`.
- A default sync plan already defines:
  - resources
  - filters
  - interval
  - batch size
  - cursor policy
  - conflict policy

## Remaining To-Do
- [x] Create `src/SyncGateway.Domain/Sync/Order.cs` with the first business entity model for the MVP sync scope.
- [x] Create `src/SyncGateway.Application/Abstractions/IOrderRepository.cs` with the source-of-truth repository contract for `Order`.
- [x] Create `src/SyncGateway.Api/Validation/SyncRequestValidator.cs` to validate the incoming sync request payload.
- [x] Create `src/SyncGateway.Api/Mapping/SyncResponseMapper.cs` to convert sync results into API responses.
- [x] Create `src/SyncGateway.Application/Abstractions/IConflictPolicyResolver.cs` to define conflict resolution behavior.
- [x] Create `src/SyncGateway.Infrastructure/Sync/OrderSyncEngine.cs` as the first entity-specific sync engine implementation.
- [ ] Update `src/SyncGateway.Infrastructure/Sync/InMemorySyncPlanProvider.cs` to reference the first MVP entity and its rules.
- [ ] Create `src/SyncGateway.Api/Endpoints/SyncEndpointsTests.cs` to verify the main sync endpoint behavior.
- [ ] Create `src/SyncGateway.Api/Endpoints/SyncPlanEndpointsTests.cs` to verify the sync plan endpoint behavior.
- [ ] Create `docs/FIRST_MVP_ENTITY.md` to record the chosen MVP entity name and why it was selected.
- [ ] Create `docs/SYSTEM_OF_RECORD.md` to record which backend system owns the source of truth.
- [ ] Create `docs/MVP_SUCCESS_CRITERIA.md` to record the exact first-release success criteria.

## Notes
This document is intentionally focused on the first task only. It should be updated once the first scope is finalized and implementation begins.
