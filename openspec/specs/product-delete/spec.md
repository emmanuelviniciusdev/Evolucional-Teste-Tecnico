# product-delete Specification

## Purpose

Provides a safe delete flow for products: the user must confirm intent before the DELETE request is sent, preventing accidental data loss.

## Requirements

### Requirement: Delete product with confirmation

The product list and/or detail screen SHALL expose a delete control for each product. Activating the control MUST display a confirmation dialog that asks the user to confirm or cancel. Only on confirmation SHALL a DELETE request be sent to `/produtos/:id`. On successful deletion the screen MUST reflect the removal (refresh the list or navigate away from the detail screen). On API error a user-readable message MUST be shown.

#### Scenario: Confirmation dialog appears before deletion

- **WHEN** the user activates the delete control for a product
- **THEN** a confirmation dialog is displayed and no DELETE request is sent yet

#### Scenario: Confirmed deletion removes the product

- **WHEN** the user confirms deletion in the dialog
- **THEN** `DELETE /produtos/<id>` is sent and the product is no longer shown in the list or the user is redirected away from the detail screen

#### Scenario: Cancelled deletion leaves the product intact

- **WHEN** the user cancels the confirmation dialog
- **THEN** no DELETE request is sent and the product remains in the list

#### Scenario: API error on delete

- **WHEN** the API returns an error during DELETE
- **THEN** a user-readable error message is shown and the product is still visible in the list
