## Why

The frontend foundation is in place (shell, routing placeholders, typed API client, test harnesses), but all product screens are still empty placeholders. The assignment requires a fully working product management panel before the technical review conversation; without it the deliverable is incomplete.

## What Changes

- Replace the product list placeholder (`/`) with a real listing screen: server-side pagination, name search with debounce, category filter, loading / error / empty states, and a total count display.
- Replace the product detail placeholder (`/produtos/:id`) with a screen that fetches and displays all product fields from the API.
- Replace the create/edit placeholders (`/produtos/novo`, `/produtos/:id/editar`) with a validated form (name ≥ 3 chars, price > 0, stock ≥ 0) that saves and shows per-field error messages and a success feedback.
- Replace the implicit delete path with a delete action on the list or detail screen behind a confirmation dialog.
- Add `React Router` URL reflection so page number, search term, and category filter survive a browser reload.
- Add unit/integration tests for at least one screen (React Testing Library) and add a full Playwright E2E suite with one test per assignment feature: listing (pagination, search, filter, URL state), product detail, create, edit, and delete with confirmation.

## Capabilities

### New Capabilities

- `product-listing`: Server-side paginated product list with name search (debounce), category filter, URL-reflected state, and loading / error / empty feedback.
- `product-detail`: Product detail screen that loads a single product by id and displays all its fields.
- `product-form`: Create and edit product form with field-level validation (name ≥ 3 chars, price > 0, stock ≥ 0), per-field error messages, and success feedback after save.
- `product-delete`: Delete action with a confirmation dialog; removes the product and redirects or refreshes the list.

### Modified Capabilities

- `frontend-foundation`: The routed placeholders requirement is superseded—each formerly-empty route now renders a working screen. No other foundation requirements change.

## Impact

- `apps/frontend/src/pages/` — `Home.tsx`, `NotFound.tsx` extended; new page components added.
- `apps/frontend/src/shared/api/products.ts` — existing typed client is wired to UI; no contract changes.
- `apps/frontend/src/` — new components, hooks, and form logic added alongside existing files.
- `apps/frontend/e2e/` — full Playwright E2E suite covering all assignment flows against the live Vite app and json-server.
- `apps/frontend/src/test/` — new React Testing Library tests added.
- No changes to `apps/backend`, OpenAPI contracts, or infrastructure.
