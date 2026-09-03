## Purpose

Provides a validated form for creating and editing products; the same component covers both modes, distinguishing them by whether a product id is present in the route.

## ADDED Requirements

### Requirement: Create product form

The create screen at `/produtos/novo` SHALL render a form that accepts `nome`, `categoria`, `preco`, and `estoque` inputs and submits a POST to the API. On successful save, the screen MUST display a pt-BR success feedback message and navigate the user to the product list or the new product's detail screen. The screen MUST render within the application shell.

#### Scenario: New product is saved and confirmed

- **WHEN** the user fills in valid data and submits the create form
- **THEN** a POST is sent to `/produtos`, a success message is shown, and the user is taken to the list or the new product detail

#### Scenario: Loading state during save

- **WHEN** the form submission is in progress
- **THEN** the submit control is disabled or a loading indicator is shown

#### Scenario: API error on create

- **WHEN** the API returns an error during POST
- **THEN** a user-readable error message is shown and the user remains on the form

### Requirement: Edit product form

The edit screen at `/produtos/:id/editar` SHALL fetch the existing product, populate the form inputs with current values, and submit a PUT to the API on save. On successful save, the screen MUST display a pt-BR success feedback message and navigate the user to the list or the product's detail screen.

#### Scenario: Edit form is pre-populated with existing values

- **WHEN** the user opens `/produtos/1/editar`
- **THEN** the form inputs are pre-filled with the current values of product 1

#### Scenario: Edited product is saved and confirmed

- **WHEN** the user changes a field and submits the edit form
- **THEN** a PUT is sent to `/produtos/1`, a success message is shown, and the user is taken to the list or the detail screen

#### Scenario: Loading state while fetching product for edit

- **WHEN** the product data is being fetched for the edit form
- **THEN** a loading indicator is visible and the form is not yet rendered

### Requirement: Form field validation

The form SHALL validate inputs before submitting to the API. Validation MUST enforce: `nome` is required and has at least 3 characters; `preco` is required and greater than zero; `estoque` is required and zero or greater. Error messages MUST appear adjacent to the relevant field, not as a generic alert. The form MUST NOT submit while validation errors exist.

#### Scenario: Short name shows a field-level error

- **WHEN** the user submits the form with `nome` shorter than 3 characters
- **THEN** an error message appears next to the name field and the form is not submitted

#### Scenario: Invalid price shows a field-level error

- **WHEN** the user submits the form with `preco` equal to zero or negative
- **THEN** an error message appears next to the price field and the form is not submitted

#### Scenario: Negative stock shows a field-level error

- **WHEN** the user submits the form with `estoque` less than zero
- **THEN** an error message appears next to the stock field and the form is not submitted

#### Scenario: Valid form submits without errors

- **WHEN** all fields satisfy the validation rules and the user submits
- **THEN** no field-level error messages are shown and the API request is sent
