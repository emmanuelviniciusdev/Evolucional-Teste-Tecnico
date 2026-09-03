# local-dev-environment Specification

## Purpose

Defines how developers start SQL Server and Redis, run the API and tests, and follow pt-BR README instructions without installing those services on the host OS.

## Requirements

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

### Requirement: Makefile entry points

A Makefile MUST provide named targets that start and stop infrastructure, show service logs, start the API against that infrastructure (Windows/.NET Framework 4.8), and run unit and integration tests. Target names MUST describe the action (for example `infra-up`, `infra-down`, `infra-logs`, `api-run`, `test-unit`, `test-integration`, `test`).

#### Scenario: Infrastructure can be started and stopped

- **WHEN** a developer runs the infrastructure start target and later the stop target
- **THEN** SQL Server and Redis start, and later those containers stop

#### Scenario: Tests have dedicated targets

- **WHEN** a developer runs the unit-test target or the integration-test target on a Windows host with .NET Framework 4.8 and, for integration tests, with infrastructure up
- **THEN** the corresponding test project executes without requiring ad-hoc MSBuild or vstest command lines

### Requirement: pt-BR runbook

`apps/backend/README.md` MUST be written in pt-BR (technical terms in English) and MUST explain what the API is, the stack, Windows requirement for building and running the host and tests, how to start Docker infrastructure, the `TesteEscola` connection string (API) and that `TesteEscola_Testes` / Redis db 1 are created and used only when integration tests run, that `make infra-up` always reseeds `TesteEscola` and flushes Redis db 0, how to run the API, how to run tests, how to call `GET /api/health`, how to open Swagger, how to open the demonstration HTML page (same origin as the API), and how to call the assignment endpoints (`/api/alunos`, `/api/turmas`, `/api/matriculas`, `/api/relatorios/alunos-por-turma`). If the SQL script was changed, the README MUST state what changed and why. The README MUST NOT require `make infra-reset` after a schema change. The root README MUST point at that runbook, MUST mention the demonstration page URL, and MUST NOT claim that assignment endpoints are unimplemented.

#### Scenario: README is enough to run locally

- **WHEN** a developer follows `apps/backend/README.md` on a supported Windows machine with Docker
- **THEN** they can start infrastructure, run the API, hit `GET /api/health`, open Swagger, open the demonstration HTML page, call the assignment endpoints, and run the provided tests without undocumented steps

#### Scenario: Missing endpoints are explicit

- **WHEN** a developer reads `apps/backend/README.md` and the root README
- **THEN** both describe that alunos, turmas, matrículas, and relatório endpoints are available, including how to reach Swagger and the demonstration HTML page, and they MUST NOT claim those routes are unimplemented
