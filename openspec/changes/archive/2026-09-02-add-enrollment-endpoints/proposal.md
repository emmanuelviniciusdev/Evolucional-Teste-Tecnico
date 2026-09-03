## Why

The backend foundation is runnable (host, SQL Server, Redis, health check, tests), but the assignment still has no business endpoints. This change delivers the school enrollment API: alunos CRUD, turma listing, transactional matrícula, and the SQL report, with Swagger and coherent HTTP status codes.

## What Changes

- Add aluno CRUD at `/api/alunos` (paginated list with optional name filter and total count, get by id, create, update, logical delete via `Ativo`).
- Add `GET /api/turmas` listing remaining seats per class, cached in Redis and invalidated when a matrícula is created.
- Add `POST /api/matriculas` with the assignment rules (active aluno, available seat, no duplicate enrollment) inside a single database transaction.
- Add `GET /api/relatorios/alunos-por-turma` computed in SQL (JOIN / GROUP BY), not assembled in C# memory.
- Expose request/response schemas and per-endpoint behavior in Swagger.
- Map validation and business failures to 400 / 404 / 409 (never 500 for those cases). Inactive aluno stays 409: the row exists and the body is valid, but current state blocks enrollment (same class of rule as turma sem vaga).
- Validate `email` as a complete address (not merely `@`). The local-part may be letters and digits without a dot (`anasouza2345@email.com`) or with a dot (`ana.souza@email.com`). Accept `dataNascimento` only as `YYYY-MM-DD`.
- Add unique constraint `(AlunoId, TurmaId)` on `Matricula` (documented schema change) so concurrent duplicate enrollments cannot persist. `make infra-up` / Compose up MUST re-apply schema and seed for `TesteEscola` every time so `make infra-reset` is not required after that change.
- Isolation: the API uses database `TesteEscola` and Redis db 0. Integration tests use `TesteEscola_Testes` and Redis db 1, and MUST create/seed that database only when tests run (not during Compose / `infra-up`).
- Extend unit and integration tests and update README/runbook so reviewers can exercise the assignment routes.

## Capabilities

### New Capabilities

- `aluno-crud`: Paginated aluno list with optional name filter and total count; get, create, update, and logical delete.
- `turma-listing`: List turmas with remaining seats; Redis cache for that list; invalidate on new matrícula.
- `matricula`: Enroll an aluno in a turma with business rules and transactional insert + seat decrement.
- `relatorio-alunos-por-turma`: Per-turma report (name, enrolled count, remaining seats) from a single SQL query.

### Modified Capabilities

- `backend-foundation`: Assignment HTTP routes are now in scope; Swagger documents those routes; domain/validation failures map to 400/404/409 instead of 500; integration tests use an isolated SQL/Redis store.
- `local-dev-environment`: README documents the assignment endpoints and Swagger UI; Compose up always refreshes `TesteEscola` schema/seed (including the unique index); `infra-reset` is not required for that refresh. The test database is not part of Compose init.

## Impact

- New controllers, application services, repositories, and DTOs under `apps/backend/src`.
- Autofac registrations, exception/result mapping, and Swashbuckle on the Web API 2 host.
- `infra/sql/init.sql` plus documented README note for the unique index; init runs on every Compose up (no skip-on-marker) for `TesteEscola` only; Redis db 0 is flushed on that same up.
- Integration tests create and seed `TesteEscola_Testes` (and use Redis db 1) only at test start via `App.config` / a test fixture; they must not rely on Compose having created that database.
- New unit tests for matrícula rules; new integration tests for HTTP contracts, cache invalidation, and the report query.
- `GET /api/health` stays as-is. The HTML/jQuery aluno screen is out of scope.
