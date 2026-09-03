## Why

The React practical test (`Enunciado/React`) needs a product-management SPA, but the repo only has the school-enrollment API and its static `/ui` page. Existing main specs (`backend-foundation`, `aluno-crud`, `alunos-matriculas-crud-ui`, and the rest) describe that C# product and MUST stay unchanged. This change delivers only the React app foundation—stack, shell, a11y baseline, API client, and test harnesses—so later changes can add listing, detail, and CRUD on a working, accessible UI.

## What Changes

- Add a TypeScript React SPA under `apps/frontend` using the latest stable React 19 and Vite 8 (strict TypeScript).
- Ship an accessible app shell (pt-BR copy, landmarks, skip link, focus, contrast) with a distinctive modern visual language and routed placeholders for the assignment screens—no product list/search/pagination, detail, create/edit, or delete yet.
- Centralize HTTP access in one typed API client (`Produto` and json-server `/produtos`) so screens never call `fetch` directly later.
- Add unit tests (Vitest + Testing Library), integration tests (Testing Library + MSW against the client and shell), and Playwright e2e tests against the running Vite app and json-server, each with at least one passing smoke (including an axe check on the shell).
- Add json-server (assignment `db.json`, port 3001), npm scripts, ESLint with `jsx-a11y`, and a pt-BR frontend README; point the root README at both apps.

## Capabilities

### New Capabilities

- `frontend-foundation`: Vite + React 19 + TypeScript SPA with accessible shell, routed placeholders, typed centralized API client, unit and integration test projects, and Playwright e2e against the live app and fake API.
- `frontend-dev-environment`: json-server from the assignment `db.json`, documented npm scripts to run the fake API and the SPA, ESLint a11y gating, and pt-BR runbooks (frontend README plus root README pointer).

### Modified Capabilities

- None. School-enrollment specs (`backend-foundation`, `local-dev-environment`, `aluno-crud`, `turma-listing`, `matricula`, `relatorio-alunos-por-turma`, `alunos-matriculas-crud-ui`) are a different product and HTTP surface; this change MUST NOT alter their requirements.

## Impact

- New tree under `apps/frontend` (Vite app, tests, Playwright config, copied or referenced `db.json`).
- New npm dependencies (React 19, Vite 8, React Router, React Aria Components, Vitest, Testing Library, MSW, Playwright, axe). Node.js 20.19+ (Vite 8).
- Root `README.md` must describe both the backend test and this frontend app without implying the product CRUD screens already exist.
- json-server on `http://localhost:3001` is the only backend for this app; it MUST NOT call `apps/backend`.
- No assignment product UI behavior in this change beyond a shell and placeholders.
