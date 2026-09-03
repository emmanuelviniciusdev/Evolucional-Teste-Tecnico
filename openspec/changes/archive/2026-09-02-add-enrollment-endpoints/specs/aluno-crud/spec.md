## Purpose

Lets operators list, inspect, create, update, and logically deactivate alunos through the school enrollment API.

## ADDED Requirements

### Requirement: Paginated aluno list with optional name filter

The API SHALL expose `GET /api/alunos`. The list MUST be paginated. Query parameter `nome` MUST optionally filter by name using a case-insensitive partial match. Query parameters `pagina` and `tamanhoPagina` MUST control the page (1-based) and page size. When omitted, `pagina` MUST default to 1 and `tamanhoPagina` MUST default to 10. `tamanhoPagina` MUST be rejected when less than 1 or greater than 100. The JSON body MUST include the page of alunos (active and inactive), the total number of records matching the filter (not only the page size), and the pagination values used. A successful list MUST return HTTP 200. Invalid pagination MUST return HTTP 400.

#### Scenario: First page without filter

- **WHEN** a client calls `GET /api/alunos` with no query parameters
- **THEN** the API returns HTTP 200 with the first page of alunos, `pagina` 1, `tamanhoPagina` 10, and `total` equal to the number of alunos in the database

#### Scenario: Filter by name

- **WHEN** a client calls `GET /api/alunos?nome=ana` and at least one aluno name contains "ana" regardless of case
- **THEN** the API returns HTTP 200 whose items all match that filter and whose `total` is the count of matching alunos, not the unfiltered table size

#### Scenario: Invalid page size

- **WHEN** a client calls `GET /api/alunos?tamanhoPagina=0` or `tamanhoPagina=101`
- **THEN** the API returns HTTP 400

### Requirement: Get aluno by id

The API SHALL expose `GET /api/alunos/{id}`. When the aluno exists (active or inactive), the response MUST be HTTP 200 with that aluno. When no row exists for the id, the response MUST be HTTP 404.

#### Scenario: Existing aluno

- **WHEN** a client calls `GET /api/alunos/{id}` for an id that exists
- **THEN** the API returns HTTP 200 with that aluno's fields, including `ativo`

#### Scenario: Missing aluno

- **WHEN** a client calls `GET /api/alunos/{id}` for an id that does not exist
- **THEN** the API returns HTTP 404

### Requirement: Create aluno

The API SHALL expose `POST /api/alunos`. The body MUST include `nome`, `email`, and `dataNascimento`. A valid create MUST persist the aluno as active, return HTTP 201, include a `Location` header pointing at `GET /api/alunos/{id}`, and return the created aluno (including generated `id`). `email` MUST be a complete address: a non-empty local-part, `@`, and a domain with at least two labels separated by a dot. The local-part MUST accept letters and digits with or without a dot (both `anasouza2345@email.com` and `ana.souza@email.com` MUST succeed). Presence of `@` alone MUST NOT be enough (`a@b`, `user@localhost`, `user@`, `@dominio.com` MUST fail). `dataNascimento` MUST be the calendar date string `YYYY-MM-DD` in both request and response (for example `2006-03-14`); values with a time component, another date layout, or an unparseable string MUST be rejected. Missing or invalid fields (empty name, incomplete or malformed email, missing/`YYYY-MM-DD`-invalid or future `dataNascimento`, name or email longer than the schema allows) MUST return HTTP 400 and MUST NOT persist a row.

#### Scenario: Valid create

- **WHEN** a client posts a valid `nome`, a complete email such as `anasouza2345@email.com` or `ana.souza@email.com`, and `dataNascimento` as `2006-03-14`
- **THEN** the API returns HTTP 201 with the created aluno, `ativo` true, a generated `id`, a `Location` header for `/api/alunos/{id}`, and `dataNascimento` serialized as `YYYY-MM-DD` with no time part

#### Scenario: Invalid create body

- **WHEN** a client posts an aluno without `nome` or with an incomplete email such as `nao-e-email`, `user@localhost`, or `a@b`
- **THEN** the API returns HTTP 400 and no new aluno is stored

#### Scenario: dataNascimento is not YYYY-MM-DD

- **WHEN** a client posts `dataNascimento` as `2006-03-14T00:00:00`, `14/03/2006`, or another string that is not `YYYY-MM-DD`
- **THEN** the API returns HTTP 400 and no new aluno is stored

### Requirement: Update aluno

The API SHALL expose `PUT /api/alunos/{id}`. The body MUST include `nome`, `email`, and `dataNascimento` with the same complete-email and `YYYY-MM-DD` rules as create. A successful update MUST persist those three fields, MUST NOT change `ativo`, and MUST return HTTP 200 with the updated aluno and `dataNascimento` as `YYYY-MM-DD`. When the id does not exist, the response MUST be HTTP 404. Invalid fields MUST return HTTP 400 and MUST NOT change the stored row.

#### Scenario: Valid update

- **WHEN** a client puts valid `nome`, `email`, and `dataNascimento` for an existing aluno
- **THEN** the API returns HTTP 200 with the updated fields and the same `ativo` value as before the request

#### Scenario: Update missing aluno

- **WHEN** a client puts an aluno body for an id that does not exist
- **THEN** the API returns HTTP 404

### Requirement: Logical delete aluno

The API SHALL expose `DELETE /api/alunos/{id}`. The operation MUST set `ativo` to false and MUST NOT delete the database row. A successful deactivation MUST return HTTP 204. When the id does not exist, the response MUST be HTTP 404. When the aluno is already inactive, the response MUST be HTTP 204 and the row MUST remain inactive (idempotent). After a logical delete, `GET /api/alunos/{id}` MUST still return the aluno with `ativo` false.

#### Scenario: Deactivate active aluno

- **WHEN** a client deletes an active aluno
- **THEN** the API returns HTTP 204, the row still exists, and a later get returns `ativo` false

#### Scenario: Delete missing aluno

- **WHEN** a client deletes an id that does not exist
- **THEN** the API returns HTTP 404

#### Scenario: Delete already inactive aluno

- **WHEN** a client deletes an aluno whose `ativo` is already false
- **THEN** the API returns HTTP 204 and the row remains inactive
