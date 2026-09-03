## MODIFIED Requirements

### Requirement: Dockerized SQL Server and Redis

Local SQL Server and Redis MUST run via Docker Compose. Every documented Compose / `infra-up` start MUST re-apply the assignment schema (including sample data and any documented script changes) to `TesteEscola` so existing table rows in that database are replaced; a skip-on-first-init marker MUST NOT leave stale data. Compose init MUST NOT create or seed `TesteEscola_Testes`. Redis db 0 MUST be flushed on that same up so cached turma listings cannot outlive a SQL reseed of `TesteEscola`. Published ports and credentials MUST be documented in the README. The Web API process MUST NOT be packaged as a Linux container. `make infra-reset` MAY still wipe Docker volumes, but it MUST NOT be required to pick up schema or seed changes.

#### Scenario: Compose starts both services

- **WHEN** a developer starts the documented Compose stack
- **THEN** SQL Server and Redis become reachable on the documented host ports

#### Scenario: Database is initialized

- **WHEN** a developer starts the documented Compose stack (including a second `infra-up` against an already-created volume)
- **THEN** database `TesteEscola` exists with `Aluno`, `Turma`, `Matricula`, the unique `(AlunoId, TurmaId)` constraint, and the assignment sample rows, replacing any rows left from a previous run, and `TesteEscola_Testes` is not created or reseeded by that Compose init

#### Scenario: API is not a Linux image

- **WHEN** a reviewer inspects Docker artifacts
- **THEN** there is no Linux image that claims to run the .NET Framework 4.8 Web API

### Requirement: English runbook

`apps/backend/README.md` MUST be written in en-US and MUST explain what the API is, the stack, Windows requirement for building and running the host and tests, how to start Docker infrastructure, the `TesteEscola` connection string (API) and that `TesteEscola_Testes` / Redis db 1 are created and used only when integration tests run, that `make infra-up` always reseeds `TesteEscola` and flushes Redis db 0, how to run the API, how to run tests, how to call `GET /api/health`, how to open Swagger, and how to call the assignment endpoints (`/api/alunos`, `/api/turmas`, `/api/matriculas`, `/api/relatorios/alunos-por-turma`). If the SQL script was changed, the README MUST state what changed and why. The README MUST NOT require `make infra-reset` after a schema change. The root README MUST point at that runbook and MUST NOT claim that assignment endpoints are unimplemented.

#### Scenario: README is enough to run locally

- **WHEN** a developer follows `apps/backend/README.md` on a supported Windows machine with Docker
- **THEN** they can start infrastructure, run the API, hit `GET /api/health`, open Swagger, call the assignment endpoints, and run the provided tests without undocumented steps

#### Scenario: Missing endpoints are explicit

- **WHEN** a developer reads `apps/backend/README.md` and the root README
- **THEN** both describe that alunos, turmas, matrículas, and relatório endpoints are available, including how to reach Swagger, and they MUST NOT claim those routes are unimplemented
