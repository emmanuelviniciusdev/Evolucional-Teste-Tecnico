# Escola enrollment API

ASP.NET Web API 2 for a school enrollment system: alunos CRUD, turma listing with Redis cache, transactional matrícula, and a SQL report. The host runs on Windows (.NET Framework 4.8); SQL Server and Redis run in Docker.

## Stack

- .NET Framework 4.8
- ASP.NET Web API 2 hosted on IIS / IIS Express (OWIN `Startup`)
- SQL Server with Dapper and parameterized SQL
- Redis via StackExchange.Redis (`ICacheService`)
- Autofac for composition
- Swashbuckle 5.6.0 for Swagger UI

## Windows requirement

Building the solution, hosting the API, and running tests require **Windows** with:

- .NET Framework 4.8 Developer Pack
- Visual Studio 2022 (or MSBuild + NuGet + IIS Express)
- Docker Desktop (for SQL Server and Redis)

The API **cannot** run on macOS, Linux, or in a Linux container. Docker is used only for SQL Server and Redis. Host ports **1433** (SQL Server) and **6379** (Redis) must be free before `make infra-up`. On Apple Silicon, the SQL Server 2022 image runs via amd64 emulation.

## How to run locally

From `apps/backend`:

```bash
make restore
make infra-up
make api-run
```

`make infra-up` always re-applies schema and sample rows to `TesteEscola` and flushes Redis db 0. You do **not** need `make infra-reset` after a schema or seed change.

`make api-run` starts infrastructure, then IIS Express on [http://localhost:5000](http://localhost:5000). Override the IIS Express path if needed:

```bash
make api-run IIS_EXPRESS="C:\Program Files\IIS Express\iisexpress.exe"
```

Alternatively, open `Escola.sln` in Visual Studio and press F5 (project URL is `http://localhost:5000/`).

### Docker infrastructure

```bash
make infra-up      # SQL Server :1433 and Redis :6379, reseed TesteEscola, flush Redis db 0
make infra-logs    # follow container logs
make infra-down    # stop containers
make infra-reset   # wipe Docker volumes, then infra-up (optional; not required after schema changes)
```

There is no Linux image for the Web API process.

### Connection strings (local only)

These development credentials are **not for production**.

| Setting | API (`Web.config`) | Integration tests (`App.config`) |
| --- | --- | --- |
| SQL Server | `Server=localhost,1433;Database=TesteEscola;User Id=sa;Password=Escola_Dev_P@ssw0rd;Encrypt=True;TrustServerCertificate=True` | Same server, database `TesteEscola_Testes` |
| Redis | `localhost:6379,abortConnect=false` (logical db 0) | `localhost:6379,abortConnect=false,defaultDatabase=1` |

`TesteEscola_Testes` is created and seeded only when integration tests run. Compose / `make infra-up` does not create it.

## Health check

With infrastructure and the API running:

```bash
curl http://localhost:5000/api/health
```

- HTTP 200 when SQL Server and Redis are reachable (`status`: `healthy`)
- HTTP 503 when either dependency is down (`unavailable` on `sqlServer` and/or `redis`)

## Swagger

Open [http://localhost:5000/swagger](http://localhost:5000/swagger) (UI) or `http://localhost:5000/swagger/docs/v1` (OpenAPI document). Assignment routes and request/response schemas are documented in en-US.

## Assignment endpoints

JSON is camelCase. Error bodies use `{ "error": "<pt-BR message>" }` with HTTP 400 (validation), 404 (missing row), or 409 (business rule).

### Alunos

```bash
curl http://localhost:5000/api/alunos
curl "http://localhost:5000/api/alunos?nome=ana&pagina=1&tamanhoPagina=10"
curl http://localhost:5000/api/alunos/1
curl -X POST http://localhost:5000/api/alunos -H "Content-Type: application/json" -d "{\"nome\":\"Ana Souza\",\"email\":\"anasouza2345@email.com\",\"dataNascimento\":\"2006-03-14\"}"
curl -X PUT http://localhost:5000/api/alunos/1 -H "Content-Type: application/json" -d "{\"nome\":\"Ana Souza\",\"email\":\"ana.souza@email.com\",\"dataNascimento\":\"2006-03-14\"}"
curl -X DELETE http://localhost:5000/api/alunos/1
```

`dataNascimento` is `YYYY-MM-DD` only. Email must be a complete address (local-part with or without a dot). DELETE is a logical deactivation (`Ativo = 0`); GET still returns the aluno.

### Turmas

```bash
curl http://localhost:5000/api/turmas
```

Remaining seats come from `VagasDisponiveis`. The list is cached in Redis (`turmas:listagem`, TTL 5 minutes) and invalidated after a successful matrícula.

### Matrículas

```bash
curl -X POST http://localhost:5000/api/matriculas -H "Content-Type: application/json" -d "{\"alunoId\":1,\"turmaId\":2}"
```

Insert and seat decrement run in one transaction. Inactive aluno, no seats, or duplicate pair return HTTP 409.

### Relatório

```bash
curl http://localhost:5000/api/relatorios/alunos-por-turma
```

Counts are computed in SQL (`LEFT JOIN` + `GROUP BY`), including turmas with zero enrollments.

## Tests

```bash
make test-unit          # no Docker required
make test-integration   # starts Docker infra, then creates TesteEscola_Testes / flushes Redis db 1
make test               # unit then integration
```

## Schema script

`infra/sql/init.sql` started as a copy of `Enunciado/C#/script-banco.sql`. It adds `CONSTRAINT UQ_Matricula_Aluno_Turma UNIQUE (AlunoId, TurmaId)` so concurrent duplicate enrollments cannot persist. Every `make infra-up` re-drops/recreates API tables in `TesteEscola`, reseeds sample rows, and flushes Redis db 0. `make infra-reset` is not required to pick up that unique index.

## Make targets

| Target | Action |
| --- | --- |
| `restore` | `nuget restore Escola.sln` |
| `infra-up` | Start SQL Server and Redis, reseed `TesteEscola`, flush Redis db 0 |
| `infra-down` | Stop containers |
| `infra-logs` | Follow Compose logs |
| `infra-reset` | `down -v` then `infra-up` (volume wipe; optional) |
| `api-run` | `infra-up` then IIS Express on port 5000 |
| `test-unit` | Restore, MSBuild, and run the unit test project |
| `test-integration` | `infra-up` then run the integration test project |
| `test` | Unit tests then integration tests |
