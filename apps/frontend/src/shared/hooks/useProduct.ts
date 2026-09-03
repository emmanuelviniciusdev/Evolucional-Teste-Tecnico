import { useEffect, useState } from 'react'
import { getProduct, Produto } from '../api/products'

interface UseProductResult {
  data: Produto | null
  loading: boolean
  error: string | null
}

export function useProduct(id: number): UseProductResult {
  const [data, setData] = useState<Produto | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    let cancelled = false
    setLoading(true)
    setError(null)
    setData(null)

    getProduct(id)
      .then((produto) => {
        if (!cancelled) setData(produto)
      })
      .catch((err: unknown) => {
        if (!cancelled) {
          setError(err instanceof Error ? err.message : 'Erro ao carregar produto')
        }
      })
      .finally(() => {
        if (!cancelled) setLoading(false)
      })

    return () => {
      cancelled = true
    }
  }, [id])

  return { data, loading, error }
}
