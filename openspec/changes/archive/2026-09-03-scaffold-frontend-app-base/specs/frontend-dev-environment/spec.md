## Purpose

Defines how developers install Node dependencies, start json-server from the React assignment `db.json`, run the Vite SPA and all frontend test suites, and follow pt-BR README instructions without using the school-enrollment backend.

## ADDED Requirements

### Requirement: Assignment fake API on port 3001

Local product data MUST come from json-server 0.17.x watching a copy of `Enunciado/React/db.json`. The process MUST listen on port 3001 and expose `/produtos` with GET, POST, PUT, and DELETE. Pagination (`_page`, `_limit`, `X-Total-Count`), name search (`nome_like`), and category filter (`categoria`) MUST work as in the assignment. The fake API MUST NOT require Docker or SQL Server.

#### Scenario: Fake API serves seeded products

- **WHEN** a developer starts the documented json-server command and calls `GET http://localhost:3001/produtos`
- **THEN** the response is JSON including the assignment seed products (for example "Teclado Mecanico TKL")

#### Scenario: Pagination header is present

- **WHEN** a client calls `GET http://localhost:3001/produtos?_page=1&_limit=10`
- **THEN** the response includes header `X-Total-Count` with the total number of matching products

### Requirement: npm scripts for app, API, lint, and tests

`apps/frontend` MUST provide named npm scripts that install is unnecessary to invent: start the fake API, start the Vite app, start API and app together for local work, run unit tests, run integration tests, run Playwright e2e tests, and run ESLint. Script names MUST be documented in the frontend README. Playwright MUST install its browsers through the documented setup so e2e can run on a clean machine.

#### Scenario: Developer can start API and app

- **WHEN** a developer runs the documented combined start script after `npm install`
- **THEN** json-server is reachable on port 3001 and the SPA is reachable on the documented Vite port

#### Scenario: Test scripts are dedicated

- **WHEN** a developer runs the unit, integration, or e2e script
- **THEN** the corresponding suite executes without requiring ad-hoc CLI flags beyond what the README lists

### Requirement: Accessibility lint gate

The frontend MUST fail ESLint when JSX introduces basic accessibility defects covered by `jsx-a11y` recommended rules (for example an `img` without an accessible name, or a clickable non-interactive element). Lint MUST be runnable via the documented npm script.

#### Scenario: Missing accessible name fails lint

- **WHEN** an `img` is added without `alt` and ESLint runs
- **THEN** the lint script exits non-zero

### Requirement: pt-BR runbooks

`apps/frontend/README.md` MUST be written in pt-BR (technical terms in English) and MUST explain what the app is, the stack (React 19, TypeScript, Vite 8), Node version (20.19+), how to install dependencies, how to start json-server, how to start the SPA, how to run unit, integration, and Playwright e2e tests, and that product listing/detail/CRUD screens are not implemented yet. The root README MUST point at that runbook, MUST mention both the school-enrollment backend and this React app, and MUST NOT claim that the product management screens are complete.

#### Scenario: README is enough to run locally

- **WHEN** a developer follows `apps/frontend/README.md` on a machine with Node.js 20.19+
- **THEN** they can install, start the fake API, start the SPA, open the shell in a browser, and run unit, integration, and e2e tests without undocumented steps

#### Scenario: Missing screens are explicit

- **WHEN** a developer reads `apps/frontend/README.md` and the root README
- **THEN** both describe that the React app foundation is in place and that list, detail, create, edit, and delete screens are later work, and they MUST NOT claim those screens are done
