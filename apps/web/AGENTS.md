# Frontend Instructions

These instructions apply to everything under `apps/web/` and extend the repository root `AGENTS.md`.

## Stack
- Next.js App Router
- React
- TypeScript
- Tailwind CSS
- shadcn/ui where useful
- Playwright for E2E when introduced
- Vitest/testing-library when unit/component tests are introduced

## General frontend rules
- Keep TypeScript strict and avoid `any` unless there is a documented reason.
- Prefer Server Components by default; use Client Components only when interactivity/browser APIs require them.
- Keep data access and authentication boundaries explicit.
- Do not duplicate backend business rules in the UI; frontend validation is for UX, backend validation remains authoritative.
- Build accessible forms: associated labels, keyboard support, focus handling and semantic elements.
- Always consider loading, empty, error and success states.
- Keep components focused; do not create a generic design-system abstraction until repeated use justifies it.

## Authentication
The backend authentication foundation is ASP.NET Core Identity.

For browser authentication:
- prefer secure cookie-based sessions;
- do not persist access/refresh tokens in `localStorage` by default;
- API calls requiring cookies must be configured intentionally (`credentials`/same-site/CORS behavior as appropriate);
- protected pages must not rely only on hiding UI controls; backend authorization is authoritative.

The first authentication UI slice should remain small:
- `/register`
- `/login`
- authenticated session/current-user check
- logout

Do not add OAuth providers, MFA or complex onboarding until explicitly requested.

## Routing
Use the App Router. Public MVP routes will likely evolve toward:
- `/login`
- `/register`
- `/jobs`
- `/jobs/[slug]`
- `/prompters/[username]`
- `/companies/[slug]`
- dashboard routes for authenticated users

Do not scaffold all future routes upfront.

## API integration
Centralize repeated API concerns only after repetition exists. Avoid large speculative API client layers for a handful of requests.

Treat API errors as structured user-facing states. Do not swallow errors or only log them to the console.

## Testing and checks
For frontend changes, run from `apps/web/` when scripts exist:

```bash
npm run lint
npm run build
```

Also run the relevant test command once tests are configured.

For authentication and other critical user journeys, prefer E2E coverage after the flow is functional.

## UX direction
The product should feel like a professional hiring marketplace for AI talent, not a generic social network. Favor clarity, credibility, scannable job/profile data and direct calls to action over decorative complexity.

<!-- BEGIN:nextjs-agent-rules -->

# This is NOT the Next.js you know

This version has breaking changes — APIs, conventions, and file structure may all differ from your training data. Read the relevant guide in `node_modules/next/dist/docs/` (resolved from this file's directory; in monorepos the `next` package may not be visible from the repo root) before writing any code. Heed deprecation notices.

This block is written and re-added by `next dev` — verify at `node_modules/next/dist/server/lib/generate-agent-files.js`. Removing it from a diff only re-creates the uncommitted change; committing it with your work keeps the tree clean.

<!-- END:nextjs-agent-rules -->
