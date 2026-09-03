## MODIFIED Requirements

### Requirement: pt-BR runbook

`apps/backend/README.md` MUST be written in pt-BR (technical terms in English) and MUST explain what the API is, the stack, Windows requirement for building and running the host and tests, how to start Docker infrastructure, the `TesteEscola` connection string (API) and that `TesteEscola_Testes` / Redis db 1 are created and used only when integration tests run, that `make infra-up` always reseeds `TesteEscola` and flushes Redis db 0, how to run the API, how to run tests, how to call `GET /api/health`, how to open Swagger, how to open the demonstration HTML page (same origin as the API), and how to call the assignment endpoints (`/api/alunos`, `/api/turmas`, `/api/matriculas`, `/api/relatorios/alunos-por-turma`). If the SQL script was changed, the README MUST state what changed and why. The README MUST NOT require `make infra-reset` after a schema change. The root README MUST point at that runbook, MUST mention the demonstration page URL, and MUST NOT claim that assignment endpoints are unimplemented.

#### Scenario: README is enough to run locally

- **WHEN** a developer follows `apps/backend/README.md` on a supported Windows machine with Docker
- **THEN** they can start infrastructure, run the API, hit `GET /api/health`, open Swagger, open the demonstration HTML page, call the assignment endpoints, and run the provided tests without undocumented steps

#### Scenario: Missing endpoints are explicit

- **WHEN** a developer reads `apps/backend/README.md` and the root README
- **THEN** both describe that alunos, turmas, matrículas, and relatório endpoints are available, including how to reach Swagger and the demonstration HTML page, and they MUST NOT claim those routes are unimplemented
