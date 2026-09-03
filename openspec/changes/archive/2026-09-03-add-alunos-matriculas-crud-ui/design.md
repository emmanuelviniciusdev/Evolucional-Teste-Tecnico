## Context

See `proposal.md` for motivation and `specs/alunos-matriculas-crud-ui/spec.md` for behavior. The Web API already exposes aluno CRUD, `GET /api/turmas`, `POST /api/matriculas`, and `GET /api/relatorios/alunos-por-turma` on `http://localhost:5000`. `Startup` maps `""` to the Swagger UI. IIS Express already serves static `Content` files from `Escola.Api`; there is no CORS middleware and no frontend project.

Constraints: no npm, no jQuery, no bundler, no new HTTP endpoints. Visible copy on the demonstration page is pt-BR (`lang="pt-BR"`). OpenSpec artifacts and Swagger stay en-US.

## Goals / Non-Goals

**Goals:**

- One static HTML demonstration page the IIS Express host serves on the same origin as `/api/*`.
- Vanilla `fetch` against existing camelCase JSON contracts; show `{ "error": "..." }` from 400/404/409.
- Tailwind utility classes without a local CSS build.
- Two sections on one page: aluno CRUD and matrícula enrollment plus relatório overview.

**Non-Goals:**

- New `GET/PUT/DELETE /api/matriculas` routes or a row-level matrícula list.
- SPA frameworks, npm, jQuery, CORS, OWIN static-file middleware.
- Browser automation (Playwright/Selenium) or a dedicated frontend test project.
- Reactivating an inactive aluno, turma CRUD, or auth.

## Decisions

### 1. Serve `/ui` as IIS static content (not a Web API controller)

Put `apps/backend/src/Escola.Api/ui/index.html` in the web project as `<Content>`. IIS Express will serve it at `http://localhost:5000/ui/index.html`. Enable `defaultDocument` `index.html` so `http://localhost:5000/ui` works. Relative `fetch('/api/...')` stays same-origin; no CORS package.

Do **not** add `Microsoft.Owin.StaticFiles` or a `UiController` that returns `StringContent`. Root `""` stays the Swagger redirect.

**Alternatives considered:** `file://` plus CORS (fragile, extra package). A second `apps/frontend` static server (two processes, CORS). Embedding HTML in a C# controller (harder to edit, not a static page).

### 2. Single HTML file, Tailwind Play CDN, vanilla JS

Keep markup, Tailwind CDN script (`https://cdn.tailwindcss.com`), and JavaScript in `ui/index.html`. That matches “one simple HTML page” and “no external dependencies” except the CDN the user asked for. No `package.json`. `fetch` + `FormData` / JSON body; `input type="date"` for `dataNascimento` (`YYYY-MM-DD`).

**Alternatives considered:** Sibling `app.js` (fine, but extra file for a simple page). Local copied Tailwind CSS (large, not actually Tailwind). jQuery (user asked for vanilla JS).

### 3. Layout: two stacked sections, inline aluno form

One column, pt-BR headings **Alunos** and **Matrículas**.

Alunos: filter input (`nome`), table (`nome`, `email`, `dataNascimento`), `total` plus previous/next using `pagina` / `tamanhoPagina` (default 10). Create/edit uses one form: empty = `POST /api/alunos`; filled hidden `id` = `PUT /api/alunos/{id}`. **Excluir** asks `confirm(...)` in pt-BR then `DELETE`. Success/error banner in pt-BR; API `error` is already pt-BR.

Matrículas: `<select>` for aluno and turma, button **Matricular** → `POST /api/matriculas`. Below, a table from `GET /api/relatorios/alunos-por-turma`. After HTTP 201, reload turmas and the relatório.

**Alternatives considered:** Separate HTML files per resource (extra navigation). Modal-only edit (more JS for little gain). Tabs (hides matrículas on first paint).

### 4. Enrollment selects vs paginated `GET /api/alunos`

`GET /api/alunos` is paged (max `tamanhoPagina` 100). For the aluno `<select>`, request `tamanhoPagina=100` and walk `pagina` until `alunos.length` covers `total`. Turmas are a full array from `GET /api/turmas`; label each option with nome, período, and `vagasDisponiveis`. Disable turmas with 0 seats in the select (API still enforces 409).

**Alternatives considered:** Only the current aluno table page in the select (misses alunos on other pages). New unpaged list endpoint (out of scope).

### 5. Document the URL in both READMEs

Add a short **Tela de demonstração** section to `apps/backend/README.md` (pt-BR) with `http://localhost:5000/ui`. Point at it from the root README. No Makefile target; `make api-run` is enough.

## Risks / Trade-offs

- [Tailwind Play CDN is not for production] → Acceptable for a local demonstration page; the page still works if the browser can reach the CDN.
- [Offline review cannot load Tailwind] → Markup and behavior still work unstyled; do not block CRUD on CSS.
- [Aluno select capped at walking pages of 100] → Fine for seed data; no new API.
- [No matrícula cancellation] → Matches the API; the relatório is the enrollment view.
- [IIS may 403 `/ui` without a default document] → Set `defaultDocument` and document `/ui/index.html` as fallback.

## Migration Plan

No data migration. Add `ui/index.html`, include it in the csproj, tweak `Web.config` default document, update READMEs. Rollback: delete the `ui` folder and README sentences; Swagger and APIs stay unchanged.

## Open Questions

None. Path `/ui`, Tailwind CDN, and relatório-as-matrícula-list are recorded above.
