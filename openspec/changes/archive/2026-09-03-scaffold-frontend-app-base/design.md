## Context

See `proposal.md` for motivation. The repo has `Enunciado/React` (instructions + `db.json`) and a complete `apps/backend` for a different assignment. There is no `apps/frontend`. Specs in `specs/frontend-foundation` and `specs/frontend-dev-environment` fix the React 19 / Vite 8 / TypeScript host, accessible shell, placeholder routes, typed `/produtos` client, unit + integration + Playwright e2e harnesses, json-server 0.17.x on port 3001, and pt-BR READMEs.

Constraints that shape the approach:

- Assignment stack: React 18+ (we take latest 19.x), Vite allowed, TypeScript preferred, centralized API access, json-server 0.17.4 on port 3001.
- This change is foundation only: no list/search/filter/pagination UI, no detail payload, no forms, no delete confirm.
- School-enrollment specs and `apps/backend` stay untouched.
- Accessibility is a gate (lint + axe in unit/e2e), not a later polish pass.

## Goals / Non-Goals

**Goals:**

- A Vite + React 19 + TypeScript app under `apps/frontend` that a reviewer can `npm install` and open.
- App shell with a distinctive visual language, WCAG 2.2 AA, React Aria primitives ready for later dialogs/comboboxes.
- Typed `Produto` client that already speaks json-server pagination and filters, unused by placeholder pages.
- Three proven harnesses: Vitest unit, Vitest integration + MSW, Playwright e2e + `@axe-core/playwright`.

**Non-Goals:**

- Product listing, debounce search, category filter UI, pagination controls, detail view, create/edit validation, delete confirmation.
- Calling `apps/backend` or replacing the static `/ui` demo page.
- Next.js, CRA, MUI/Ant default themes, or a component library whose default look is the product UI.
- Docker for the frontend or json-server.

## Decisions

### 1. App layout and naming

```
apps/frontend/
  package.json
  vite.config.ts
  playwright.config.ts
  index.html
  public/
  src/
    main.tsx
    app/                 # router, providers, shell
    pages/               # placeholder routes + not-found
    shared/
      api/               # Produto types + HTTP client
      ui/                # tokens, layout, skip link, primitives
      a11y/
    test/
      setup.ts
      msw/
  e2e/
    shell.spec.ts
  db.json                # copy of Enunciado/React/db.json
  README.md
```

Technical folders stay English (`shared/api`, `e2e`). Domain type is `Produto` with API field names. Brand copy in the shell uses a short catalog name (**Nexo**) so the UI is not a generic “Admin Dashboard”.

**Alternatives considered:** Colocate under `apps/backend` (wrong product). Next.js app router (assignment is a SPA against json-server; extra SSR is noise). Keep `db.json` only in `Enunciado/` (fragile relative paths for scripts; copy and note the source in the README).

### 2. Stack: Vite 8 + React 19 + TypeScript strict

Scaffold with `npm create vite@latest` React + TypeScript template, then pin React 19.x and Vite 8.x. `tsc --noEmit` in CI-equivalent npm `typecheck`. `@vitejs/plugin-react` v6. Path alias `@/` → `src/`. Dev server port **5173**. Vite proxy `/produtos` → `http://localhost:3001` so the browser stays same-origin and Playwright does not fight CORS.

**Alternatives considered:** CRA (assignment allows it; unmaintained). Next.js (SSR unused). JavaScript (user required TypeScript).

### 3. Routing: React Router 7

Browser router with routes `/`, `/produtos/novo`, `/produtos/:id`, `/produtos/:id/editar`, and a catch-all. Register `/produtos/novo` before `:id`. Each route sets `document.title` (pt-BR). Placeholders are small page components, not one 800-line file.

**Alternatives considered:** No router until CRUD (rejected: assignment bonus and later URL state for page/search need the shell now). File-based routing (not needed).

### 4. UI: custom tokens + React Aria Components

Do not ship MUI/Ant/chakra defaults. Tokens as CSS variables on `:root`:

| Token | Value | Role |
| --- | --- | --- |
| `--bg` | `#e7ece8` | cool workshop paper (not white, not cream) |
| `--ink` | `#12151a` | primary text |
| `--accent` | `#0f6e6a` | teal actions (not purple, not orange) |
| `--focus` | `#0b8f78` | 3px focus ring, offset 2px |
| `--danger` | `#9b1d2a` | reserved for later errors |
| `--radius` | `8px` | controls; never `999px` pills |

Fonts: **Schibsted Grotesk** (UI) + **IBM Plex Mono** (ids, prices later). One motion: 160ms fade/translate on route main only; honor `prefers-reduced-motion`. Shell: skip link, `header` with product-area nav, `main` with `h1`. Future lists use a table, not card grids.

React Aria Components for Link/Button now so later Dialog (delete confirm) and ComboBox (category) inherit a11y. No `outline: none` without the `--focus` ring.

**Alternatives considered:** shadcn/ui defaults (generic AI look). MUI (assignment-ok but fights custom a11y/visual). Raw HTML only (dialogs later would be hand-rolled a11y).

### 5. API client

One module (`shared/api/products.ts`) wrapping `fetch` to `/produtos`. Functions: `listProducts({ page, limit, nome, categoria })`, `getProduct(id)`, `createProduct`, `updateProduct`, `deleteProduct`. Parse `X-Total-Count`. Throw a typed error on non-OK. Base URL from `import.meta.env.VITE_API_BASE` defaulting to `''` (proxy) in the browser and `http://localhost:3001` in Node if ever needed. No React Query in this change (YAGNI until list screen).

**Alternatives considered:** OpenAPI generator (json-server has no schema). Axios (fetch is enough). Scatter fetch in pages (assignment forbids).

### 6. Tests: Vitest unit, Vitest+MSW integration, Playwright e2e

| Layer | Tool | Proves | Live json-server |
| --- | --- | --- | --- |
| Unit | Vitest + Testing Library + jsdom + `vitest-axe` | Shell renders, landmarks, `lang` | No |
| Integration | Vitest + MSW | Client list + `X-Total-Count` | No |
| E2E | Playwright + `@axe-core/playwright` | Real browser, `/`, axe serious/critical = 0 | Yes |

Scripts: `test:unit`, `test:integration` (vitest project filter), `test:e2e` (`playwright test`). Playwright `webServer` starts `npm run dev:e2e` (concurrent json-server + vite) unless `reuseExistingServer`. Browsers: Chromium only for the smoke (fast on Windows). `axe` source: same WCAG 2.2 AA tags.

**Alternatives considered:** Playwright for all tests (slow, assignment asks RTL). Cypress (user required Playwright). Only RTL (user required e2e).

### 7. json-server and lint

Pin `json-server@0.17.4` as a devDependency; `db.json` copied into `apps/frontend`. Script `api` → `json-server --watch db.json --port 3001`. `dev` → `concurrently` api + vite. ESLint flat config: `typescript-eslint` + `plugin:jsx-a11y/recommended` as errors. `engines.node`: `>=20.19.0`.

**Alternatives considered:** json-server 1.x (assignment specifies 0.17.4 query/header behavior). Proxy-only without documenting 3001 (reviewer follows the enunciado).

## Risks / Trade-offs

- [json-server 0.17 vs 1.x] → Pin 0.17.4 so `_page`/`_limit`/`X-Total-Count` match the assignment, not 1.x wrapping.
- [Playwright lengthening `npm test`] → Keep default `test` = unit+integration; e2e is `test:e2e` so local loops stay fast.
- [Axe noise on third-party CSS] → Scan only `main` + shell; fail on serious/critical, not minor.
- [Proxy hides CORS mistakes] → E2E hits Vite origin; README still documents raw `:3001` for curl.
- [Placeholder routes look “unfinished” to a reviewer] → README states foundation-only; `h1` copy says catalog screens come next.

## Migration Plan

New tree only. Rollback: delete `apps/frontend` and revert root README pointers. No data migration. Playwright browsers are local (`npx playwright install chromium`); they are not committed.
