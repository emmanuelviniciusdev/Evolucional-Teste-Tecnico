import { http, HttpResponse } from 'msw'
import { Produto } from '@/shared/api/products'

let nextId = 100

let produtos: Produto[] = [
  { id: 1, nome: 'Teclado', categoria: 'Perifericos', preco: 100, estoque: 10, ativo: true },
  { id: 2, nome: 'Mouse', categoria: 'Perifericos', preco: 50, estoque: 5, ativo: true },
  { id: 3, nome: 'Monitor', categoria: 'Monitores', preco: 1500, estoque: 3, ativo: true },
  { id: 4, nome: 'Headset', categoria: 'Audio', preco: 260, estoque: 8, ativo: true },
  { id: 5, nome: 'SSD', categoria: 'Armazenamento', preco: 450, estoque: 20, ativo: true },
  { id: 6, nome: 'RAM DDR4', categoria: 'Componentes', preco: 290, estoque: 15, ativo: true },
  { id: 7, nome: 'Hub USB', categoria: 'Acessorios', preco: 180, estoque: 7, ativo: true },
  { id: 8, nome: 'Webcam', categoria: 'Perifericos', preco: 229, estoque: 6, ativo: true },
  { id: 9, nome: 'Mousepad', categoria: 'Perifericos', preco: 60, estoque: 40, ativo: true },
  { id: 10, nome: 'Cabo HDMI', categoria: 'Acessorios', preco: 35, estoque: 50, ativo: true },
  { id: 11, nome: 'Monitor 4K', categoria: 'Monitores', preco: 2500, estoque: 2, ativo: true },
]

const SEED = [...produtos]

/** Reset handler data between tests that mutate state */
export const resetHandlerData = () => {
  nextId = 100
  produtos = SEED.map((p) => ({ ...p }))
}

export const handlers = [
  // GET /produtos — paginated, searchable, filterable
  http.get('http://localhost:3001/produtos', ({ request }) => {
    const url = new URL(request.url)
    const page = Number(url.searchParams.get('_page') || '1')
    const limit = Number(url.searchParams.get('_limit') || '10')
    const nomeLike = url.searchParams.get('nome_like') || ''
    const categoriaFilter = url.searchParams.get('categoria') || ''

    let filtered = produtos
    if (nomeLike) {
      filtered = filtered.filter((p) =>
        p.nome.toLowerCase().includes(nomeLike.toLowerCase()),
      )
    }
    if (categoriaFilter) {
      filtered = filtered.filter((p) => p.categoria === categoriaFilter)
    }

    const start = (page - 1) * limit
    const items = filtered.slice(start, start + limit)

    return HttpResponse.json(items, {
      headers: { 'X-Total-Count': String(filtered.length) },
    })
  }),

  // GET /produtos/:id
  http.get('http://localhost:3001/produtos/:id', ({ params }) => {
    const id = Number(params['id'])
    const produto = produtos.find((p) => p.id === id)
    if (!produto) {
      return HttpResponse.json({ message: 'Não encontrado' }, { status: 404 })
    }
    return HttpResponse.json(produto)
  }),

  // POST /produtos
  http.post('http://localhost:3001/produtos', async ({ request }) => {
    const body = await request.json() as Omit<Produto, 'id'>
    const novo: Produto = { ...body, id: ++nextId }
    produtos.push(novo)
    return HttpResponse.json(novo, { status: 201 })
  }),

  // PUT /produtos/:id
  http.put('http://localhost:3001/produtos/:id', async ({ request, params }) => {
    const id = Number(params['id'])
    const body = await request.json() as Partial<Produto>
    const idx = produtos.findIndex((p) => p.id === id)
    if (idx === -1) return HttpResponse.json({ message: 'Não encontrado' }, { status: 404 })
    produtos[idx] = { ...produtos[idx], ...body }
    return HttpResponse.json(produtos[idx])
  }),

  // DELETE /produtos/:id
  http.delete('http://localhost:3001/produtos/:id', ({ params }) => {
    const id = Number(params['id'])
    const idx = produtos.findIndex((p) => p.id === id)
    if (idx === -1) return HttpResponse.json({ message: 'Não encontrado' }, { status: 404 })
    produtos.splice(idx, 1)
    return HttpResponse.json({})
  }),
]
