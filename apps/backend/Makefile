# Local workflow for the Escola Web API (.NET Framework 4.8).
# Docker infrastructure works on any Docker host.
# restore, api-run, and tests require Windows with .NET Framework 4.8.

COMPOSE_FILE := infra/docker-compose.yml
COMPOSE := docker compose -f $(COMPOSE_FILE) --project-name escola
NUGET ?= nuget
MSBUILD ?= msbuild
DOTNET ?= dotnet
IIS_EXPRESS ?= C:\Program Files\IIS Express\iisexpress.exe
API_PORT ?= 5000
API_PHYSICAL_PATH := $(CURDIR)/src/Escola.Api
UNIT_PROJECT := testes/Escola.Testes.Unitarios/Escola.Testes.Unitarios.csproj
INTEGRATION_PROJECT := testes/Escola.Testes.Integracao/Escola.Testes.Integracao.csproj

.PHONY: restore infra-up infra-down infra-logs infra-reset api-run test-unit test-integration test

restore:
	$(NUGET) restore Escola.sln

infra-up:
	$(COMPOSE) up -d sqlserver redis --wait
	$(COMPOSE) run --rm sqlserver-init
	$(COMPOSE) exec -T redis redis-cli -n 0 FLUSHDB

infra-down:
	$(COMPOSE) down

infra-logs:
	$(COMPOSE) logs -f

infra-reset:
	$(COMPOSE) down -v
	$(MAKE) infra-up

api-run: infra-up
	"$(IIS_EXPRESS)" /path:"$(API_PHYSICAL_PATH)" /port:$(API_PORT)

test-unit: restore
	$(MSBUILD) $(UNIT_PROJECT) /p:Configuration=Debug /restore
	$(DOTNET) test $(UNIT_PROJECT) --no-build -c Debug

test-integration: restore infra-up
	$(MSBUILD) $(INTEGRATION_PROJECT) /p:Configuration=Debug /restore
	$(DOTNET) test $(INTEGRATION_PROJECT) --no-build -c Debug

test: test-unit test-integration
