/**
 * In Vite (browser): '' — relative URLs are forwarded by the dev proxy.
 * In vitest integration tests: 'http://localhost:3001' — set via VITE_API_BASE define.
 */
// eslint-disable-next-line @typescript-eslint/no-explicit-any
const API_BASE: string = (import.meta.env as any).VITE_API_BASE ?? ''

export type Produto = {
  id: number
  nome: string
  categoria: string
  preco: number
  estoque: number
  ativo: boolean
}

export type ListResult<T> = {
  items: T[]
  total: number
}

const parseTotal = (headers: Headers) => {
  const v = headers.get('X-Total-Count') || headers.get('x-total-count')
  return v ? Number(v) : 0
}

export async function listProducts(params?: { page?: number; limit?: number; nome?: string; categoria?: string; }): Promise<ListResult<Produto>> {
  const qp = new URLSearchParams()
  if (params?.page) qp.set('_page', String(params.page))
  if (params?.limit) qp.set('_limit', String(params.limit))
  if (params?.nome) qp.set('nome_like', params.nome)
  if (params?.categoria) qp.set('categoria', params.categoria)
  const res = await fetch(`${API_BASE}/produtos?${qp.toString()}`)
  if (!res.ok) throw new Error('Erro ao listar produtos')
  const data: Produto[] = await res.json()
  const total = parseTotal(res.headers)
  return { items: data, total }
}

export async function getProduct(id: number): Promise<Produto> {
  const res = await fetch(`${API_BASE}/produtos/${id}`)
  if (!res.ok) throw new Error('Erro ao obter produto')
  return res.json()
}

export async function createProduct(p: Omit<Produto, 'id'>): Promise<Produto> {
  const res = await fetch(`${API_BASE}/produtos`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(p)
  })
  if (!res.ok) throw new Error('Erro ao criar produto')
  return res.json()
}

export async function updateProduct(id: number, p: Partial<Produto>): Promise<Produto> {
  const res = await fetch(`${API_BASE}/produtos/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(p)
  })
  if (!res.ok) throw new Error('Erro ao atualizar produto')
  return res.json()
}

export async function deleteProduct(id: number): Promise<void> {
  const res = await fetch(`${API_BASE}/produtos/${id}`, { method: 'DELETE' })
  if (!res.ok) throw new Error('Erro ao excluir produto')
}

