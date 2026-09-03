# Painel de gerenciamento de produtos

SPA em React que consome uma API REST para gerenciar um catálogo de produtos, feita para o teste prático de front-end. A stack segue o enunciado: **React 19** com **TypeScript**, build e dev server no **Vite**, roteamento com **React Router** e a API fake do **json-server** sobre o `db.json` fornecido. Todas as mensagens da UI estão em pt-BR.

## O que o enunciado pede e onde isso está

Um resumo de como cada requisito foi atendido, para facilitar a revisão:

| Requisito do enunciado | Como foi atendido |
| --- | --- |
| **Listagem de produtos** (tabela, busca, filtro, paginação real, total) | `pages/Home.tsx` monta a tabela; a busca por nome, o filtro por categoria e a paginação vão para a API (`_page`, `_limit`, `nome_like`, `categoria`), e o total vem do header `X-Total-Count`, não de fatiar tudo no front. |
| **Estados de carregando, erro e vazio** | `Home` alterna entre `LoadingSpinner`, `ErrorMessage` e um estado vazio ("Nenhum produto encontrado") conforme o resultado da chamada. |
| **Detalhe do produto** | `pages/ProductDetail.tsx` carrega o produto por id e mostra os dados completos. |
| **Criar e editar** com validação por campo | `pages/ProductForm.tsx` usa React Hook Form: nome obrigatório com mínimo de 3 caracteres, preço maior que zero e estoque zero ou mais. As mensagens aparecem ao lado de cada campo (`role="alert"`), não em um alert genérico. |
| **Feedback de sucesso ao salvar** | Após criar ou editar, o formulário mostra uma mensagem de sucesso e redireciona para o catálogo. |
| **Excluir com confirmação** | `components/ConfirmDialog.tsx` pede confirmação antes do DELETE; depois de excluir, a listagem é atualizada com uma mensagem de sucesso. |
| **Chamadas de API centralizadas** | Todo o acesso HTTP fica em `shared/api/products.ts`; os componentes só falam com hooks (`useProducts`, `useProduct`, `useSaveProduct`, `useDeleteProduct`), sem `fetch` solto. |
| **Componentização** | Páginas em `pages/`, componentes reutilizáveis em `shared/components/` e hooks em `shared/hooks/`; nenhum arquivo monolítico. |

Itens bônus do enunciado, todos implementados:

- **TypeScript bem usado** — o tipo `Produto` e o `ListResult<T>` da API são tipados e reaproveitados pelos hooks e páginas.
- **Debounce na busca** — `useDebounce` aguarda 300 ms antes de disparar a chamada enquanto o usuário digita.
- **Testes** — Vitest com React Testing Library e MSW (unit e integration), mais Playwright para e2e com checagem de acessibilidade (`axe`).
- **React Router refletindo o estado na URL** — página, busca (`q`) e categoria ficam nos query params, então recarregar ou compartilhar o link preserva a tela.

## Stack

- React 19 + TypeScript
- Vite (dev server e build)
- React Router 7 para o roteamento
- React Hook Form para o formulário e a validação
- json-server 0.17.4 como API fake (compatível com o enunciado)
- Vitest + React Testing Library + MSW para testes; Playwright + axe para e2e

## Pré-requisitos

- Node.js >= 20.19

## Como rodar localmente

A partir de `apps/frontend`, instale as dependências:

```bash
npm install
```

O jeito mais rápido de subir tudo é rodar a API fake e o app juntos:

```bash
npm run dev:all
```

Isso levanta o json-server em [http://localhost:3001](http://localhost:3001) (recurso `/produtos`) e o Vite em [http://localhost:5173](http://localhost:5173). O dev server já faz proxy de `/produtos` para o json-server, então o app funciona sem configuração extra.

Se preferir rodar cada parte separada, em dois terminais:

```bash
npm run api    # json-server em http://localhost:3001
npm run dev    # app em http://localhost:5173
```

## Testes

```bash
npm run test              # unit e integration (Vitest)
npm run test:unit         # só os testes unitários
npm run test:integration  # testes de integração com MSW
```

Para o e2e com Playwright, instale o browser uma vez e depois rode a suíte (ela sobe o dev server sozinha):

```bash
npx playwright install chromium
npm run test:e2e
```

## Estrutura

```
src/
  app/            AppShell com o layout, o skip link e as rotas
  pages/          Home (listagem), ProductDetail, ProductForm, NotFound
  shared/
    api/          products.ts — ponto único de acesso HTTP
    hooks/        useProducts, useProduct, useSaveProduct, useDeleteProduct, useDebounce
    components/   LoadingSpinner, ErrorMessage, ConfirmDialog
  test/           setup, handlers do MSW e testes de integração
e2e/              specs do Playwright
```

## Observações

- Todas as mensagens da UI são em pt-BR.
- O `db.json` incluído é o dataset do enunciado; o json-server aceita GET, POST, PUT e DELETE em `/produtos`.
