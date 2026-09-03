# backend-foundation Specification

## Purpose

Defines the school enrollment API foundation: a layered .NET Framework 4.8 host with SQL Server persistence, Redis cache access, configuration, a readiness check, OpenAPI documentation, and test projects.

## Requirements

### Requirement: Framework 4.8 Web API host

The backend SHALL be an ASP.NET Web API application targeting .NET Framework 4.8 located under `apps/backend`. It MUST NOT target .NET Core or .NET 5+. The host MUST start with JSON as the default response format.

#### Scenario: Host targets Framework 4.8

- **WHEN** a reviewer inspects the Web API project
- **THEN** the project targets .NET Framework 4.8 and is an ASP.NET Web API host, not .NET Core or .NET 5+

#### Scenario: JSON is the default response format

- **WHEN** a client calls a JSON route without requiring XML
- **THEN** the response Content-Type is JSON

### Requirement: Layered composition

The solution SHALL separate HTTP hosting, application/use-case logic, domain types, and infrastructure (database and cache). HTTP controllers MUST NOT contain business rules or SQL. Infrastructure MUST be reachable from application code through abstractions, not from controllers talking to SQL or Redis directly.

#### Scenario: Controllers stay thin

- **WHEN** a reviewer inspects controller code
- **THEN** controllers only accept requests, invoke application services, and map results to HTTP responses

### Requirement: SQL Server access with application-owned SQL

The system SHALL persist data in SQL Server. All database access MUST use parameterized SQL written in the application. The system MUST NOT use Entity Framework or any ORM that generates SQL. Connection settings MUST come from configuration, not hardcoded secrets in source.

#### Scenario: Schema matches the assignment baseline

- **WHEN** the database is initialized for local or test use
- **THEN** it contains `Aluno`, `Turma`, and `Matricula` with the assignment columns and sample data from the provided script, unless the README records a justified script change

#### Scenario: Queries are parameterized

- **WHEN** infrastructure issues SQL
- **THEN** user- or request-derived values are passed as parameters, not concatenated into the command text

### Requirement: Redis cache abstraction

The system SHALL expose a cache abstraction backed by Redis in the default configuration. Callers MUST depend on the abstraction, not on a Redis client type. The abstraction MUST support get, set, and delete by key so the turma listing cache and matrícula invalidation can use it without controllers talking to Redis.

#### Scenario: Default configuration uses Redis

- **WHEN** the API starts with the documented local configuration
- **THEN** cache operations talk to the configured Redis instance

#### Scenario: Callers do not take a Redis client

- **WHEN** application code needs cache
- **THEN** it depends on the cache abstraction, not on a Redis client class

### Requirement: Readiness endpoint

The API SHALL expose `GET /api/health`. The path and JSON body MUST be en-US (`status` values `healthy` / `unavailable`, dependency keys `sqlServer` and `redis`). When SQL Server and Redis are reachable, the response MUST be HTTP 200 and a JSON body that reports both dependencies as healthy. When either dependency is unreachable, the response MUST be HTTP 503 and MUST identify which dependency failed.

#### Scenario: Dependencies are up

- **WHEN** a client calls `GET /api/health` and SQL Server and Redis accept connections
- **THEN** the API returns HTTP 200 with JSON indicating both are healthy (`healthy`)

#### Scenario: A dependency is down

- **WHEN** a client calls `GET /api/health` and SQL Server or Redis cannot be reached
- **THEN** the API returns HTTP 503 and the body identifies the failed dependency (`unavailable`)

#### Scenario: Assignment endpoints are absent

- **WHEN** a client calls `/api/alunos`, `/api/turmas`, `/api/matriculas`, or `/api/relatorios/alunos-por-turma`
- **THEN** the API serves those assignment operations (CRUD, turma listing, matrícula, and relatório), not HTTP 404 for a missing controller

### Requirement: Language conventions

Domain names, comments, and business messages in implementation MUST be written in pt-BR (`Aluno`, `Turma`, `Matricula`). Technical identifiers MUST be written in en-US (`IConnectionFactory`, `ICacheService`, `HealthService`, `HealthController`, `Create`, `GetAsync`). The health-check HTTP contract (`GET /api/health` and its JSON) MUST be en-US. OpenSpec artifacts MUST be written in en-US. Operator and developer documentation (README and runbooks) MUST be written in pt-BR, keeping technical terms in English.

#### Scenario: Domain is pt-BR and technical names are en-US

- **WHEN** a reviewer inspects C# types and members added in this change
- **THEN** domain types use Portuguese names and technical types/members use English names (for example `IConnectionFactory`, not `IFabricaConexao`)

#### Scenario: Health HTTP is en-US

- **WHEN** a client calls the readiness endpoint
- **THEN** the path is `/api/health` and JSON field names and status strings are English

#### Scenario: Project docs are pt-BR; OpenSpec is en-US

- **WHEN** a reviewer inspects README files and OpenSpec artifacts added in this change
- **THEN** README and operator runbooks are in Brazilian Portuguese with technical terms in English, and OpenSpec specs remain in English (United States)

### Requirement: Unit and integration test projects

The solution SHALL include a unit test project and an integration test project, both targeting .NET Framework 4.8, with the packages required to write unit tests (including isolation of application services) and integration tests (HTTP host, SQL Server, and Redis). At least one smoke test MUST pass in each project so the harness is proven. Assignment business tests (matrícula rules and HTTP contracts for alunos, turmas, matrículas, and relatórios) MUST live in these projects.

#### Scenario: Unit test project compiles and runs

- **WHEN** unit tests are executed on a Windows host with .NET Framework 4.8
- **THEN** the unit test project compiles, has isolation packages available, and its smoke test passes

#### Scenario: Integration test project can use SQL Server and Redis

- **WHEN** integration tests run against the documented local SQL Server and Redis
- **THEN** the project compiles, can issue an HTTP request to the API, can reach the database and cache, and its smoke test passes

### Requirement: Integration tests are isolated from the API database

Integration tests MUST use SQL database `TesteEscola_Testes` and Redis logical database 1. The running API MUST keep using `TesteEscola` and Redis logical database 0. `TesteEscola_Testes` MUST be created and seeded only when integration tests execute (a test fixture or the `test-integration` target MAY use the already-running Docker SQL Server). Compose / `infra-up` MUST NOT create or seed `TesteEscola_Testes`. Writes performed by integration tests MUST NOT change rows in `TesteEscola` or keys in Redis db 0. Writes performed through the API against `TesteEscola` MUST NOT change `TesteEscola_Testes` or Redis db 1.

#### Scenario: Tests do not mutate the API database

- **WHEN** integration tests insert or update alunos, turmas, or matrículas
- **THEN** those changes appear only in `TesteEscola_Testes`, and `TesteEscola` still has the seed (or whatever the API last wrote), not the test rows

#### Scenario: Tests do not share the API Redis database

- **WHEN** integration tests read or write the turma listing cache
- **THEN** they use Redis db 1, and Redis db 0 used by the API is unchanged by those test cache operations

#### Scenario: Compose up does not initialize the test database

- **WHEN** a developer runs `make infra-up` or Compose without running integration tests
- **THEN** `TesteEscola_Testes` is not created or reseeded by that start

#### Scenario: Integration tests initialize their own database

- **WHEN** integration tests start
- **THEN** `TesteEscola_Testes` exists with the assignment schema, unique index, and sample rows (replacing any previous test data), and Redis db 1 is flushed or otherwise isolated for that run

### Requirement: OpenAPI documentation

The API SHALL expose Swagger/OpenAPI UI and a machine-readable schema for the assignment endpoints (`/api/alunos`, `/api/turmas`, `/api/matriculas`, `/api/relatorios/alunos-por-turma`) and `GET /api/health`. Each assignment operation MUST include a description of its behavior. Request and response bodies MUST have documented schemas (properties, types, and required fields). Swagger text MUST be written in en-US. The UI MUST be reachable from the running host without extra undocumented setup.

#### Scenario: Swagger UI is reachable

- **WHEN** the API is running and a client opens the documented Swagger URL
- **THEN** the UI lists the assignment endpoints and health, each with a behavior description

#### Scenario: Schemas are documented

- **WHEN** a reviewer inspects the Swagger document for `POST /api/alunos` or `POST /api/matriculas`
- **THEN** the request body schema names the required fields and the response schema is present

### Requirement: Client errors use coherent HTTP statuses

Validation failures MUST return HTTP 400. Missing records MUST return HTTP 404. Business-rule rejections (inactive aluno, turma without seats, duplicate matrícula) MUST return HTTP 409. Those cases MUST NOT return HTTP 500. Unexpected failures MAY still return HTTP 500 as JSON. Successful reads and updates MUST return HTTP 200. Successful creates MUST return HTTP 201. Successful logical deletes MUST return HTTP 204. Error JSON for assignment routes MUST include a pt-BR `error` message. HTTP 409 is the correct status for an inactive aluno: the aluno exists (not 404) and the request is well-formed (not 400); enrollment is blocked by current resource state, which is the same class of conflict as a turma without seats.

#### Scenario: Validation is not a server error

- **WHEN** a client sends an assignment request with missing required fields
- **THEN** the API returns HTTP 400 with a JSON `error` message, not HTTP 500

#### Scenario: Business rule is a conflict

- **WHEN** a client tries to enroll an inactive aluno or into a turma with no remaining seats
- **THEN** the API returns HTTP 409, not HTTP 500
