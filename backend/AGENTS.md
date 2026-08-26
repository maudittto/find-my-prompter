# Backend Instructions

These instructions apply to everything under `backend/` and extend the repository root `AGENTS.md`.

## Stack
- .NET 10
- ASP.NET Core
- Entity Framework Core
- PostgreSQL via Npgsql
- ASP.NET Core Identity for authentication/identity persistence
- xUnit for tests
- Testcontainers for integration tests when introduced

## Project dependency direction
Maintain this dependency direction:

- `FindMyPrompter.Domain` -> no dependency on Application, Infrastructure or Api.
- `FindMyPrompter.Application` -> may depend on Domain.
- `FindMyPrompter.Infrastructure` -> may depend on Application and Domain.
- `FindMyPrompter.Api` -> composition root; may reference Application and Infrastructure.

Do not reference Infrastructure from Domain or Application merely for convenience.

## Architecture
Use a modular-monolith mindset and feature-oriented/vertical-slice organization. Do not pre-create a project per module or add microservices.

Prefer feature names that express use cases, for example:
- `CreateJob`
- `PublishJob`
- `ApplyToJob`
- `ChangeApplicationStatus`

Avoid a codebase organized primarily around generic `Services`, `Repositories`, `Managers` and `Helpers` folders.

## Domain model
Business rules belong close to the domain where practical. Infrastructure concerns do not belong in domain entities.

Do not put ASP.NET types, EF-specific concerns, HTTP requests or external SDK models in the Domain project.

## Entity Framework Core
- PostgreSQL is the source of truth for relational persistence.
- Add migrations only when the model/schema changes.
- Do not manually edit generated migration code unless there is a specific migration requirement and the reason is documented.
- Avoid eager-loading large graphs by default.
- Use projections for read models when full entities are unnecessary.
- Treat query count and accidental N+1 behavior as correctness/performance concerns.

## Identity
Use ASP.NET Core Identity as the authentication foundation.
- The Identity persistence implementation belongs in Infrastructure.
- Do not store bearer tokens in frontend localStorage as the default web authentication strategy.
- For the web application, prefer secure cookie-based authentication unless architecture changes explicitly.
- User identity and business profiles are separate concepts. Do not turn `ApplicationUser` into the entire professional profile/company model.

## Authorization
Authentication is not authorization.
Every endpoint that reads or modifies user-owned/company-owned resources must validate ownership or an explicit permission/policy.

Avoid scattering checks such as `if role == ...` throughout business code. Prefer authorization policies/requirements as the application grows.

Be alert to IDOR/BOLA risks whenever an endpoint accepts an entity ID.

## API
- REST API.
- Use consistent `/api/...` routes.
- Use appropriate HTTP status codes.
- Validate request input at the boundary.
- Do not expose EF entities directly as API contracts by default.
- Do not leak stack traces or internal exception details to clients.

## Testing
For backend changes, run at minimum when applicable:

```bash
dotnet build backend/FindMyPrompter.sln
dotnet test backend/FindMyPrompter.sln
```

If running from `backend/`, use:

```bash
dotnet build FindMyPrompter.sln
dotnet test FindMyPrompter.sln
```

When an EF migration is added, verify that the application can start against the local PostgreSQL environment and that the migration is valid.

## Security
Never weaken password, cookie, CORS, CSRF, authorization or validation settings merely to make a request pass locally.

Do not commit real credentials. Development-only defaults belong in local development configuration and should be replaceable by environment variables.
