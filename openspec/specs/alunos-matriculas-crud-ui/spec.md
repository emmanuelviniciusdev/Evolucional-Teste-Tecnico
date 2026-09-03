# alunos-matriculas-crud-ui Specification

## Purpose

Lets someone demonstrate aluno CRUD and matrícula creation from a same-origin browser page written in pt-BR, without Swagger or curl.

## Requirements

### Requirement: Same-origin demonstration page

The Web API host SHALL serve a static demonstration page at a documented path under the same origin as the API (for example `http://localhost:5000/ui`). Opening that path MUST return HTML that the browser can render without a separate frontend build or package install. The page MUST call the existing endpoints on that same origin (`/api/alunos`, `/api/turmas`, `/api/matriculas`, `/api/relatorios/alunos-por-turma`). The host MUST NOT add new HTTP contracts for this page. Root `/` MUST continue to redirect to Swagger.

#### Scenario: Page is reachable after the API starts

- **WHEN** the API is started with `make api-run` and the documented demonstration page URL is opened in a browser
- **THEN** the host returns HTML for that path and the page can call `/api/alunos` without a cross-origin failure

#### Scenario: Swagger root is unchanged

- **WHEN** `http://localhost:5000/` is opened
- **THEN** the browser is still redirected to the Swagger UI

### Requirement: Demonstration page text is pt-BR

Every visible string on the page MUST be written in pt-BR: document language, headings, labels, buttons, placeholders, empty states, pagination controls, confirmation prompts, client-side validation hints, and success or error feedback. Technical identifiers (HTTP methods, JSON field names, URL paths) MAY appear unchanged. English UI copy MUST NOT appear as the primary labels.

#### Scenario: Page language is Portuguese

- **WHEN** the demonstration page is opened
- **THEN** the document language is `pt-BR` and headings, form labels, and action buttons are in Portuguese (for example Alunos, Matrículas, Salvar, Excluir, Matricular)

#### Scenario: Feedback is in Portuguese

- **WHEN** a create, update, delete, or matrícula action succeeds or fails
- **THEN** the page shows a pt-BR success message or the API `error` text (already pt-BR), not an English-only toast or alert

### Requirement: Aluno list, filter, and pagination

The page SHALL list active alunos from `GET /api/alunos`. It MUST offer an optional name filter that maps to query parameter `nome`, and pagination controls that map to `pagina` and `tamanhoPagina`. The visible rows MUST include at least nome, email, and dataNascimento. The page MUST show the matching `total` from the API. An empty result MUST show a pt-BR empty state rather than a blank table.

#### Scenario: Seeded alunos appear

- **WHEN** the page is opened against the seeded database with no name filter
- **THEN** the first page of active alunos is shown with nome, email, dataNascimento, and the API `total`

#### Scenario: Name filter is applied

- **WHEN** the name filter is set to a fragment such as `ana` and the list is requested
- **THEN** the page calls `GET /api/alunos` with `nome` set and shows only the matching page returned by the API

#### Scenario: Empty list

- **WHEN** the API returns zero alunos for the current filter
- **THEN** the page shows a pt-BR message that no alunos were found

### Requirement: Create, update, and deactivate alunos

The page SHALL let someone create an aluno (`POST /api/alunos`) with `nome`, `email`, and `dataNascimento` (`YYYY-MM-DD`), edit those three fields (`PUT /api/alunos/{id}`), and logically deactivate (`DELETE /api/alunos/{id}`) after a pt-BR confirmation. After a successful write, the list MUST refresh so the new state is visible. Client-side hints MAY reject an empty name or incomplete email before the request; HTTP 400/404 from the API MUST still be shown.

#### Scenario: Create aluno

- **WHEN** a valid nome, complete email, and birth date are submitted
- **THEN** the page posts to `/api/alunos` and, on HTTP 201, the new aluno appears in the list

#### Scenario: Update aluno

- **WHEN** an existing aluno is opened, nome, email, or dataNascimento is changed, and the form is saved
- **THEN** the page puts `/api/alunos/{id}` and, on HTTP 200, the list shows the updated fields

#### Scenario: Logical delete

- **WHEN** deactivation of an active aluno is confirmed
- **THEN** the page deletes `/api/alunos/{id}` and, on HTTP 204, that aluno disappears from the list

### Requirement: Create matrícula and show enrollment overview

The page SHALL let someone enroll an aluno in a turma by posting `{ alunoId, turmaId }` to `/api/matriculas`. Aluno and turma choices MUST come from `GET /api/alunos` and `GET /api/turmas` (turma options MUST show remaining seats). The page MUST also show the per-turma enrollment overview from `GET /api/relatorios/alunos-por-turma` (turma name, enrolled count, remaining seats). The page MUST NOT offer edit or delete of a matrícula row. After a successful enrollment, turma options and the overview MUST refresh so remaining seats match the API.

#### Scenario: Successful enrollment

- **WHEN** an active aluno and a turma with remaining seats are selected and enrollment is confirmed
- **THEN** the page posts to `/api/matriculas` and, on HTTP 201, the overview and turma remaining seats update

#### Scenario: Business rule rejection is visible

- **WHEN** enrollment returns HTTP 409 (inactive aluno, no seats, or duplicate pair)
- **THEN** the page shows the API `error` text and does not pretend the matrícula was created

#### Scenario: Overview lists every turma

- **WHEN** the page loads the enrollment overview against the seeded database
- **THEN** every turma appears with enrolled count and remaining seats, including turmas with zero enrollments
