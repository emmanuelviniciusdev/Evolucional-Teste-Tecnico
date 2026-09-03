# product-detail Specification

## Purpose

Provides a read-only screen that fetches and displays the complete data of a single product, accessible by navigating from the product list or directly by URL.

## Requirements

### Requirement: Product detail screen

The product detail screen at `/produtos/:id` SHALL fetch the product with the given id from the API and display all its fields: `id`, `nome`, `categoria`, `preco`, `estoque`, and `ativo`. The screen MUST show a loading state while fetching, a user-readable error state if the fetch fails or the product is not found, and the product data once loaded. The screen MUST render within the application shell.

#### Scenario: All product fields are shown

- **WHEN** the user opens `/produtos/1`
- **THEN** the screen shows the name, category, price, stock, and active status of the product with id 1

#### Scenario: Loading state while fetching

- **WHEN** the product fetch is in progress
- **THEN** a loading indicator is visible and no product data is rendered

#### Scenario: Error state when product is not found

- **WHEN** the API returns 404 or a network error for the requested id
- **THEN** a user-readable error message is shown and no product fields are rendered

### Requirement: Navigation back to the product list

The product detail screen SHALL provide a visible control that navigates the user back to the product list at `/`.

#### Scenario: Back navigation returns to the list

- **WHEN** the user activates the back control on the detail screen
- **THEN** the browser navigates to `/`

### Requirement: Navigation to edit from detail

The product detail screen SHALL provide a visible control that navigates the user to the edit form at `/produtos/:id/editar` for the currently displayed product.

#### Scenario: Edit control opens the edit form

- **WHEN** the user activates the edit control on the detail screen
- **THEN** the browser navigates to `/produtos/<id>/editar`
