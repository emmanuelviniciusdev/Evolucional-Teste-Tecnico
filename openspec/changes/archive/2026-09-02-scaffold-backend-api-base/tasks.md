## 1. Solution skeleton

- [x] 1.1 Create `apps/backend/Escola.sln` with `src/Escola.Api` (non-SDK Web Application, `v4.8`), `src/Escola.Aplicacao`, `src/Escola.Dominio`, `src/Escola.Infraestrutura` (SDK-style `net48`), and verify the solution lists all four projects with the correct target frameworks
- [x] 1.2 Add project references (Api → Aplicacao → Dominio, Infraestrutura → Dominio and Dapper/`System.Data.SqlClient`/StackExchange.Redis, Aplicacao not referencing Infraestrutura) and verify `Escola.Aplicacao.csproj` has no Infraestrutura reference
- [x] 1.3 Add NuGet packages (Web API 2, OWIN SystemWeb, Autofac.WebApi2, Newtonsoft.Json, Dapper, StackExchange.Redis) and verify `nuget restore apps/backend/Escola.sln` succeeds

## 2. Domain and infrastructure

- [x] 2.1 Add pt-BR domain entities `Aluno`, `Turma`, `Matricula` plus English technical abstractions `IConnectionFactory`, `ICacheService`, `IDependencyChecker` and verify they compile in `Escola.Dominio`
- [x] 2.2 Implement `ConnectionFactory` (SqlConnection from connection string `Escola`), Dapper `SELECT 1` health check, and `RedisCacheService` (`GetAsync`/`SetAsync`/`RemoveAsync`) and verify a short console or unit-level compile of Infraestrutura with parameterized SQL only
- [x] 2.3 Implement `HealthService` in Aplicacao that aggregates SQL and Redis checks into an en-US status model (`healthy` / `unavailable`) and verify it compiles without referencing Redis or SqlClient types

## 3. Web host and readiness

- [x] 3.1 Configure OWIN `Startup` with JSON-only Web API, Autofac modules, attribute routing, and a global JSON exception filter, and verify `Escola.Api` builds
- [x] 3.2 Add `HealthController` mapped to `GET /api/health` returning 200 when both dependencies are healthy and 503 naming the failed one, and verify no alunos/turmas/matriculas/relatorios controllers exist
- [x] 3.3 Put local connection string and Redis endpoint in `Web.config` (localhost:1433 `TesteEscola`, localhost:6379, documented SA password) and verify secrets are not duplicated in C# source

## 4. Tests

- [x] 4.1 Create `testes/Escola.Testes.Unitarios` (xUnit, NSubstitute, FluentAssertions, `net48`) with a `HealthService` smoke test using mocked `IDependencyChecker` and verify `dotnet test` or the xUnit runner reports a pass
- [x] 4.2 Create `testes/Escola.Testes.Integracao` (xUnit, Microsoft.Owin.Testing, matching App.config) with a `GET /api/health` smoke test against TestServer and verify the test project builds and references Escola.Api Startup
- [x] 4.3 Add both test projects to `Escola.sln` and verify the solution restores and the unit smoke test passes without Docker

## 5. Docker, Makefile, and docs

- [x] 5.1 Add `apps/backend/infra/docker-compose.yml` (SQL Server 2022, redis:7-alpine, sqlcmd init service) and copy `Enunciado/C#/script-banco.sql` to `infra/sql/init.sql`, then verify `docker compose` config is valid and there is no API Linux image
- [x] 5.2 Add an init wrapper that retries `sqlcmd` until SQL is up, applies `init.sql`, and verify `TesteEscola` contains Aluno/Turma/Matricula after `infra-up` on a Docker host
- [x] 5.3 Add `apps/backend/Makefile` with `restore`, `infra-up`, `infra-down`, `infra-logs`, `infra-reset`, `api-run` (IIS Express :5000), `test-unit`, `test-integration`, and `test`, and verify `make -n` prints the expected commands
- [x] 5.4 Write `apps/backend/README.md` in en-US covering stack, Windows requirement, Docker, connection strings, `make` targets, `GET /api/health`, schema-script notes, and that assignment endpoints are later; add a short root README pointer; verify both files are English and mention missing business routes
