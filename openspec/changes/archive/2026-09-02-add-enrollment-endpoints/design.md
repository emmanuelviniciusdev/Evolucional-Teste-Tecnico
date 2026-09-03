## Context

See `proposal.md` for motivation. The host already exists under `apps/backend` as four layers (`Escola.Api`, `Escola.Aplicacao`, `Escola.Dominio`, `Escola.Infraestrutura`) plus unit/integration tests, Autofac, Dapper, `ICacheService` → Redis, and `GET /api/health`. Assignment routes, repositories, Swagger, and domain-to-HTTP mapping are still missing.

Constraints: .NET Framework 4.8, ASP.NET Web API 2, Dapper with parameterized SQL, no EF. Domain names and business messages stay pt-BR; technical identifiers and Swagger/README stay en-US. JSON is already camelCase.

## Goals / Non-Goals

**Goals:**

- Complete Clean Architecture for the assignment routes: thin controllers, use-case services, domain abstractions, Dapper repositories.
- One database transaction for matrícula insert + `VagasDisponiveis` decrement, with a unique index as a concurrency backstop.
- Redis cache on `GET /api/turmas` with invalidation after a successful matrícula.
- Swashbuckle on the existing OWIN pipeline; exception filter maps validation/not-found/conflict to 400/404/409.
- Unit tests for matrícula rules; integration tests for HTTP contracts, cache invalidation, and the SQL report.

**Non-Goals:**

- HTML/jQuery aluno screen (assignment bonus, not requested).
- Reactivating an aluno (PUT does not change `Ativo`; only DELETE sets it false).
- Unique email, turma CRUD, matrícula cancel/list, or in-memory cache fallback.
- MediatR, AutoMapper, or a full Unit-of-Work framework.

## Decisions

### 1. Keep the existing Clean Architecture layout

Controllers in `Escola.Api` accept HTTP, call one application service, and map success to 200/201/204. Use-case services in `Escola.Aplicacao` (`AlunoService`, `TurmaService`, `MatriculaService`, `RelatorioService`) own validation and business rules. Repository interfaces live in `Escola.Dominio`; Dapper implementations live in `Escola.Infraestrutura`. Register new types in the existing Autofac modules.

Request/response DTOs live in `Escola.Aplicacao` next to the services (not in controllers) so Swagger and tests share the same shapes. Map entities to DTOs by hand.

**Alternatives considered:** MediatR handlers (extra package, little gain on four use cases). Fat controllers (assignment penalizes business logic there). Repositories in Aplicacao (leaks SQL abstractions upward).

### 2. Domain exceptions → HTTP statuses

Introduce `ValidationException` (400), `NotFoundException` (404), and `ConflictException` (409) in `Escola.Dominio`. Extend `JsonExceptionFilter` (or a dedicated domain filter registered beside it) to serialize `{ "error": "<pt-BR message>" }` for those types. Unexpected exceptions stay HTTP 500 with the existing en-US generic message.

Controllers still choose 201 (`Created`) and 204 (`StatusCode(204)`) on success. `Location` is `/api/alunos/{id}` or `/api/matriculas/{id}`.

Keep **409** for inactive aluno (and for turma sem vaga / duplicate). The aluno exists, so 404 is wrong. The JSON body is well-formed, so 400 is wrong. 403 would imply authorization, which this API does not have. 422 Unprocessable Entity is the only serious REST alternative for a semantic rule, but the assignment lists 409 for “regra de negócio impedir a operação (ex: turma sem vaga)” — inactive aluno is the same class of current-state conflict.

**Alternatives considered:** `Result<T>` objects returned to controllers (verbose on C# 7.3, duplicates status mapping). Mapping statuses inside every controller action (easy to miss a 409). 422 for inactive aluno (reasonable HTTP, but off the assignment's 200/201/400/404/409 list).

### 3. Matrícula transaction and concurrency

`MatriculaService` orchestrates the rules so they are unit-testable with NSubstitute:

1. Reject non-positive or missing ids (`ValidationException`).
2. Open one connection via `IConnectionFactory`, `BeginTransaction`.
3. Load aluno and turma on that transaction (turma with `UPDLOCK, ROWLOCK`).
4. Not found → `NotFoundException`; inactive aluno, `VagasDisponiveis <= 0`, or existing pair → `ConflictException`.
5. `UPDATE Turma SET VagasDisponiveis = VagasDisponiveis - 1 WHERE Id = @id AND VagasDisponiveis > 0`. Zero rows → `ConflictException` (lost race on last seat).
6. `INSERT` matrícula. Unique-index violation → `ConflictException`.
7. `Commit`, then `ICacheService.RemoveAsync` for the turma list key.

Repository methods accept `IDbTransaction` (Dominio already references `System.Data`). Do not commit inside individual repositories.

Schema change in `infra/sql/init.sql` (documented in README):

```sql
CONSTRAINT UQ_Matricula_Aluno_Turma UNIQUE (AlunoId, TurmaId)
```

Seed data already has unique pairs. `init.sql` applies DDL+seed to `TesteEscola` only. Remove the skip-on-marker in `wait-and-init.sh` so every `make infra-up` re-drops API tables, recreates them with the unique index, and reseeds. Flush Redis db 0 on that same up so the API turma cache cannot outlive the SQL seed. `make infra-reset` may remain as a volume wipe; it is not required after this schema change.

**Alternatives considered:** One infra `TryEnroll` stored-procedure-style method (harder to unit-test rules without SQL). Serializable isolation without the unique index (still allows duplicates if checks race). Application-only duplicate check (fails under concurrency). Keep the init marker and document `infra-reset` (rejected: this is not production; Compose up should refresh API state). Seed `TesteEscola_Testes` from Compose (rejected: the test database is initialized only when tests run).

### 4. Turma listing cache

Cache key `turmas:listagem`. TTL 5 minutes as a safety net; successful matrícula deletes the key after commit so the next list is a SQL miss + refill. `TurmaService.ListAsync` reads cache first, on miss loads from `ITurmaRepository` and `SetAsync`. Do not invalidate on aluno CRUD (seats do not change). Relatório is always live SQL, not cached.

If Redis get/set fails, still return the SQL list (listing must not 500 because cache is down); health already reports Redis. Invalidation after commit is best-effort: log and continue if Redis delete fails (the TTL bounds staleness).

**Alternatives considered:** Cache-aside per turma id (overkill for a small list). In-memory `MemoryCache` (user required Redis). Invalidate before commit (empty cache then rollback would show a write that never happened).

### 5. Aluno persistence and pagination

`IAlunoRepository`: paged list + count (optional `nome` with `LIKE '%' + @Nome + '%'`, case-insensitive via default collation), get by id, insert, update (`Nome`, `Email`, `DataNascimento` only), logical delete (`UPDATE ... SET Ativo = 0`). SQL Server `OFFSET/FETCH` (2022 image). Default `pagina = 1`, `tamanhoPagina = 10`, max 100.

Validation in `AlunoService`: required nome/email/dataNascimento; nome/email length ≤ 120; **complete** email (parse with `System.Net.Mail.MailAddress` **and** require a domain with at least two labels separated by `.`). The local-part MUST accept letters and digits with or without a dot: `anasouza2345@email.com` and `ana.souza@email.com` are both valid. Reject `a@b`, `user@localhost`, `user@`, `@dominio.com`, whitespace. `dataNascimento` is a calendar date only: JSON request and response MUST be `YYYY-MM-DD`. Put an `IsoDateTimeConverter` (or equivalent) with a pattern that accepts only that layout on the DTO property; do not let Newtonsoft silently accept `2006-03-14T00:00:00`. `DataCadastro` / `DataMatricula` stay full datetimes. Reject a future calendar date. List includes inactive alunos. DELETE is idempotent when already inactive.

### 6. Relatório SQL

`IRelatorioRepository.ListAlunosPorTurma` runs a single parameterized query and maps rows to a DTO. No `GroupBy` in C#.

```sql
SELECT t.Nome AS NomeTurma,
       COUNT(m.Id) AS QuantidadeAlunos,
       t.VagasDisponiveis AS VagasRestantes
FROM dbo.Turma t
LEFT JOIN dbo.Matricula m ON m.TurmaId = t.Id
GROUP BY t.Id, t.Nome, t.VagasDisponiveis
ORDER BY t.Nome
```

`LEFT JOIN` keeps turmas with zero enrollments.

### 7. Swagger (Swashbuckle 5.x)

Add `Swashbuckle` 5.6.0 to `Escola.Api` and enable it in `Startup` on the same `HttpConfiguration` (`EnableSwagger` + `EnableSwaggerUi`). Document XML comments on controllers/DTOs; include that XML in Swagger. UI at `/swagger` (default Swashbuckle path). Operation summaries in en-US describing behavior (pagination, logical delete, transaction, cache, SQL report). Do not expose XML formatter.

**Alternatives considered:** NSwag (heavier host wiring on Web API 2). Hand-written OpenAPI file (drifts from code).

### 8. Tests isolated from the API database

**Unit (`Escola.Testes.Unitarios`):** `MatriculaService` with substituted repositories, connection factory, and cache — success (decrement + insert + cache remove), inactive aluno, no seats, duplicate, missing aluno/turma, no cache remove on failure. `AlunoService` tests for incomplete emails (`a@b`, `user@localhost`), valid complete emails (`anasouza2345@email.com` and `ana.souza@email.com`), and `dataNascimento` not `YYYY-MM-DD`.

**Integration (`Escola.Testes.Integracao`):** `TestServer` + live SQL/Redis. `App.config` MUST use `Database=TesteEscola_Testes` and Redis `defaultDatabase=1`. `Web.config` stays `TesteEscola` and Redis db 0. An xUnit collection fixture (or the `test-integration` Makefile target immediately before `dotnet test`) MUST create database `TesteEscola_Testes` if needed, apply the same schema+seed as `init.sql` (including the unique index), and flush Redis db 1. That fixture MUST run only when tests execute; Compose init stays `TesteEscola`-only. It MAY reuse the Docker SQL Server already started for the API. Cover aluno CRUD (including logical delete still gettable, `dataNascimento` as `YYYY-MM-DD`, complete-email 400s and `anasouza2345@email.com` 201), `GET /api/turmas` remaining seats, POST matrícula 201/400/404/409, cache invalidation on Redis db 1, report matching SQL counts. Put mutating tests in that collection so they do not run in parallel against `TesteEscola_Testes`. Prefer creating unique alunos per test. `Turma Lotada` and inactive Diego on the test DB remain useful if the collection is sequential.

Health smoke tests stay (they hit `TesteEscola_Testes` / Redis db 1 after the fixture and MUST NOT write to `TesteEscola`).

### 9. Docs

Update `apps/backend/README.md` and the root README: assignment routes, example `curl`s, Swagger URL (`http://localhost:5000/swagger`), unique-index change, that `make infra-up` always reseeds `TesteEscola` and flushes Redis db 0, that integration tests create/seed `TesteEscola_Testes` themselves, remove “endpoints not implemented yet”. Do not tell reviewers they must run `make infra-reset` to pick up the unique index.

## Risks / Trade-offs

- [Re-running DROP/CREATE on every `infra-up` wipes local API data in `TesteEscola`] → Acceptable: this is not production; reviewers get a known API seed without `infra-reset`. Test data lives in `TesteEscola_Testes` and is rebuilt only when tests run.
- [Exception-as-flow for 400/404/409] → Keep exception types small and filter-only; do not catch them inside services.
- [Cache invalidation after commit can still race with a list in flight] → Accept a brief stale read; TTL 5 minutes plus post-commit delete is enough for this test.
- [TestServer might still read `Web.config` if config is not copied] → Point `App.config` at `TesteEscola_Testes` / Redis db 1 and verify a test that inserts an aluno does not add a row to `TesteEscola`.
- [Swashbuckle 5.x on OWIN] → Enable on the same `HttpConfiguration` as Web API; verify `/swagger` through TestServer or a manual README step.
- [Framework 4.8 still Windows-only] → Unchanged; no new host requirement.

## Migration Plan

No production data. Developers: pull, `make infra-up` (reseed `TesteEscola` + flush Redis db 0), `make restore`, `make api-run`, open `/swagger`. Integration tests create `TesteEscola_Testes` when they start. Rollback: revert the change branch and `make infra-up` to restore the previous API script.

## Open Questions

None. Pagination query names (`pagina`, `tamanhoPagina`, `nome`), PUT not touching `Ativo`, including inactive alunos in the list, 409 for inactive aluno, complete email (local-part with or without a dot), `YYYY-MM-DD` birth date, Compose-up reseed of `TesteEscola` only, and test-only initialization of `TesteEscola_Testes` are recorded in the specs.
