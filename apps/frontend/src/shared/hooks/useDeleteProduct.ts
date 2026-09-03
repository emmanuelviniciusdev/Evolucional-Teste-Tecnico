import { useState } from 'react'
import { deleteProduct } from '../api/products'

interface UseDeleteProductResult {
  deleteProduct: (id: number) => Promise<boolean>
  loading: boolean
  error: string | null
}

export function useDeleteProduct(): UseDeleteProductResult {
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const del = async (id: number): Promise<boolean> => {
    setLoading(true)
    setError(null)

    try {
      await deleteProduct(id)
      return true
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Erro ao excluir produto')
      return false
    } finally {
      setLoading(false)
    }
  }

  return { deleteProduct: del, loading, error }
}
