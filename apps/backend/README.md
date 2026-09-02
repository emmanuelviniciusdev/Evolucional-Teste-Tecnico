# Escola enrollment API

ASP.NET Web API 2 foundation for a school enrollment system. The host, SQL Server access, Redis cache, tests, and local Docker environment are in place. Assignment business endpoints (alunos, turmas, matrículas, relatórios) are **not implemented yet** and will be added in a later change.

## Stack

- .NET Framework 4.8
- ASP.NET Web API 2 hosted on IIS / IIS Express (OWIN `Startup`)
- SQL Server with Dapper and parameterized SQL
- Redis via StackExchange.Redis (`ICacheService`)
- Autofac for composition

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

`make api-run` starts infrastructure, then IIS Express on [http://localhost:5000](http://localhost:5000). Override the IIS Express path if needed:

```bash
make api-run IIS_EXPRESS="C:\Program Files\IIS Express\iisexpress.exe"
```

Alternatively, open `Escola.sln` in Visual Studio and press F5 (project URL is `http://localhost:5000/`).

### Docker infrastructure

```bash
make infra-up      # SQL Server :1433 and Redis :6379, then apply schema
make infra-logs    # follow container logs
make infra-down    # stop containers
make infra-reset   # wipe volumes and re-seed
```

There is no Linux image for the Web API process.

### Connection strings (local only)

These development credentials are **not for production**.

| Setting | Value |
| --- | --- |
| SQL Server | `Server=localhost,1433;Database=TesteEscola;User Id=sa;Password=Escola_Dev_P@ssw0rd;Encrypt=True;TrustServerCertificate=True` (`Web.config` connection string name `Escola`) |
| Redis | `localhost:6379,abortConnect=false` (`Web.config` appSetting `Redis`) |

Integration tests use the same values in `testes/Escola.Testes.Integracao/App.config`.

## Health check

With infrastructure and the API running:

```bash
curl http://localhost:5000/api/health
```

- HTTP 200 when SQL Server and Redis are reachable (`status`: `healthy`)
- HTTP 503 when either dependency is down (`unavailable` on `sqlServer` and/or `redis`)

Assignment routes such as `/api/alunos`, `/api/turmas`, `/api/matriculas`, and `/api/relatorios/alunos-por-turma` are not served yet (HTTP 404 is expected).

## Tests

```bash
make test-unit          # no Docker required
make test-integration   # starts Docker infra, then hits GET /api/health
make test               # unit then integration
```

## Schema script

`infra/sql/init.sql` is a copy of `Enunciado/C#/script-banco.sql`. The script was not changed: `sqlcmd` understands `GO` batches, and the init wrapper retries until SQL Server accepts connections. A marker volume skips re-applying the script on later `infra-up` runs; use `make infra-reset` to re-seed.

## Make targets

| Target | Action |
| --- | --- |
| `restore` | `nuget restore Escola.sln` |
| `infra-up` | Start SQL Server and Redis, wait until they are healthy, apply `init.sql` |
| `infra-down` | Stop containers |
| `infra-logs` | Follow Compose logs |
| `infra-reset` | `down -v` then `infra-up` |
| `api-run` | `infra-up` then IIS Express on port 5000 |
| `test-unit` | Restore, MSBuild, and run the unit test project |
| `test-integration` | `infra-up` then run the integration test project |
| `test` | Unit tests then integration tests |
