# local-dev-environment Specification

## Purpose

Defines how developers start SQL Server and Redis, run the API and tests, and follow English instructions without installing those services on the host OS.

## Requirements

### Requirement: Dockerized SQL Server and Redis

Local SQL Server and Redis MUST run via Docker Compose. The SQL Server service MUST apply the assignment schema (including sample data) on first start so the database is usable without manual table creation. Published ports and credentials MUST be documented in the README. The Web API process MUST NOT be packaged as a Linux container.

#### Scenario: Compose starts both services

- **WHEN** a developer starts the documented Compose stack
- **THEN** SQL Server and Redis become reachable on the documented host ports

#### Scenario: Database is initialized

- **WHEN** SQL Server starts for the first time with the Compose stack
- **THEN** database `TesteEscola` exists with `Aluno`, `Turma`, `Matricula`, and the assignment sample rows

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

### Requirement: English runbook

`apps/backend/README.md` MUST be written in en-US and MUST explain what the API is, the stack, Windows requirement for building and running the host and tests, how to start Docker infrastructure, connection strings, how to run the API, how to run tests, how to call `GET /api/health`, and that assignment endpoints are not implemented yet. If the SQL script was changed, the README MUST state what changed and why.

#### Scenario: README is enough to run locally

- **WHEN** a developer follows `apps/backend/README.md` on a supported Windows machine with Docker
- **THEN** they can start infrastructure, run the API, hit `GET /api/health`, and run the provided tests without undocumented steps

#### Scenario: Missing endpoints are explicit

- **WHEN** a developer reads the README
- **THEN** it states that alunos, turmas, matrículas, and relatório endpoints will be added later
