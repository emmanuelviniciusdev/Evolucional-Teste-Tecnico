## 1. Dependencies and Utilities

- [x] 1.1 Add `react-hook-form` to `apps/frontend/package.json` and verify `npm install` succeeds without peer-dependency errors.
- [x] 1.2 Create `apps/frontend/src/shared/hooks/useDebounce.ts` with a generic `useDebounce<T>(value, delay)` hook and verify a unit test for it passes (input value should not update until the delay elapses).

## 2. Data-Fetching Hooks

- [x] 2.1 Create `apps/frontend/src/shared/hooks/useProducts.ts` wrapping `ProductApi.list`; it MUST accept page, limit, search, and category params and return `{ data, total, loading, error }`; verify an integration test (using existing MSW setup) passes covering the list and the `total` from `X-Total-Count`.
- [x] 2.2 Create `apps/frontend/src/shared/hooks/useProduct.ts` wrapping `ProductApi.getById`; returns `{ data, loading, error }`; verify a unit test covers loading and error states.
- [x] 2.3 Create `apps/frontend/src/shared/hooks/useSaveProduct.ts` encapsulating create (POST) and update (PUT) via `ProductApi`; returns `{ save, loading, error, success }`; verify a unit test covers the success and error branches.
- [x] 2.4 Create `apps/frontend/src/shared/hooks/useDeleteProduct.ts` wrapping `ProductApi.delete`; returns `{ deleteProduct, loading, error }`; verify a unit test covers the success path.

## 3. Shared UI Components

- [x] 3.1 Create `apps/frontend/src/shared/components/LoadingSpinner.tsx` (or equivalent indicator) used by all screens; verify it renders in a smoke unit test.
- [x] 3.2 Create `apps/frontend/src/shared/components/ErrorMessage.tsx` that accepts a `message` prop and renders a visible error; verify it renders in a unit test.
- [x] 3.3 Create `apps/frontend/src/shared/components/ConfirmDialog.tsx` using the native `<dialog>` element with confirm and cancel actions; verify a unit test confirms the dialog renders when `open` is true and fires `onConfirm`/`onCancel` correctly.

## 4. Product Listing Screen

- [x] 4.1 Replace the `Home.tsx` placeholder with the product list screen: render a table or card list from `useProducts`, show name, category, price, stock, and a link/button to each product's detail; verify products from MSW appear in an integration test.
- [x] 4.2 Wire the name search input with 300 ms debounce (via `useDebounce`) and connect to `useProducts`; verify an integration test that typing a search term produces a request with `nome_like`.
- [x] 4.3 Add a category filter control (static list from `db.json` values) and connect to `useProducts`; verify an integration test that selecting a category produces a request with `categoria`.
- [x] 4.4 Sync page, search term (`q`), and category to the URL using React Router `useSearchParams`; verify that reloading a URL with query params restores the same filters in an integration or Playwright test.
- [x] 4.5 Render loading, error, and empty states in `Home.tsx` using `LoadingSpinner` and `ErrorMessage`; verify a unit test for each of the three states: loading renders the spinner, error renders the message, and empty renders an "empty" message.
- [x] 4.6 Render total record count (`X-Total-Count`) and pagination controls; controls MUST be disabled / hidden when total ≤ page size; verify an integration test covers the count display and disabled state.
- [x] 4.7 Add a delete button on each product row that opens `ConfirmDialog`; on confirm call `useDeleteProduct` and refresh the list; verify a unit test that cancel does NOT call the delete hook.

## 5. Product Detail Screen

- [x] 5.1 Create `apps/frontend/src/pages/ProductDetail.tsx` that calls `useProduct(id)` and renders all product fields (`id`, `nome`, `categoria`, `preco`, `estoque`, `ativo`); wire the `/produtos/:id` route in `AppShell.tsx`; verify a unit test that all fields are displayed.
- [x] 5.2 Render loading and error states in `ProductDetail.tsx`; verify unit tests for both states.
- [x] 5.3 Add a back link to `/` and an edit link to `/produtos/:id/editar` on the detail screen; verify navigation links are rendered with the correct `href` values.

## 6. Product Form Screen (Create and Edit)

- [x] 6.1 Create `apps/frontend/src/pages/ProductForm.tsx` using `react-hook-form`; register fields `nome`, `categoria`, `preco`, `estoque`; wire `/produtos/novo` and `/produtos/:id/editar` routes; in edit mode fetch the product with `useProduct` and populate `reset()`; verify a unit test that the form renders with empty fields for create mode.
- [x] 6.2 Implement field-level validation: `nome` required and ≥ 3 chars, `preco` required and > 0, `estoque` required and ≥ 0; display errors adjacent to each field; verify unit tests that submitting an invalid form shows the correct per-field error messages and does not call the save hook.
- [x] 6.3 On form submit call `useSaveProduct`; on success show a pt-BR success banner and navigate to `/` or the product detail; verify a unit test (with MSW) that a valid submission causes navigation to the list.
- [x] 6.4 Show loading state (disabled submit button) during save and an error banner if the API call fails; verify unit tests for both.

## 7. Tests and Quality Checks

- [x] 7.1 Add at least one React Testing Library integration test for the list screen covering: products appear, search triggers a filtered request, and pagination controls render the total count; verify the suite passes with `npm run test:integration`.
- [x] 7.2 E2E — Product listing: write a Playwright test that opens `/`, asserts at least one product row is visible, navigates to page 2, asserts the URL contains `page=2`, and asserts `X-Total-Count`-derived total is rendered; verify `npm run test:e2e` passes.
- [x] 7.3 E2E — Search, filter and URL state: write a Playwright test that types a name into the search box, waits for debounce, asserts the URL contains `q=<term>` and only matching rows are shown; selects a category, asserts the URL contains `categoria=<value>`; reloads the page and asserts the same search term and category are still active; verify `npm run test:e2e` passes.
- [x] 7.4 E2E — Product detail: write a Playwright test that clicks a product row on the list, asserts navigation to `/produtos/:id`, and asserts all product fields (`nome`, `categoria`, `preco`, `estoque`, `ativo`) are visible on the page; verify `npm run test:e2e` passes.
- [x] 7.5 E2E — Create product: write a Playwright test that navigates to `/produtos/novo`, fills in valid data for all fields, submits the form, asserts a success message is shown, and asserts the new product appears in the list at `/`; verify `npm run test:e2e` passes.
- [x] 7.6 E2E — Edit product: write a Playwright test that navigates to `/produtos/:id/editar` for an existing product, changes at least one field, submits, asserts a success message is shown, and asserts the updated value is visible in the list or detail; verify `npm run test:e2e` passes.
- [x] 7.7 E2E — Delete product: write a Playwright test that activates the delete control for a product, asserts the confirmation dialog appears, confirms deletion, and asserts the product is no longer in the list; also write a second test that opens the dialog and cancels, asserting the product remains; verify `npm run test:e2e` passes.
- [x] 7.8 Run the full unit test suite and confirm all tests (including pre-existing shell smoke) still pass: `npm run test` exits 0.
- [x] 7.9 Run `npm run build` in `apps/frontend` and verify it exits 0 with no TypeScript errors and no unused-import warnings treated as errors.
