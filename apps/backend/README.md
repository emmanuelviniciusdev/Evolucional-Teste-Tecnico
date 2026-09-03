# API de matrícula Escola

ASP.NET Web API 2 para um sistema de matrícula escolar: CRUD de alunos, listagem de turmas com cache Redis, matrícula transacional e um relatório SQL. O host roda no Windows (.NET Framework 4.8); SQL Server e Redis sobem no Docker.

## Stack

- .NET Framework 4.8
- ASP.NET Web API 2 hospedada no IIS / IIS Express (OWIN `Startup`)
- SQL Server com Dapper e SQL parameterized
- Redis via StackExchange.Redis (`ICacheService`)
- Autofac para composition
- Swashbuckle 5.6.0 para o Swagger UI

## Requisito Windows

Build da solution, hosting da API e execução dos testes exigem **Windows** com:

- .NET Framework 4.8 Developer Pack
- Visual Studio 2022 (ou MSBuild + NuGet + IIS Express)
- Docker Desktop (para SQL Server e Redis)

A API **não** roda em macOS, Linux, nem em um container Linux. O Docker é usado só para SQL Server e Redis. As portas do host **1433** (SQL Server) e **6379** (Redis) precisam estar livres antes de `make infra-up`. Em Apple Silicon, a imagem do SQL Server 2022 sobe via emulação amd64.

## Como rodar localmente

A partir de `apps/backend`:

```bash
make restore
make infra-up
make api-run
```

`make infra-up` sempre reaplica o schema e as linhas de sample em `TesteEscola` e faz flush do Redis db 0. **Não** é necessário `make infra-reset` depois de uma mudança de schema ou seed.

`make api-run` compila a API, sobe a infrastructure e inicia o IIS Express em [http://localhost:5000](http://localhost:5000). Sobrescreva o path do IIS Express se precisar:

```bash
make api-run IIS_EXPRESS="C:\Program Files\IIS Express\iisexpress.exe"
```

Alternativamente, abra `Escola.sln` no Visual Studio e pressione F5 (a URL do project é `http://localhost:5000/`).

### Infrastructure Docker

```bash
make infra-up      # SQL Server :1433 e Redis :6379, reseed de TesteEscola, flush do Redis db 0
make infra-logs    # acompanha os logs dos containers
make infra-down    # para os containers
make infra-reset   # apaga os volumes Docker e depois infra-up (opcional; não é necessário após mudanças de schema)
```

Não existe imagem Linux para o processo da Web API.

### Connection strings (somente local)

Essas credentials de development **não são para production**.

| Setting | API (`Web.config`) | Testes de integration (`App.config`) |
| --- | --- | --- |
| SQL Server | `Server=localhost,1433;Database=TesteEscola;User Id=sa;Password=Escola_Dev_P@ssw0rd;Encrypt=True;TrustServerCertificate=True` | Mesmo server, database `TesteEscola_Testes` |
| Redis | `localhost:6379,abortConnect=false` (db lógico 0) | `localhost:6379,abortConnect=false,defaultDatabase=1` |

`TesteEscola_Testes` é criado e recebe seed só quando os testes de integration rodam. O Compose / `make infra-up` não cria esse database.

## Health check

Com a infrastructure e a API no ar:

```bash
curl http://localhost:5000/api/health
```

- HTTP 200 quando SQL Server e Redis estão reachable (`status`: `healthy`)
- HTTP 503 quando qualquer dependency está down (`unavailable` em `sqlServer` e/ou `redis`)

## Swagger

Abra [http://localhost:5000/swagger](http://localhost:5000/swagger) (UI) ou `http://localhost:5000/swagger/docs/v1` (documento OpenAPI). As rotas do enunciado têm description em inglês (en-US) e schemas de request/response (propriedades, tipos e campos).

## Endpoints do enunciado

O JSON é camelCase. Bodies de erro usam `{ "error": "<mensagem em pt-BR>" }` com HTTP 400 (validation), 404 (registro ausente) ou 409 (regra de negócio).

### Alunos

```bash
curl http://localhost:5000/api/alunos
curl "http://localhost:5000/api/alunos?nome=ana&pagina=1&tamanhoPagina=10"
curl http://localhost:5000/api/alunos/1
curl -X POST http://localhost:5000/api/alunos -H "Content-Type: application/json" -d "{\"nome\":\"Ana Souza\",\"email\":\"anasouza2345@email.com\",\"dataNascimento\":\"2006-03-14\"}"
curl -X PUT http://localhost:5000/api/alunos/1 -H "Content-Type: application/json" -d "{\"nome\":\"Ana Souza\",\"email\":\"ana.souza@email.com\",\"dataNascimento\":\"2006-03-14\"}"
curl -X DELETE http://localhost:5000/api/alunos/1
```

`dataNascimento` é somente `YYYY-MM-DD`. O email precisa ser um address completo (local-part com ou sem ponto). DELETE é uma desativação lógica (`Ativo = 0`); GET da listagem e GET por id omitem alunos inativos (HTTP 404 por id). A row permanece no database para a matrícula ainda poder recusar um aluno inativo com HTTP 409.

### Turmas

```bash
curl http://localhost:5000/api/turmas
```

As vagas restantes vêm de `VagasDisponiveis`. A listagem fica em cache no Redis (`turmas:listagem`, TTL de 5 minutes) e é invalidada após uma matrícula com sucesso.

### Matrículas

```bash
curl -X POST http://localhost:5000/api/matriculas -H "Content-Type: application/json" -d "{\"alunoId\":1,\"turmaId\":2}"
```

O insert e o decrement de vagas rodam em uma única transaction. Aluno inativo, sem vagas ou par duplicado retornam HTTP 409.

### Relatório

```bash
curl http://localhost:5000/api/relatorios/alunos-por-turma
```

As contagens são calculadas em SQL (`LEFT JOIN` + `GROUP BY`), incluindo turmas com zero enrollments.

## Testes

```bash
make test-unit          # Docker não é necessário
make test-integration   # sobe a infra Docker e depois cria TesteEscola_Testes / faz flush do Redis db 1
make test               # unit e em seguida integration
```

## Script de schema

`infra/sql/init.sql` começou como cópia de `Enunciado/C#/script-banco.sql`. Ele adiciona `CONSTRAINT UQ_Matricula_Aluno_Turma UNIQUE (AlunoId, TurmaId)` para que enrollments duplicados concorrentes não persistam. Cada `make infra-up` re-drop/recria as tables da API em `TesteEscola`, faz reseed das rows de sample e flush do Redis db 0. `make infra-reset` não é necessário para aplicar esse unique index.

## Targets do Make

| Target | Ação |
| --- | --- |
| `restore` | `nuget restore Escola.sln` |
| `infra-up` | Sobe SQL Server e Redis, reseed de `TesteEscola`, flush do Redis db 0 |
| `infra-down` | Para os containers |
| `infra-logs` | Acompanha os logs do Compose |
| `infra-reset` | `down -v` e depois `infra-up` (wipe de volume; opcional) |
| `api-run` | `infra-up`, build da API e IIS Express na porta 5000 |
| `test-unit` | Restore, MSBuild e execução do project de testes unitários |
| `test-integration` | `infra-up` e depois execução do project de testes de integration |
| `test` | Testes unitários e em seguida testes de integration |
