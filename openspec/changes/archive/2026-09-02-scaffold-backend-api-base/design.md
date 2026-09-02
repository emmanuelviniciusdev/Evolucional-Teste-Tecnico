## Context

See `proposal.md` for motivation. The repo has the assignment and `Enunciado/C#/script-banco.sql` only; there is no `apps/backend` yet. Specs in `specs/backend-foundation` and `specs/local-dev-environment` fix the host, layering, SQL/Redis, readiness route, language, tests, Docker, Makefile, and README.

Constraints that shape the approach:

- Stack is non-negotiable: .NET Framework 4.8, ASP.NET Web API, SQL Server, Dapper with handwritten SQL, Redis.
- This change is foundation only: no alunos/turmas/matrículas/relatórios routes.
- The current workstation may be macOS, but Framework 4.8 cannot run on macOS or in Linux containers. Docker is therefore infrastructure-only.

## Goals / Non-Goals

**Goals:**

- A Visual Studio–openable Web API 2 solution under `apps/backend` with four runtime layers plus two test projects.
- Composition root with DI so later features plug into existing SQL, Redis, and HTTP pipelines.
- Docker Compose that brings up SQL Server (schema + seed) and Redis; Makefile targets that wrap infra, IIS Express host, and tests.
- Prove the harness with one unit smoke test and one integration smoke test (`GET /api/health`).

**Non-Goals:**

- Assignment business endpoints, matrícula transaction rules, turma cache invalidation, or the HTML/jQuery screen.
- Linux (or Windows) Docker image for the Web API process.
- Entity Framework, .NET Core / .NET 5+, or AutoMapper-heavy scaffolding.
- Changing the assignment schema unless SQL Server Linux init forces a documented, mechanical adjustment (for example `GO` batch handling).

## Decisions

### 1. Solution layout and naming

```
apps/backend/
  Escola.sln
  src/
    Escola.Api/                 # ASP.NET Web API 2 host (IIS / IIS Express)
    Escola.Aplicacao/           # use cases (HealthService now; others later)
    Escola.Dominio/             # entities + abstractions
    Escola.Infraestrutura/      # Dapper, SqlConnection, Redis
  testes/
    Escola.Testes.Unitarios/
    Escola.Testes.Integracao/
  infra/
    docker-compose.yml
    sql/init.sql                # copy of assignment script, plus any documented tweaks
  Makefile
  README.md
```

Layer project names stay pt-BR (`Escola.Aplicacao`, `Escola.Dominio`, `Escola.Infraestrutura`). Domain entities `Aluno`, `Turma`, and `Matricula` ship now so later repositories map 1:1 to tables. Technical types use English (`IAlunoRepository` later, not `IRepositorioAluno`). Repository implementations wait for the endpoint change (YAGNI). Health uses `IDependencyChecker` rather than fake repositories.

**Alternatives considered:** Translating Factory/Service/Cache into Portuguese (`IFabricaConexao`) — rejected; technical terms stay English. English project names (rejected: layer folders stay pt-BR). Repositories for every table now (rejected: unused surface). Merging Aplicacao into Api (rejected: assignment penalizes business logic in controllers).

### 2. Host: Web API 2 on IIS Express, OWIN Startup for tests

`Escola.Api` is a classic ASP.NET Web Application targeting `v4.8` (non-SDK csproj) so reviewers can F5 in Visual Studio. HTTP pipeline is configured with an OWIN `Startup` (`Microsoft.Owin.Host.SystemWeb` + `Microsoft.AspNet.WebApi.Owin`): JSON-only Web API, Autofac Web API integration, attribute routing, `HealthController`.

`make api-run` launches IIS Express against `src/Escola.Api` on `http://localhost:5000`. Integration tests host the same `Startup` in-process with `Microsoft.Owin.Testing.TestServer` so they do not need IIS.

**Alternatives considered:** OWIN console self-host only (easier Makefile, less familiar to Framework reviewers). Pure IIS without OWIN (awkward in-memory tests). Linux container for the API (impossible for Framework 4.8).

### 3. DI: Autofac

Autofac + `Autofac.WebApi2` is the composition root in `Startup`. Application and Infrastructure modules register `HealthService`, `IConnectionFactory`, `ICacheService`, and `IDependencyChecker`. Controllers resolve only application services.

**Alternatives considered:** Unity (more ceremony, weaker module story). `Microsoft.Extensions.DependencyInjection` (not native to Web API 2). Poor-man's `new` in controllers (violates layering spec).

### 4. Persistence: Dapper + `IConnectionFactory`

`IConnectionFactory.Create()` returns an open `IDbConnection` (`SqlConnection` from `System.Data.SqlClient`). Health runs `SELECT 1`. All SQL lives in Infraestrutura as parameterized strings. Connection string name `Escola` in `Web.config` / test `App.config`.

No Unit of Work type yet; when matrícula lands, a single `IDbTransaction` on a connection from this factory is enough. Do not add EF or a micro-ORM besides Dapper.

**Alternatives considered:** ADO.NET without Dapper (more boilerplate, assignment asks for Dapper). Repository-per-table now (premature).

### 5. Cache: `ICacheService` + StackExchange.Redis

```text
GetAsync<T>(key)
SetAsync<T>(key, value, expiration?)
RemoveAsync(key)
```

Default implementation `RedisCacheService` uses `StackExchange.Redis` `IConnectionMultiplexer` from appSetting `Redis` (e.g. `localhost:6379`). JSON serialization via Newtonsoft.Json. No in-memory fallback in this change: local Redis is guaranteed by Compose. Callers never take `IDatabase`.

**Alternatives considered:** In-memory-only cache (assignment bonus allows it, but the user asked for Redis). Exposing StackExchange types to Aplicacao (violates spec).

### 6. Readiness: `GET /api/health`

`HealthController` → `HealthService` → `IDependencyChecker` (SQL `SELECT 1` + Redis `PING`). HTTP 200 when both succeed; HTTP 503 when either fails, with en-US JSON naming the failed dependency (`sqlServer` / `redis`) and status text (`healthy` / `unavailable`). Unexpected exceptions still become JSON 500 via a global exception filter (placeholder for later domain-to-HTTP mapping).

### 7. Tests

| Project | Packages | Smoke |
| --- | --- | --- |
| `Escola.Testes.Unitarios` | xUnit, NSubstitute, FluentAssertions | `HealthService` with mocked `IDependencyChecker` |
| `Escola.Testes.Integracao` | xUnit, Microsoft.Owin.Testing, Dapper, StackExchange.Redis | `GET /api/health` against TestServer + live Docker SQL/Redis |

Class libraries and test projects use SDK-style `net48` + PackageReference. Integration tests read the same connection settings as local Docker (localhost:1433 / 6379). No Testcontainers (weak Framework 4.8 story).

**Alternatives considered:** NUnit (fine; xUnit is enough). MSTest. Hitting IIS Express from integration tests (slower, more flaky).

### 8. Docker and Makefile

Compose services:

- `sqlserver`: `mcr.microsoft.com/mssql/server:2022-latest`, SA password documented, port 1433.
- `sqlserver-init`: mssql-tools job that waits for `sqlcmd` then applies `infra/sql/init.sql` (assignment script; `sqlcmd` understands `GO`).
- `redis`: `redis:7-alpine`, port 6379.

Makefile (in `apps/backend`):

| Target | Action |
| --- | --- |
| `infra-up` | `docker compose up -d` and wait until SQL accepts connections |
| `infra-down` | `docker compose down` |
| `infra-logs` | follow compose logs |
| `infra-reset` | down -v then up (re-seed) |
| `api-run` | `infra-up` then IIS Express on port 5000 |
| `test-unit` | MSBuild + xUnit runner for unit project |
| `test-integration` | `infra-up` then integration project |
| `test` | unit then integration |
| `restore` | NuGet restore of `Escola.sln` |

Root `README.md` is a short en-US pointer to `apps/backend/README.md` so the GitHub landing page has the delivery runbook.

**Alternatives considered:** Postgres-style `/docker-entrypoint-initdb.d` on the SQL image (not supported). Bundling the API in Compose (incompatible with Framework 4.8 on Linux). `make run-local` as a single vague name (rejected in favor of `infra-up` / `api-run`).

### 9. Language and docs

Domain names, comments, and business messages in pt-BR (`Aluno`, `Turma`, `Matricula`). Technical identifiers in en-US (`IConnectionFactory`, `ICacheService`, `HealthService`, `HealthController`). README, Makefile comments, OpenSpec artifacts, and `GET /api/health` JSON in en-US. Later assignment routes stay pt-BR (`/api/alunos`, etc.). Do not translate Factory, Service, Cache, Repository, Controller, DTO, or similar terms.

## Risks / Trade-offs

- [Framework 4.8 will not build or run on macOS] → Document Windows + .NET Framework 4.8 Developer Pack + IIS Express + Docker Desktop. Infra can still be started with Docker on macOS; `api-run` and tests cannot.
- [SQL Server Linux image does not auto-run `.sql` on boot] → Dedicated init service with retry loop; `infra-up` waits for readiness rather than racing `sqlcmd`.
- [IIS Express path differs per machine] → Makefile uses `IIS_EXPRESS` override; README shows the default `Program Files` path and Visual Studio F5 as fallback.
- [SA password in Compose and Web.config] → Dev-only credentials, documented as local secrets; not for production.
- [SDK-style libraries + old-style Web csproj] → Restore via solution-level NuGet; README includes `make restore`. Mixing styles is a trade-off for nicer test/library projects vs IIS web project constraints.
- [Health check is an extra endpoint vs the assignment list] → Needed to prove host + Docker; README states business routes come later so reviewers are not surprised.

## Migration Plan

Greenfield: no production system and no data to migrate.

1. Add solution, Docker, Makefile, and README under `apps/backend`.
2. Developers: install Docker, start `make infra-up`, open or `make api-run` on Windows, verify `GET /api/health`.
3. Rollback: delete `apps/backend` and `docker compose down -v`; no schema lives outside Compose volumes and local SQL.

Later endpoint work should reuse `IConnectionFactory`, `ICacheService`, Autofac modules, and the exception filter without replacing the host.
