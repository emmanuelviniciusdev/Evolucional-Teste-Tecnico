## 1. Shared plumbing

- [x] 1.1 Add `UQ_Matricula_Aluno_Turma UNIQUE (AlunoId, TurmaId)` to `apps/backend/infra/sql/init.sql` for `TesteEscola` only, remove the skip-on-marker in `wait-and-init.sh`, and make `infra-up` flush Redis db 0; verify a second `make infra-up` recreates sample rows in `TesteEscola` (including the unique index) without `make infra-reset` and does not create `TesteEscola_Testes`
- [x] 1.2 Add `ValidationException`, `NotFoundException`, and `ConflictException` in `Escola.Dominio` and verify `Escola.Dominio` builds
- [x] 1.3 Map those exceptions in `JsonExceptionFilter` to HTTP 400/404/409 with JSON `{ "error": "<pt-BR message>" }` (keep unexpected errors as 500) and verify a unit test of the filter asserts those three statuses
- [x] 1.4 Add repository interfaces `IAlunoRepository`, `ITurmaRepository`, `IMatriculaRepository`, and `IRelatorioRepository` in `Escola.Dominio` (transaction-aware where design requires `IDbTransaction`) and verify Aplicacao can compile against them without referencing Infraestrutura

## 2. Aluno CRUD

- [x] 2.1 Implement Dapper `AlunoRepository` (paged list + count with optional `nome` LIKE, get by id, insert, update nome/email/dataNascimento only, `UPDATE Ativo = 0`) with parameterized SQL and verify `Escola.Infraestrutura` builds
- [x] 2.2 Implement `AlunoService` plus DTOs (pagination defaults `pagina=1`, `tamanhoPagina=10`, max 100; complete email via `MailAddress` plus a dotted domain; local-part with or without a dot; `dataNascimento` only `YYYY-MM-DD` on the DTO JSON converter; reject future dates) and register it in `ApplicationModule`; verify unit tests accept `anasouza2345@email.com` and `ana.souza@email.com` and throw `ValidationException` for `a@b`, `user@localhost`, `2006-03-14T00:00:00`, and `tamanhoPagina` 0/101
- [x] 2.3 Add `AlunosController` at `/api/alunos` (GET list, GET by id, POST 201+Location, PUT, DELETE 204) that only calls `AlunoService`, register the repository in `InfrastructureModule`, and verify `Escola.Api` builds with those routes

## 3. Turma listing and Redis cache

- [x] 3.1 Implement Dapper `TurmaRepository` (list all with `VagasDisponiveis`, get by id, get for update with `UPDLOCK, ROWLOCK`, decrement `WHERE VagasDisponiveis > 0`) and verify SQL is parameterized
- [x] 3.2 Implement `TurmaService` cache-aside on key `turmas:listagem` (TTL 5 minutes, SQL fallback if Redis get/set fails) plus `TurmasController` `GET /api/turmas`, and verify Aplicacao references `ICacheService` but not StackExchange.Redis

## 4. Matrícula

- [x] 4.1 Implement Dapper `MatriculaRepository` (`Exists` and `Insert` on the given transaction) and `MatriculaService` (one connection/transaction, rules, unique-index conflict handling, cache `RemoveAsync` only after commit) and verify unit tests cover success (insert + decrement + cache remove), inactive aluno, no seats, duplicate, missing aluno/turma, and no cache remove on failure
- [x] 4.2 Add `MatriculasController` `POST /api/matriculas` returning 201 + `Location` `/api/matriculas/{id}` that only calls `MatriculaService`, register types in Autofac, and verify `Escola.Api` builds

## 5. Relatório

- [x] 5.1 Implement `RelatorioRepository` with a single SQL `LEFT JOIN` + `GROUP BY` returning turma name, enrolled count, and remaining seats (no in-memory grouping), plus `RelatorioService` and `GET /api/relatorios/alunos-por-turma`, and verify the repository method contains JOIN/GROUP BY and C# does not `GroupBy` enrollment rows

## 6. Swagger

- [x] 6.1 Add Swashbuckle 5.6.0 to `Escola.Api`, enable Swagger + UI in `Startup` with XML comments, and document each assignment action in en-US (pagination, logical delete, cache, transaction, SQL report); verify `GET /swagger` (or `/swagger/ui/index`) returns 200 on TestServer and the v1 document lists `/api/alunos`, `/api/turmas`, `/api/matriculas`, `/api/relatorios/alunos-por-turma`, and `/api/health` with request/response schemas

## 7. Integration tests

- [x] 7.1 Point `Escola.Testes.Integracao/App.config` at `TesteEscola_Testes` and Redis `defaultDatabase=1` (leave `Web.config` on `TesteEscola` / db 0); add an xUnit collection fixture (or `test-integration` pre-step) that creates/seeds `TesteEscola_Testes` and flushes Redis db 1 only when tests run; cover aluno CRUD (list total + name filter, get 404, create 201 including `anasouza2345@email.com`, `YYYY-MM-DD` `dataNascimento`, complete-email 400s, update, logical delete still GET with `ativo` false); verify `make infra-up` alone does not create `TesteEscola_Testes` and a test insert does not add a row to `TesteEscola`
- [x] 7.2 Add integration tests for `GET /api/turmas` remaining seats, `POST /api/matriculas` 201/400/404/409 (inactive, full turma, duplicate), cache invalidation on Redis db 1 (list, enroll, list shows decremented seats), and the report matching SQL counts including a turma with zero enrollments; verify `make test-unit` and `make test-integration` both pass and Redis db 0 is untouched by those cache writes

## 8. Docs

- [x] 8.1 Update `apps/backend/README.md` and the root README in en-US: assignment endpoints, example curls, Swagger at `http://localhost:5000/swagger`, unique-index change, `make infra-up` always reseeds `TesteEscola` and flushes Redis db 0, integration tests initialize `TesteEscola_Testes` themselves, and remove “endpoints not implemented yet”; verify both files mention how to call the business routes and do not require `make infra-reset` after a schema change
