# Teste técnico Evolucional

API de matrícula escolar em .NET Framework 4.8.

O backend executável fica em [`apps/backend`](apps/backend). Siga [`apps/backend/README.md`](apps/backend/README.md) para restaurar os packages, subir SQL Server e Redis com Docker, rodar a Web API no Windows, abrir o Swagger em [http://localhost:5000/swagger](http://localhost:5000/swagger), abrir a tela de demonstração em [http://localhost:5000/ui](http://localhost:5000/ui) e chamar as rotas do enunciado:

- `GET/POST /api/alunos`, `GET/PUT/DELETE /api/alunos/{id}`
- `GET /api/turmas`
- `POST /api/matriculas`
- `GET /api/relatorios/alunos-por-turma`
- `GET /api/health`

Exemplo:

```bash
curl http://localhost:5000/api/alunos
curl http://localhost:5000/api/turmas
curl -X POST http://localhost:5000/api/matriculas -H "Content-Type: application/json" -d "{\"alunoId\":1,\"turmaId\":2}"
```
