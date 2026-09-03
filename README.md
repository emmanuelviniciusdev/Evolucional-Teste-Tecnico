# Teste técnico Evolucional

Este repositório reúne as soluções de **dois testes práticos independentes**, cada um com seu próprio enunciado e domínio:

- **Back-end (.NET Pleno)** — uma API de controle de matrículas escolares. Fica em [`apps/backend`](apps/backend); enunciado em [`Enunciado/C#/instrucoes-teste-pratico.txt`](Enunciado/C%23/instrucoes-teste-pratico.txt).
- **Front-end (React Pleno)** — um painel de gerenciamento de produtos que consome uma API REST. Fica em [`apps/frontend`](apps/frontend); enunciado em [`Enunciado/React/instrucoes-teste-pratico.txt`](Enunciado/React/instrucoes-teste-pratico.txt).

Os dois são independentes: o front-end **não** depende do back-end .NET — ele consome a API fake do `json-server` sobre o `db.json` fornecido. Cada projeto tem um README próprio com o passo a passo completo, a stack e a tabela de requisitos do enunciado.

## Back-end — API de matrícula escolar

Solução do teste de .NET Pleno. A stack segue o enunciado à risca: **.NET Framework 4.8** com **ASP.NET Web API 2**, **SQL Server** acessado por **Dapper** com SQL escrito na mão, e **Redis** como cache (bônus). Roda no Windows; SQL Server e Redis sobem no Docker.

Requisitos obrigatórios e os três bônus implementados:

- **CRUD de alunos** — `GET/POST /api/alunos`, `GET/PUT/DELETE /api/alunos/{id}` (listagem paginada com filtro por nome e total de registros; DELETE é exclusão lógica).
- **Turmas** — `GET /api/turmas` com as vagas restantes de cada turma.
- **Matrícula** — `POST /api/matriculas` com as regras de negócio (turma com vaga, aluno ativo, sem matrícula duplicada) e insert + decremento de vagas na mesma transaction.
- **Relatório** — `GET /api/relatorios/alunos-por-turma`, calculado em SQL (`JOIN` + `GROUP BY`).
- **Health check** — `GET /api/health`.
- **Bônus** — cache Redis na listagem de turmas, testes unitários da regra de matrícula e a tela de demonstração em `/ui`.

Como rodar (a partir de `apps/backend`, no Windows):

```bash
make restore
make infra-up
make api-run
```

Com a API no ar em [http://localhost:5000](http://localhost:5000):

- Swagger em [http://localhost:5000/swagger](http://localhost:5000/swagger)
- Tela de demonstração do CRUD em [http://localhost:5000/ui](http://localhost:5000/ui)

O passo a passo completo (connection strings, Docker, testes e a tabela de requisitos) está em [`apps/backend/README.md`](apps/backend/README.md).

## Front-end — Painel de produtos

Solução do teste de React Pleno: um painel de gerenciamento de produtos em **React 19 + TypeScript** com Vite, React Router e a API fake do `json-server`.

Cobre todos os requisitos do enunciado — listagem com busca, filtro por categoria, paginação real e estados de carregando/erro/vazio; detalhe do produto; criação e edição com validação por campo e feedback de sucesso; e exclusão com confirmação — além dos bônus (TypeScript, debounce na busca, testes com Vitest/RTL e Playwright, e estado refletido na URL).

Como rodar (a partir de `apps/frontend`):

```bash
npm install
npm run dev:all   # inicia json-server em :3001 e Vite em :5173
```

O passo a passo completo, a tabela de requisitos, a demonstração em GIF e os scripts de teste estão em [`apps/frontend/README.md`](apps/frontend/README.md).
