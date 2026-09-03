# frontend-foundation Specification

## Purpose

Defines the React product-catalog SPA foundation: a TypeScript app shell with accessibility baselines, a centralized typed HTTP client for json-server `/produtos`, routed placeholders for the assignment screens, and unit, integration, and Playwright e2e harnesses—without listing, detail, or CRUD behavior yet.

## Requirements

### Requirement: React TypeScript SPA host

The frontend SHALL be a single-page application written in TypeScript and React 19 (latest stable 19.x) located under `apps/frontend`. It MUST use Vite 8 as the development and production bundler. It MUST NOT be a Next.js, CRA, or JavaScript-only app. TypeScript MUST be configured in strict mode (`strict` enabled). The app MUST render in the browser without calling `apps/backend`.

#### Scenario: Host is React 19 with TypeScript

- **WHEN** a reviewer inspects `apps/frontend` package manifest and TypeScript config
- **THEN** the app depends on React 19.x, is written in TypeScript with `strict` enabled, and uses Vite 8 as the bundler

#### Scenario: App is independent of the school API

- **WHEN** the frontend is started with the documented scripts
- **THEN** it does not request `http://localhost:5000` or any `apps/backend` route

### Requirement: Accessible application shell

The SPA SHALL render an application shell on every route. The document language MUST be `pt-BR`. Visible copy (title, headings, navigation, skip link, placeholder body text) MUST be pt-BR. The shell MUST expose a skip link that moves keyboard focus to the main content, page landmarks (`header`/`nav`/`main`, or equivalent ARIA), a unique document title per route, a visible focus indicator on interactive elements, and a color contrast of at least WCAG 2.2 AA for text and UI. When the user prefers reduced motion, the shell MUST NOT play decorative animation. Automated accessibility checks on the shell MUST report no serious or critical violations.

#### Scenario: Document language and copy are Portuguese

- **WHEN** any shell route is opened
- **THEN** `html lang` is `pt-BR` and headings, navigation, skip link, and placeholder text are in Portuguese

#### Scenario: Keyboard user can skip to main content

- **WHEN** a keyboard user tabs from the start of the page
- **THEN** the first focusable control is a skip link, activating it moves focus into `main`, and that main region is present

#### Scenario: Automated a11y check on the shell

- **WHEN** the documented accessibility check runs against the shell at `/`
- **THEN** it reports no serious or critical violations

### Requirement: Routed placeholders without product CRUD

The SPA SHALL use client-side routing with URLs for the assignment screens: product list (`/`), product detail (`/produtos/:id`), create (`/produtos/novo`), and edit (`/produtos/:id/editar`). Each of those routes MUST render the shell plus a working product screen as defined by the `product-listing`, `product-detail`, `product-form`, and `product-delete` capabilities. The list route MUST render the paginated, searchable product list backed by the API. Unknown paths MUST render a pt-BR not-found state inside the shell.

#### Scenario: List route renders the product list

- **WHEN** the user opens `/`
- **THEN** the shell is shown with the paginated product list populated from the API

#### Scenario: Other assignment routes render working screens

- **WHEN** the user opens `/produtos/novo`, `/produtos/1`, or `/produtos/1/editar`
- **THEN** each URL renders the shell and the corresponding working product screen (form, detail, or edit)

#### Scenario: Unknown path

- **WHEN** the user opens a path that is not one of the assignment routes
- **THEN** the page shows a pt-BR not-found message inside the shell

### Requirement: Centralized typed product API client

All HTTP calls to the fake API MUST go through a single typed client. The client MUST model `Produto` with `id`, `nome`, `categoria`, `preco`, `estoque`, and `ativo` matching `Enunciado/React/db.json`. It MUST expose operations for list (including page, limit, name search, and category filter query params and `X-Total-Count`), get-by-id, create, update, and delete against `/produtos`. UI components MUST NOT call `fetch` or another HTTP API directly. This change MUST NOT wire those operations to the placeholder screens.

#### Scenario: Product type matches the fake API

- **WHEN** a reviewer inspects the client types
- **THEN** `Produto` includes `id`, `nome`, `categoria`, `preco`, `estoque`, and `ativo`

#### Scenario: Screens do not fetch inline

- **WHEN** a reviewer inspects React components added in this change
- **THEN** none of them call `fetch` or a raw HTTP client; only the centralized module talks to `/produtos`

#### Scenario: List contract is ready for later screens

- **WHEN** the list operation is invoked with page, limit, and optional name or category
- **THEN** the client requests `/produtos` with the corresponding json-server query params and returns items plus the total from `X-Total-Count`

### Requirement: Language conventions

Domain names and user-facing strings in implementation MUST be written in pt-BR (`Produto`, `categoria`, catalog copy). Technical identifiers MUST be written in en-US (`ProductApi`, `getProducts`, `AppShell`). OpenSpec artifacts MUST be written in en-US. Operator and developer documentation (README) MUST be written in pt-BR, keeping technical terms in English.

#### Scenario: Domain is pt-BR and technical names are en-US

- **WHEN** a reviewer inspects frontend types and modules added in this change
- **THEN** the product model uses Portuguese field names from the API and technical modules use English names (for example `getProducts`, not `buscarProdutos`)

#### Scenario: Project docs are pt-BR; OpenSpec is en-US

- **WHEN** a reviewer inspects README files and OpenSpec artifacts added in this change
- **THEN** README and operator runbooks are in Brazilian Portuguese with technical terms in English, and OpenSpec specs remain in English (United States)

### Requirement: Unit and integration tests

The frontend SHALL include a unit test suite and an integration test suite. Unit tests MUST run in isolation (no live json-server) and MUST include at least one passing smoke that the shell renders with `lang="pt-BR"` and a main landmark. Integration tests MUST exercise the typed API client (and MAY render a route) against a mocked HTTP layer, proving list parsing including `X-Total-Count`, and MUST NOT require json-server to be running. Assignment product-screen tests (search, pagination UI, forms, delete confirm) MUST wait for later changes.

#### Scenario: Unit smoke passes without the fake API

- **WHEN** unit tests are executed
- **THEN** the suite compiles, does not start json-server, and the shell smoke test passes

#### Scenario: Integration smoke proves the client contract

- **WHEN** integration tests are executed without json-server
- **THEN** the suite compiles, the client list operation returns items and a total derived from `X-Total-Count`, and at least one such test passes

### Requirement: Playwright end-to-end tests

The frontend SHALL include Playwright end-to-end tests that drive a real browser against the running Vite app and the live json-server. At least one smoke MUST pass: open `/`, assert the shell is visible in pt-BR, and run an accessibility scan with no serious or critical violations. E2E tests MUST NOT target `apps/backend`. Full assignment flows (search, pagination, create, edit, delete) MUST NOT be required in this change.

#### Scenario: Playwright smoke against the live app

- **WHEN** Playwright e2e tests run with the documented app and json-server processes
- **THEN** `/` loads the pt-BR shell and the accessibility scan reports no serious or critical violations

#### Scenario: E2E does not use the school API

- **WHEN** Playwright tests execute
- **THEN** they do not navigate to or assert against `http://localhost:5000`
