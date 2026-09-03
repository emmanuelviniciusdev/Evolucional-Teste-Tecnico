## MODIFIED Requirements

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
