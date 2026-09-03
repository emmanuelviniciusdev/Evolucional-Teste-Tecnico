## Why

The API is only reachable through Swagger and curl. A same-origin demonstration page that lists and edits alunos and creates matrículas shows the existing endpoints working in a browser, without a SPA toolchain.

## What Changes

- Add a single static HTML demonstration page (vanilla JavaScript, Tailwind via CDN, no npm/jQuery/bundler) served by the Web API host.
- The page MUST be entirely in pt-BR (labels, buttons, empty states, validation hints, and displayed API error messages).
- Alunos: list (paginated, optional name filter), create, edit, and logical delete against the existing CRUD endpoints.
- Matrículas: enroll an aluno in a turma (`POST /api/matriculas`) using selects fed by `GET /api/alunos` and `GET /api/turmas`; show enrollment totals via `GET /api/relatorios/alunos-por-turma` (the API has no matrícula list/update/delete).
- Document the page URL in `apps/backend/README.md` and the root README (pt-BR).

## Capabilities

### New Capabilities

- `alunos-matriculas-crud-ui`: Same-origin demonstration page in pt-BR for aluno CRUD and matrícula enrollment, using the existing API endpoints.

### Modified Capabilities

- `local-dev-environment`: Runbooks MUST tell how to open the demonstration page after `make api-run`.

## Impact

- New static files under `apps/backend/src/Escola.Api` (HTML plus optional sibling JS), included as web content so IIS Express serves them.
- Root `/` stays the Swagger redirect; the page lives on a dedicated path such as `/ui`.
- No new HTTP contracts, no CORS package, no npm, no jQuery.
- Tailwind Play CDN (`cdn.tailwindcss.com`) is the only third-party browser resource.
- README files (pt-BR) gain a short section with the page URL.
- Automated UI tests are out of scope; an optional integration smoke that `GET /ui` returns HTML MAY be added.
