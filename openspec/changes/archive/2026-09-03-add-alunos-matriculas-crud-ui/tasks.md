## 1. Host the static page

- [x] 1.1 Add `apps/backend/src/Escola.Api/ui/index.html` as a csproj `<Content>` item (empty or shell with `<html lang="pt-BR">`) and verify the file is copied with the web project
- [x] 1.2 Enable IIS `defaultDocument` `index.html` for `/ui` (project `Web.config` and/or `ui/web.config` so the static file handler wins over Web API routing) and verify `GET http://localhost:5000/ui` or `GET http://localhost:5000/ui/index.html` returns HTML while `GET http://localhost:5000/` still redirects to Swagger

## 2. Aluno CRUD UI

- [x] 2.1 Build the Alunos section in `ui/index.html` with Tailwind Play CDN (`cdn.tailwindcss.com`) and vanilla JS: paginated table from `GET /api/alunos` (nome, email, dataNascimento, `total`, `pagina` / `tamanhoPagina`), optional `nome` filter, and a pt-BR empty state; verify the table renders seeded alunos and filtering calls `nome`
- [x] 2.2 Add the inline create/edit form (`POST /api/alunos` / `PUT /api/alunos/{id}` with `nome`, `email`, `input type="date"` for `dataNascimento`) and Excluir with a pt-BR `confirm` then `DELETE /api/alunos/{id}`; verify a successful create appears in the list, an edit updates the row, and a confirmed delete removes the aluno from the list
- [x] 2.3 Show a pt-BR success banner or the API `{ "error" }` text on 400/404/409 and refresh the list after writes; verify invalid email shows the API error (or a pt-BR client hint) and does not add a row

## 3. Matrícula UI

- [x] 3.1 Add aluno and turma `<select>`s (walk `GET /api/alunos?tamanhoPagina=100` until `total` is covered; `GET /api/turmas` with nome, período, vagas; disable 0-seat turmas) and **Matricular** posting `{ alunoId, turmaId }` to `/api/matriculas`; verify HTTP 201 updates turma seats and HTTP 409 shows the API `error` text
- [x] 3.2 Render the enrollment overview table from `GET /api/relatorios/alunos-por-turma` (nome da turma, quantidade de alunos, vagas restantes), including turmas with zero enrollments, and refresh it after a successful matrícula; verify the seed overview matches the API payload

## 4. Copy and docs

- [x] 4.1 Ensure every visible string is pt-BR (headings Alunos / Matrículas, labels, buttons Salvar / Excluir / Matricular, placeholders, pagination, empty states, confirmations, banners) and `lang="pt-BR"`; verify no English primary labels remain
- [x] 4.2 Document `http://localhost:5000/ui` in `apps/backend/README.md` (pt-BR section **Tela de demonstração**) and mention the same URL in the root README; verify both describe how to open the demonstration page after `make api-run` without claiming assignment endpoints are missing
