import { useEffect, useState } from 'react'
import { listProducts, Produto } from '../api/products'

interface UseProductsParams {
  page?: number
  limit?: number
  search?: string
  categoria?: string
}

interface UseProductsResult {
  data: Produto[]
  total: number
  loading: boolean
  error: string | null
}

export function useProducts({
  page = 1,
  limit = 10,
  search = '',
  categoria = '',
}: UseProductsParams = {}): UseProductsResult {
  const [data, setData] = useState<Produto[]>([])
  const [total, setTotal] = useState(0)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    let cancelled = false
    setLoading(true)
    setError(null)

    listProducts({
      page,
      limit,
      nome: search || undefined,
      categoria: categoria || undefined,
    })
      .then((result) => {
        if (!cancelled) {
          setData(result.items)
          setTotal(result.total)
        }
      })
      .catch((err: unknown) => {
        if (!cancelled) {
          setError(err instanceof Error ? err.message : 'Erro ao carregar produtos')
        }
      })
      .finally(() => {
        if (!cancelled) setLoading(false)
      })

    return () => {
      cancelled = true
    }
  }, [page, limit, search, categoria])

  return { data, total, loading, error }
}
