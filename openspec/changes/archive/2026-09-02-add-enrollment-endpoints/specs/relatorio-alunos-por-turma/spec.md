## Purpose

Reports, per turma, how many alunos are enrolled and how many seats remain, with aggregation performed in SQL Server.

## ADDED Requirements

### Requirement: Alunos-por-turma report

The API SHALL expose `GET /api/relatorios/alunos-por-turma`. A successful call MUST return HTTP 200. The payload MUST include every turma. Each item MUST include the turma name, the count of alunos enrolled in that turma, and the remaining seats. Turmas with zero enrollments MUST still appear with enrolled count 0. Remaining seats MUST match the stored `VagasDisponiveis` for that turma.

#### Scenario: Seeded report

- **WHEN** a client calls `GET /api/relatorios/alunos-por-turma` against the assignment seed
- **THEN** the API returns HTTP 200 with one item per seeded turma, each with the turma name, enrolled count matching `Matricula` rows for that turma, and remaining seats matching `VagasDisponiveis`

#### Scenario: Turma with no enrollments

- **WHEN** a turma has no `Matricula` rows
- **THEN** the report still includes that turma with enrolled count 0 and its current remaining seats

#### Scenario: Report after a new matrícula

- **WHEN** a matrícula is created successfully and the client then calls `GET /api/relatorios/alunos-por-turma`
- **THEN** that turma's enrolled count is one higher and remaining seats are one lower than before the matrícula

### Requirement: Report aggregation runs in SQL

The enrolled count and remaining seats MUST be produced by a SQL query (JOIN and GROUP BY as needed). The application MUST NOT load raw `Matricula` and `Turma` rows and aggregate them in process to build this report.

#### Scenario: Database performs the aggregation

- **WHEN** a reviewer inspects how `GET /api/relatorios/alunos-por-turma` is implemented
- **THEN** a single SQL query returns turma name, enrolled count, and remaining seats, and C# does not group or count enrollments in memory
