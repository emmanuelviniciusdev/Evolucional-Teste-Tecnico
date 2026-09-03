# Evolucional technical test

School enrollment API on .NET Framework 4.8.

The runnable backend lives in [`apps/backend`](apps/backend). Follow [`apps/backend/README.md`](apps/backend/README.md) to restore packages, start SQL Server and Redis with Docker, run the Web API on Windows, open Swagger at [http://localhost:5000/swagger](http://localhost:5000/swagger), and call the assignment routes:

- `GET/POST /api/alunos`, `GET/PUT/DELETE /api/alunos/{id}`
- `GET /api/turmas`
- `POST /api/matriculas`
- `GET /api/relatorios/alunos-por-turma`
- `GET /api/health`

Example:

```bash
curl http://localhost:5000/api/alunos
curl http://localhost:5000/api/turmas
curl -X POST http://localhost:5000/api/matriculas -H "Content-Type: application/json" -d "{\"alunoId\":1,\"turmaId\":2}"
```
