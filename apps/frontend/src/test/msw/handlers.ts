import { rest } from 'msw'
import { Produto } from '@/shared/api/products'

const produtos: Produto[] = [
  { id: 1, nome: 'Teclado', categoria: 'Perifericos', preco: 100, estoque: 10, ativo: true },
  { id: 2, nome: 'Mouse', categoria: 'Perifericos', preco: 50, estoque: 5, ativo: true }
]

export const handlers = [
  rest.get('http://localhost:3001/produtos', (req, res, ctx) => {
    const page = Number(req.url.searchParams.get('_page') || '1')
    const limit = Number(req.url.searchParams.get('_limit') || '10')
    const start = (page - 1) * limit
    const items = produtos.slice(start, start + limit)
    return res(ctx.set('X-Total-Count', String(produtos.length)), ctx.json(items))
  })
]

