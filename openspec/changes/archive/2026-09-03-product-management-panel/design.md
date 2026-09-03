## Context

The frontend foundation provides an application shell, React Router route placeholders, and a centralized typed `ProductApi` client (`apps/frontend/src/shared/api/products.ts`). All data-fetching and state are wired to placeholders. `json-server 0.17` at `http://localhost:3001` is the only API backend; it supports `_page`, `_limit`, `nome_like`, `categoria` query parameters and returns the total record count in `X-Total-Count`. See `proposal.md` for the full motivation.

## Goals / Non-Goals

**Goals:**
- Wire all four assignment screens (list, detail, create/edit, delete) to the existing `ProductApi` client.
- Introduce a consistent data-fetching pattern via custom React hooks so no component calls the API directly.
- Keep browser URL in sync with list state (page, search, category) using React Router's `useSearchParams`.
- Enforce form validation inline without a dedicated validation library (or with a lightweight one such as `react-hook-form`).
- Extend the existing unit / integration / Playwright harnesses with at least one new test per screen type.

**Non-Goals:**
- Changing the `ProductApi` client's HTTP contract or the `Produto` type.
- Introducing a global state manager (Redux, Zustand, etc.); React state + URL params are sufficient.
- Adding new infrastructure or CI jobs.
- Implementing authentication or role-based access.

## Decisions

### 1. Data-fetching via custom hooks, not inline in components

**Decision**: Each screen's data-fetching logic lives in a dedicated custom hook (`useProducts`, `useProduct`, `useSaveProduct`, `useDeleteProduct`) that calls `ProductApi` and returns `{ data, loading, error }`. Components remain presentational.

**Why**: The assignment explicitly requires centralized API calls and evaluates componentization. Co-locating fetch logic in hooks keeps components small, makes mocking trivial in tests, and avoids prop-drilling.

**Alternative considered**: React Query / SWR. Rejected to keep the dependency footprint small and to demonstrate custom hook patterns, which are easier to explain in a live technical review.

### 2. URL state with React Router `useSearchParams`

**Decision**: The list screen syncs `page`, `q` (search term), and `categoria` into the URL via `useSearchParams`. Changing any of the three updates the URL without a full navigation, and the hook reads initial values from the URL on mount.

**Why**: Satisfies the "URL reflects state" bonus requirement and makes the list bookmarkable and shareable with no extra library.

**Alternative considered**: `useState` only. Rejected because it would not survive a browser reload.

### 3. Debounce implemented with a `useDebounce` utility hook

**Decision**: A small `useDebounce<T>(value: T, delay: number)` hook delays propagating the search term to the API query. Delay is 300 ms as the spec mandates.

**Why**: Keeps the debounce logic reusable and independent of the input component. No external library needed.

**Alternative considered**: `lodash.debounce` on the event handler. Rejected because it requires careful cleanup on unmount and is harder to test in isolation.

### 4. Form handling with `react-hook-form`

**Decision**: The create/edit form uses `react-hook-form` for field registration, validation, and error state.

**Why**: `react-hook-form` provides field-level error messages, uncontrolled inputs (better performance), and a small bundle footprint (~9 kB). It avoids writing boilerplate validation state by hand and is already a common industry choice the reviewer is likely to recognize.

**Alternative considered**: Fully manual `useState`-based validation. Acceptable but produces more boilerplate and is more error-prone.

### 5. Delete confirmation via a modal dialog

**Decision**: The delete control opens a small inline confirmation dialog (a `<dialog>` element or a styled overlay component). The DELETE request is only sent after the user confirms.

**Why**: Required by the spec. Using the native `<dialog>` element keeps accessibility (focus trap, `Escape` closes) with minimal code and no additional dependency.

**Alternative considered**: `window.confirm`. Rejected because it is not styled, not accessible in all contexts, and cannot be easily tested with Testing Library.

### 6. Success feedback via a transient in-page message

**Decision**: After a successful save or delete the form/list renders a pt-BR success banner that auto-dismisses after a few seconds, or persists until the user navigates away.

**Why**: The spec requires feedback that does not use a generic `alert`. A simple rendered message satisfies the requirement and is easy to unit-test.

## Risks / Trade-offs

- **`X-Total-Count` header availability in tests** → MSW (already wired) MUST return this header in mocked list responses; integration tests already verify this from the foundation.
- **`react-hook-form` version mismatch** → Pin to a recent stable v7.x. If it conflicts with existing dependencies, fall back to manual validation.
- **URL search params and React Router v6 API** → React Router v6 `useSearchParams` replaces `history.push`; confirm project is on v6 (it is, per the foundation scaffold) before coding.
- **Category list source** → The assignment does not provide an API endpoint for categories. The category filter control MUST derive the list of categories either from `db.json` constants or dynamically from a full product fetch on mount. Decision: use a static list matching `db.json` values to avoid an extra API call on every page load; this means a new category added to json-server after the app loads will not appear until the constants are updated.

## Open Questions

- None. All material decisions are resolved above.
