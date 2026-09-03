## 1. Vite + React 19 skeleton

- [x] 1.1 Create `apps/frontend` with Vite 8, React 19.x, TypeScript `strict`, path alias `@/` → `src/`, and `engines.node` `>=20.19.0`, then verify `package.json` lists `react` 19.x and `vite` 8.x and `tsc --noEmit` succeeds
- [x] 1.2 Configure Vite port 5173 and proxy `/produtos` to `http://localhost:3001`, then verify `vite.config.ts` contains that proxy and no reference to `localhost:5000`

## 2. Accessible shell and placeholder routes

- [x] 2.1 Add CSS tokens (`--bg` `#e7ece8`, `--ink`, `--accent` `#0f6e6a`, `--focus` ring 3px/2px offset, `--radius` 8px), Schibsted Grotesk + IBM Plex Mono, and `prefers-reduced-motion`, then verify `index.html` has `lang="pt-BR"` and focus styles are not `outline: none` without a replacement
- [x] 2.2 Implement `AppShell` with skip link, `header`/`nav`/`main`, Nexo pt-BR copy, and React Aria Button/Link, then verify the skip link is the first focusable control and activating it focuses `main`
- [x] 2.3 Add React Router 7 routes `/`, `/produtos/novo`, `/produtos/:id`, `/produtos/:id/editar`, and a catch-all not-found, each with a unique pt-BR `document.title` and placeholder (no table, form, or API-backed cards), then verify those URLs render placeholders and an unknown path shows a pt-BR not-found inside the shell

## 3. Typed product API client

- [x] 3.1 Add `Produto` (`id`, `nome`, `categoria`, `preco`, `estoque`, `ativo`) and `listProducts` / `getProduct` / `createProduct` / `updateProduct` / `deleteProduct` in `src/shared/api`, mapping `_page`/`_limit`/`nome_like`/`categoria` and `X-Total-Count`, then verify no React page component imports `fetch`
- [x] 3.2 Leave placeholder pages unwired to the client, then verify `/` still shows no product rows

## 4. Unit and integration tests

- [x] 4.1 Add Vitest + Testing Library + jsdom + `vitest-axe` with `test:unit`, and a shell smoke that asserts `lang="pt-BR"`, a `main` landmark, and no serious/critical axe violations, then verify `npm run test:unit` passes without json-server
- [x] 4.2 Add an integration Vitest project with MSW and `test:integration` that stubs `GET /produtos` with `X-Total-Count` and asserts `listProducts` returns items plus total, then verify `npm run test:integration` passes without json-server
- [x] 4.3 Add `test` that runs unit + integration only (not Playwright), then verify `npm test` does not launch a browser

## 5. Playwright e2e

- [x] 5.1 Add Playwright (Chromium) + `@axe-core/playwright`, `playwright.config.ts` with `webServer` starting json-server and Vite, and `test:e2e`, then verify `npx playwright install chromium` is documented and the config baseURL is port 5173 not 5000
- [x] 5.2 Add `e2e/shell.spec.ts` that opens `/`, asserts pt-BR shell copy, and fails on axe serious/critical, then verify `npm run test:e2e` passes

## 6. Fake API, lint, and docs

- [x] 6.1 Copy `Enunciado/React/db.json` to `apps/frontend/db.json`, pin `json-server@0.17.4`, and add scripts `api`, `dev` (API + Vite), then verify `GET http://localhost:3001/produtos?_page=1&_limit=10` returns seed data and `X-Total-Count` after `npm run api`
- [x] 6.2 Add ESLint flat config with `jsx-a11y/recommended` as errors and script `lint`, then verify `npm run lint` passes on the shell and fails if an `img` without `alt` is introduced
- [x] 6.3 Write `apps/frontend/README.md` in pt-BR (stack, Node 20.19+, install, api, dev, test:unit, test:integration, test:e2e, Playwright Chromium, screens not done) and update the root README to point at both apps without claiming CRUD screens exist, then verify both files are pt-BR and state list/detail/create/edit/delete are later work

