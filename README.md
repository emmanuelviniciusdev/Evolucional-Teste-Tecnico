# Teste técnico Evolucional — API de matrícula escolar

Solução do teste prático de .NET Pleno (back-end): uma API de controle de matrículas de uma escola. O enunciado original está em [`Enunciado/C#/instrucoes-teste-pratico.txt`](Enunciado/C%23/instrucoes-teste-pratico.txt).

A stack segue exatamente o que o enunciado exige: **.NET Framework 4.8** com **ASP.NET Web API 2**, **SQL Server** com **Dapper** e SQL escrito na mão. Redis é usado como cache (item bônus) e há uma tela simples que consome a API.

## Onde está cada coisa

O backend executável fica em [`apps/backend`](apps/backend). O [`apps/backend/README.md`](apps/backend/README.md) tem o passo a passo completo — restaurar packages, subir SQL Server e Redis com Docker, rodar a Web API no Windows, connection strings e como testar cada endpoint — além de uma tabela mostrando onde cada requisito do enunciado foi atendido.

Resumindo, com a API no ar em [http://localhost:5000](http://localhost:5000) você tem:

- Swagger em [http://localhost:5000/swagger](http://localhost:5000/swagger)
- Tela de demonstração do CRUD em [http://localhost:5000/ui](http://localhost:5000/ui)

## Requisitos do enunciado

Todos os endpoints obrigatórios e os três itens bônus foram implementados:

- **CRUD de alunos** — `GET/POST /api/alunos`, `GET/PUT/DELETE /api/alunos/{id}` (listagem paginada com filtro por nome e total de registros; DELETE é exclusão lógica).
- **Turmas** — `GET /api/turmas` com as vagas restantes de cada turma.
- **Matrícula** — `POST /api/matriculas` com as regras de negócio (turma com vaga, aluno ativo, sem matrícula duplicada) e insert + decremento de vagas na mesma transaction.
- **Relatório** — `GET /api/relatorios/alunos-por-turma`, calculado em SQL (`JOIN` + `GROUP BY`).
- **Health check** — `GET /api/health`.
- **Bônus** — cache Redis na listagem de turmas, testes unitários da regra de matrícula e a tela em `/ui`.

## Passo a passo

Veja [`apps/backend/README.md`](apps/backend/README.md#como-rodar-localmente). Em resumo, a partir de `apps/backend` no Windows:

```bash
make restore
make infra-up
make api-run
```

Exemplos de chamadas:

```bash
curl http://localhost:5000/api/alunos
curl http://localhost:5000/api/turmas
curl -X POST http://localhost:5000/api/matriculas -H "Content-Type: application/json" -d "{\"alunoId\":1,\"turmaId\":2}"
```

## Frontend

Em [`apps/frontend`](apps/frontend) está a solução do teste prático de front-end: um painel de gerenciamento de produtos em **React 19 + TypeScript** com Vite, React Router e a API fake do `json-server` sobre o `db.json` fornecido. Ele é independente do backend .NET.

O painel cobre todos os requisitos do enunciado — listagem com busca, filtro por categoria, paginação real e estados de carregando/erro/vazio; detalhe do produto; criação e edição com validação por campo e feedback de sucesso; e exclusão com confirmação — além dos bônus (TypeScript, debounce na busca, testes com Vitest/RTL e Playwright, e estado refletido na URL).

Para rodar:

```bash
cd apps/frontend
npm install
npm run dev:all   # inicia json-server em :3001 e Vite em :5173
```

O passo a passo completo, a tabela de requisitos e os scripts de teste estão em [`apps/frontend/README.md`](apps/frontend/README.md).

