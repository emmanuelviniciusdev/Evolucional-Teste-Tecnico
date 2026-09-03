# product-listing Specification

## Purpose

Provides the primary browsing screen for the product catalog: a paginated, searchable, and filterable list of products backed entirely by server-side queries so the client never holds more records than one page.

## Requirements

### Requirement: Server-side paginated product list

The product list screen SHALL display products fetched from the API one page at a time. Pagination MUST be performed by the API (using `_page` and `_limit` query parameters) and MUST NOT be achieved by fetching all records and slicing on the client. The screen MUST display the total number of records returned in `X-Total-Count`. Navigation controls MUST allow the user to move between pages, and MUST be disabled or hidden when there is only one page.

#### Scenario: First page loads with total count

- **WHEN** the user opens the product list at `/`
- **THEN** the screen shows up to the configured page size of products and displays the total count from `X-Total-Count`

#### Scenario: Navigating to the next page fetches new data

- **WHEN** the user advances to page 2
- **THEN** the screen requests `/produtos?_page=2&_limit=<size>` and renders the products from that page

#### Scenario: Single-page result hides or disables page navigation

- **WHEN** the total count fits in one page
- **THEN** the pagination controls are disabled or not rendered

### Requirement: Name search with debounce

The screen SHALL provide a text input that filters products by name. Search MUST be sent to the API as `/produtos?nome_like=<term>` and MUST NOT be performed by filtering a local array. Typing MUST be debounced so that API calls are not sent on every keystroke; the debounce delay MUST be at least 300 ms. When the search term changes the page MUST reset to 1.

#### Scenario: Search queries the API

- **WHEN** the user types a name in the search box and the debounce delay elapses
- **THEN** the screen requests `/produtos?nome_like=<term>&_page=1&_limit=<size>`

#### Scenario: Clearing search returns all products

- **WHEN** the user clears the search input
- **THEN** the screen requests `/produtos` without `nome_like` and shows all products from page 1

#### Scenario: New search resets to page 1

- **WHEN** the user is on page 3 and changes the search term
- **THEN** the screen resets to page 1 before sending the new request

### Requirement: Category filter

The screen SHALL provide a control (select or equivalent) that restricts the product list to a single category. The filter MUST be sent to the API as `/produtos?categoria=<value>` and MUST NOT filter a local array. Changing the category MUST reset the page to 1. The filter MUST be combinable with name search in the same request.

#### Scenario: Selecting a category filters the list

- **WHEN** the user selects a category
- **THEN** the screen requests `/produtos?categoria=<value>&_page=1&_limit=<size>` and shows only matching products

#### Scenario: Filter and search are combined

- **WHEN** the user has both a name search and a category selected
- **THEN** the request includes both `nome_like` and `categoria` query parameters

#### Scenario: Clearing the category restores the unfiltered list

- **WHEN** the user resets the category control to the default (all)
- **THEN** `categoria` is omitted from the next request

### Requirement: URL-reflected list state

The current page, search term, and active category filter SHALL be reflected in the browser URL as query parameters. Loading the URL directly MUST restore the same page, search term, and category shown when the URL was captured.

#### Scenario: List state survives a browser reload

- **WHEN** the user is on page 2 with search "teclado" and category "Perifericos" and reloads the page
- **THEN** the same page, search term, and category are active after load

#### Scenario: Browser back restores previous list state

- **WHEN** the user navigates away from the list and presses browser back
- **THEN** the list restores the page, search, and filter that were active before navigating away

### Requirement: Loading, error, and empty feedback

The screen SHALL display distinct visual states for: data loading (at least a visible indicator), an API error (a user-readable error message that does not expose raw HTTP details), and an empty result set (a message indicating no products were found). These states MUST be mutually exclusive with the product rows.

#### Scenario: Loading indicator during fetch

- **WHEN** a product fetch is in progress
- **THEN** a loading indicator is visible and no product rows are rendered

#### Scenario: Error message on API failure

- **WHEN** the API returns an error or the network request fails
- **THEN** a user-readable error message is shown, and no product rows or loading indicator are rendered

#### Scenario: Empty state when search returns nothing

- **WHEN** the API returns zero products for the current filters
- **THEN** a message indicating no results is shown and the pagination controls are not rendered

### Requirement: Navigation to product detail

Each product entry in the list SHALL be clickable and navigate the user to the product detail screen at `/produtos/:id`.

#### Scenario: Clicking a product navigates to detail

- **WHEN** the user clicks on a product in the list
- **THEN** the browser navigates to `/produtos/<id>` for that product
