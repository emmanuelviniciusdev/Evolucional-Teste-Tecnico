# turma-listing Specification

## Purpose

Lets operators list every turma with remaining seats, using Redis so repeated reads stay cheap until a new matrícula lands.

## Requirements

### Requirement: List turmas with remaining seats

The API SHALL expose `GET /api/turmas`. A successful call MUST return HTTP 200 and a JSON array (or equivalent list payload) of every turma. Each item MUST include the turma identity, name, period, total seats, and remaining seats (`vagasDisponiveis`). Remaining seats MUST come from the stored `VagasDisponiveis` value, not from a count computed only in the API process.

#### Scenario: Seeded turmas are returned

- **WHEN** a client calls `GET /api/turmas` against the assignment seed
- **THEN** the API returns HTTP 200 including every seeded turma and each item's remaining seats match the database `VagasDisponiveis`

#### Scenario: Full turma still appears

- **WHEN** a turma has zero remaining seats
- **THEN** `GET /api/turmas` still includes that turma with remaining seats equal to 0

### Requirement: Redis cache for turma listing

`GET /api/turmas` MUST use Redis to cache the listing payload. On a cache miss, the API MUST load turmas from SQL Server and store the payload in Redis. Subsequent `GET /api/turmas` calls MUST be served from that cache until it is invalidated or expires. Cache keys and TTL are implementation details; a successful matrícula MUST invalidate the listing so the next `GET /api/turmas` reflects the new remaining seats.

#### Scenario: Cache hit after first list

- **WHEN** a client calls `GET /api/turmas` twice with no successful matrícula between the calls
- **THEN** both responses are HTTP 200 with the same remaining seats, and the second call does not depend on a write to SQL Server

#### Scenario: Cache invalidation after matrícula

- **WHEN** a client successfully creates a matrícula that decrements a turma's remaining seats and then calls `GET /api/turmas`
- **THEN** that turma's remaining seats in the list match the decremented database value, not the value cached before the matrícula
