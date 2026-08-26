# Find My Prompter — Repository Instructions

## Product mission
Find My Prompter is a vertical hiring marketplace for Prompt Engineers and other AI professionals. The MVP must let an AI professional create a profile, find a job and apply, while a recruiter/company can create a company, publish a job, review candidates and move an application through a hiring pipeline.

Treat this statement as the product north star. Do not expand scope unless a task explicitly requires it.

## Current direction
- Monorepo.
- Frontend: Next.js + React + TypeScript + Tailwind CSS + shadcn/ui.
- Backend: ASP.NET Core on .NET 10.
- Persistence: Entity Framework Core + PostgreSQL.
- Authentication: ASP.NET Core Identity.
- Local infrastructure: Docker / Docker Compose.
- Future infrastructure, only when needed: Redis, Hangfire, object storage, OpenTelemetry, Sentry.
- Future search: PostgreSQL Full-Text Search first; pgvector later for semantic matching.
- Architecture: modular monolith, not microservices.
- API style: REST.

## Repository layout
Expected high-level layout:

- `apps/web/` — Next.js frontend.
- `backend/src/FindMyPrompter.Api/` — HTTP/API composition root.
- `backend/src/FindMyPrompter.Application/` — use cases/application logic.
- `backend/src/FindMyPrompter.Domain/` — domain model and business rules.
- `backend/src/FindMyPrompter.Infrastructure/` — EF Core, PostgreSQL, Identity and external infrastructure.
- `backend/tests/` — backend tests.
- `docs/` — product and architecture documentation.
- `.codex/agents/` — project-scoped Codex agent roles.

If the actual repository differs, inspect the code and update documentation rather than assuming the expected structure is already complete.

## Mandatory working style
1. Inspect the repository before changing files.
2. Read the relevant documentation under `docs/` for the task.
3. Prefer the smallest complete vertical slice over broad speculative infrastructure.
4. Do not introduce a new framework, database, broker, architectural pattern or external service without a concrete requirement.
5. Do not implement post-MVP features unless explicitly requested.
6. Keep changes scoped to the requested feature.
7. Preserve existing public behavior unless the task explicitly changes it.
8. Never commit secrets, passwords, API keys or production connection strings.
9. Prefer explicit, readable code over premature abstractions.
10. Do not create generic repositories/services/base classes unless there is demonstrated reuse.

## Product scope guardrails
The following are intentionally outside the initial MVP unless explicitly requested:
- microservices;
- Kafka;
- Kubernetes;
- Elasticsearch/OpenSearch;
- native mobile apps;
- real-time chat;
- video calls;
- billing/subscriptions;
- ML models trained in-house;
- advanced AI matching;
- Prompter Score;
- automated skill certification;
- social feed.

## Planned business modules
- Identity
- Professionals
- Portfolio
- Companies
- Jobs
- Search
- Applications
- Notifications
- Administration
- Platform

Build them incrementally. Their presence in this list does not mean all modules should be scaffolded upfront.

## Primary MVP workflow
Professional:
`Register -> Login -> Create profile -> Add skills -> Add portfolio -> Search jobs -> Apply -> Track application`

Recruiter:
`Register -> Login -> Create company -> Create job -> Publish job -> Review applicants -> Change application status -> Hire`

A change that helps neither flow should normally not block the MVP.

## Implementation approach
Use vertical slices. For a feature, implement the minimum required path across persistence, application logic, API, UI and tests before opening several unrelated features.

Prefer this:
`feature -> database/domain -> use case -> endpoint -> UI -> tests`

Avoid this:
`all database work -> all backend work -> all frontend work`.

## Git conventions
- Keep `main` deployable.
- Use short-lived branches when branches are needed.
- Conventional Commits are preferred:
  - `feat:` new behavior
  - `fix:` bug fix
  - `test:` tests
  - `refactor:` behavior-preserving refactor
  - `docs:` documentation
  - `chore:` tooling/dependencies
- Do not mix unrelated refactors with feature work.

## Definition of done
Before declaring a code task complete, as applicable:
- build succeeds;
- relevant tests pass;
- validation exists;
- authorization is verified;
- errors are handled intentionally;
- no secrets were added;
- database migrations are included when schema changed;
- frontend states for loading/error/empty/success are considered;
- documentation is updated if architecture or product behavior changed.

If a check cannot be run, state exactly what was not verified and why.

## Important docs
Read only what is relevant to the task:
- `docs/PRODUCT.md` — product vision and users.
- `docs/MVP.md` — MVP boundaries and acceptance criteria.
- `docs/ARCHITECTURE.md` — architectural rules.
- `docs/DOMAIN.md` — domain concepts and planned model.
- `docs/ROADMAP.md` — implementation sequence.
- `docs/DEVELOPMENT.md` — local workflow and commands.
- `docs/CODEX.md` — how Codex should be used in this repository.
- `docs/adr/` — durable architecture decisions.

## When uncertain
Prefer inspecting existing code and asking the smallest necessary question over inventing product rules. For implementation details that can be safely inferred from existing patterns, proceed consistently with the repository.
