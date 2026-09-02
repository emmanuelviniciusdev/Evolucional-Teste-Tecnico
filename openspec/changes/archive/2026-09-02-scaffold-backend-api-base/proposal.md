## Why

The practical test requires a school enrollment API on .NET Framework 4.8 with ASP.NET Web API, SQL Server, Dapper (raw SQL), and Redis. The repository has the assignment and schema script, but no backend yet. This change delivers only the runnable foundation so later changes can add the assignment endpoints on a working host, data access, cache, tests, and local environment.

## What Changes

- Add a layered ASP.NET Web API 4.8 solution under `apps/backend` (no assignment CRUD, turma, matrícula, or relatório endpoints yet).
- Wire SQL Server access with Dapper and raw SQL, using the assignment schema as the database baseline.
- Wire Redis behind an abstraction so listing/invalidation can be added later without changing callers.
- Add unit and integration test projects with the packages needed to write those tests later.
- Add Docker Compose for SQL Server and Redis, Makefile targets for local infra and tests, and an English README with run instructions.
- Domain language stays in pt-BR (`Aluno`, `Turma`, comments and business messages). Technical identifiers stay in en-US (`IConnectionFactory`, `ICacheService`, `HealthController`). Documentation and the health-check HTTP contract (`GET /api/health` and its JSON) stay in en-US.

## Capabilities

### New Capabilities

- `backend-foundation`: Layered .NET Framework 4.8 Web API host with configuration, Dapper/SQL Server, Redis cache abstraction, and unit/integration test projects.
- `local-dev-environment`: Docker-based SQL Server and Redis, Makefile entry points, and README instructions to run the API and tests.

### Modified Capabilities

- None. There are no existing main specs.

## Impact

- New tree under `apps/backend` (solution, layered projects, test projects, NuGet packages).
- New Docker Compose, Makefile, and `apps/backend/README.md` (and root README pointers if needed so the delivery README is easy to find).
- Database bootstrap from `Enunciado/C#/script-banco.sql` (copied or referenced; adjustments documented in the README).
- No public assignment endpoints in this change. A minimal health/readiness route (`GET /api/health`, en-US JSON) is in scope so operators can confirm the host and dependencies start.
- .NET Framework 4.8 cannot run in Linux containers or natively on macOS. Docker is used for SQL Server and Redis; building, hosting, and running tests require a Windows environment with .NET Framework 4.8 and MSBuild.
