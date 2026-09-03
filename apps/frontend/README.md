# Nexo — Frontend (fundação)

Este diretório contém a base do aplicativo React para o teste prático. É uma fundação apenas: as telas de listagem, detalhe e CRUD serão implementadas em mudanças posteriores.

Pré-requisitos
- Node.js >= 20.19

Instalação

1. Na raiz do `apps/frontend` rode:

```bash
npm install
```

Como rodar a API fake (json-server)

```bash
npm run api
# abre http://localhost:3001/produtos
```

Como rodar o app em dev (Vite)

```bash
npm run dev
# abre http://localhost:5173
```

Como rodar ambos (API fake + app)

```bash
npm run dev:all
```

Testes

- Unit + Integration (Vitest): `npm run test`
- Playwright e2e: `npx playwright install chromium` e depois `npm run test:e2e`

Observações

- Todas as mensagens da UI são em pt-BR. Este pacote usa json-server `0.17.4` por compatibilidade com o enunciado.

