## Purpose

Enrolls an active aluno into a turma with available seats, persisting the matrícula and decrementing remaining seats atomically.

## ADDED Requirements

### Requirement: Create matrícula

The API SHALL expose `POST /api/matriculas`. The body MUST include `alunoId` and `turmaId`. When the aluno exists and is active, the turma exists and has at least one remaining seat, and the aluno is not already enrolled in that turma, the API MUST insert a `Matricula` row, decrement that turma's `VagasDisponiveis` by one, return HTTP 201, include a `Location` header for `/api/matriculas/{id}`, and return the created matrícula. After success, the turma listing cache MUST be invalidated.

#### Scenario: Successful enrollment

- **WHEN** a client posts an active aluno and a turma that still has remaining seats, and that pair is not already enrolled
- **THEN** the API returns HTTP 201 with the new matrícula, the turma's `VagasDisponiveis` is one less than before, and a later `GET /api/turmas` shows the decremented seats

#### Scenario: Missing ids in body

- **WHEN** a client posts a body without `alunoId` or `turmaId`, or with non-positive ids
- **THEN** the API returns HTTP 400 and no matrícula is stored

### Requirement: Reject enrollment when aluno or turma is missing

When the aluno id or turma id does not exist, `POST /api/matriculas` MUST return HTTP 404 and MUST NOT insert a matrícula or change `VagasDisponiveis`.

#### Scenario: Unknown aluno

- **WHEN** a client posts a matrícula whose `alunoId` does not exist
- **THEN** the API returns HTTP 404 and no seats change

#### Scenario: Unknown turma

- **WHEN** a client posts a matrícula whose `turmaId` does not exist
- **THEN** the API returns HTTP 404 and no matrícula is stored

### Requirement: Reject enrollment when business rules fail

`POST /api/matriculas` MUST return HTTP 409 and MUST NOT persist a matrícula or decrement seats when any of these hold: the aluno is inactive; the turma has zero remaining seats; the aluno is already enrolled in that turma. The system MUST persist at most one matrícula per (`alunoId`, `turmaId`) pair.

#### Scenario: Inactive aluno

- **WHEN** a client posts a matrícula for an existing aluno with `ativo` false
- **THEN** the API returns HTTP 409 and no matrícula is stored

#### Scenario: Turma without seats

- **WHEN** a client posts a matrícula for a turma whose `VagasDisponiveis` is 0
- **THEN** the API returns HTTP 409 and `VagasDisponiveis` stays 0

#### Scenario: Duplicate enrollment

- **WHEN** a client posts a matrícula for an aluno already enrolled in that turma
- **THEN** the API returns HTTP 409 and a second `Matricula` row is not stored

### Requirement: Matrícula write is transactional

Inserting the `Matricula` row and decrementing `VagasDisponiveis` MUST happen in one database transaction. If either write fails, neither change MUST remain committed. Concurrent enrollments MUST NOT drive `VagasDisponiveis` below zero or create two rows for the same aluno and turma.

#### Scenario: Both writes commit together

- **WHEN** enrollment succeeds
- **THEN** a new `Matricula` row exists and the matching turma's `VagasDisponiveis` is decremented in the same commit

#### Scenario: Failure leaves no partial write

- **WHEN** decrementing seats or inserting the matrícula cannot complete
- **THEN** the database has neither a new matrícula nor a changed `VagasDisponiveis` for that attempt
