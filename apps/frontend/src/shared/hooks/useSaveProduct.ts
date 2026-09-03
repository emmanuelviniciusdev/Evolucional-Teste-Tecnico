import { useState } from 'react'
import { createProduct, updateProduct, Produto } from '../api/products'

interface UseSaveProductResult {
  save: (data: Omit<Produto, 'id'>, id?: number) => Promise<Produto | null>
  loading: boolean
  error: string | null
  success: boolean
  reset: () => void
}

export function useSaveProduct(): UseSaveProductResult {
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState(false)

  const reset = () => {
    setError(null)
    setSuccess(false)
  }

  const save = async (data: Omit<Produto, 'id'>, id?: number): Promise<Produto | null> => {
    setLoading(true)
    setError(null)
    setSuccess(false)

    try {
      const result = id ? await updateProduct(id, data) : await createProduct(data)
      setSuccess(true)
      return result
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Erro ao salvar produto')
      return null
    } finally {
      setLoading(false)
    }
  }

  return { save, loading, error, success, reset }
}
