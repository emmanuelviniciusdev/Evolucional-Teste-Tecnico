## Purpose

Defines the school enrollment API foundation: a layered .NET Framework 4.8 host with SQL Server persistence, Redis cache access, configuration, a readiness check, and test projects, without the assignment business endpoints.

## ADDED Requirements

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

The system SHALL expose a cache abstraction backed by Redis in the default configuration. Callers MUST depend on the abstraction, not on a Redis client type. The abstraction MUST support get, set, and delete by key so later turma listing cache and matrícula invalidation can be added without changing the host wiring.

#### Scenario: Default configuration uses Redis

- **WHEN** the API starts with the documented local configuration
- **THEN** cache operations talk to the configured Redis instance

#### Scenario: Callers do not take a Redis client

- **WHEN** application code needs cache
- **THEN** it depends on the cache abstraction, not on a Redis client class

### Requirement: Readiness endpoint

The API SHALL expose `GET /api/health`. The path and JSON body MUST be en-US (`status` values `healthy` / `unavailable`, dependency keys `sqlServer` and `redis`). When SQL Server and Redis are reachable, the response MUST be HTTP 200 and a JSON body that reports both dependencies as healthy. When either dependency is unreachable, the response MUST be HTTP 503 and MUST identify which dependency failed. This is the only HTTP endpoint in this change. Assignment routes (`/api/alunos`, `/api/turmas`, `/api/matriculas`, `/api/relatorios/...`) MUST NOT be implemented yet.

#### Scenario: Dependencies are up

- **WHEN** a client calls `GET /api/health` and SQL Server and Redis accept connections
- **THEN** the API returns HTTP 200 with JSON indicating both are healthy (`healthy`)

#### Scenario: A dependency is down

- **WHEN** a client calls `GET /api/health` and SQL Server or Redis cannot be reached
- **THEN** the API returns HTTP 503 and the body identifies the failed dependency (`unavailable`)

#### Scenario: Assignment endpoints are absent

- **WHEN** a client calls `/api/alunos`, `/api/turmas`, `/api/matriculas`, or `/api/relatorios/alunos-por-turma`
- **THEN** the API does not serve those business operations yet (HTTP 404 is acceptable)

### Requirement: Language conventions

Domain names, comments, and business messages in implementation MUST be written in pt-BR (`Aluno`, `Turma`, `Matricula`). Technical identifiers MUST be written in en-US (`IConnectionFactory`, `ICacheService`, `HealthService`, `HealthController`, `Create`, `GetAsync`). The health-check HTTP contract (`GET /api/health` and its JSON) MUST be en-US. Operator and developer documentation MUST be written in en-US.

#### Scenario: Domain is pt-BR and technical names are en-US

- **WHEN** a reviewer inspects C# types and members added in this change
- **THEN** domain types use Portuguese names and technical types/members use English names (for example `IConnectionFactory`, not `IFabricaConexao`)

#### Scenario: Health HTTP is en-US

- **WHEN** a client calls the readiness endpoint
- **THEN** the path is `/api/health` and JSON field names and status strings are English

#### Scenario: Docs are en-US

- **WHEN** a reviewer inspects README and other developer docs added in this change
- **THEN** they are in English (United States)

### Requirement: Unit and integration test projects

The solution SHALL include a unit test project and an integration test project, both targeting .NET Framework 4.8, with the packages required to write unit tests (including isolation of application services) and integration tests (HTTP host, SQL Server, and Redis). At least one smoke test MUST pass in each project so the harness is proven. Business tests for matrícula and other assignment rules are out of scope.

#### Scenario: Unit test project compiles and runs

- **WHEN** unit tests are executed on a Windows host with .NET Framework 4.8
- **THEN** the unit test project compiles, has isolation packages available, and its smoke test passes

#### Scenario: Integration test project can use SQL Server and Redis

- **WHEN** integration tests run against the documented local SQL Server and Redis
- **THEN** the project compiles, can issue an HTTP request to the API, can reach the database and cache, and its smoke test passes
