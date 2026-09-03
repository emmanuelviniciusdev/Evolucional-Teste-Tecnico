import { renderHook, act } from '@testing-library/react'
import { useDeleteProduct } from '../useDeleteProduct'
import * as api from '@/shared/api/products'

vi.mock('@/shared/api/products')

afterEach(() => vi.clearAllMocks())

test('deleteProduct returns true on success', async () => {
  vi.mocked(api.deleteProduct).mockResolvedValue(undefined)
  const { result } = renderHook(() => useDeleteProduct())

  let ok = false
  await act(async () => {
    ok = await result.current.deleteProduct(1)
  })

  expect(ok).toBe(true)
  expect(result.current.error).toBeNull()
})

test('deleteProduct returns false and sets error on failure', async () => {
  vi.mocked(api.deleteProduct).mockRejectedValue(new Error('Erro ao excluir produto'))
  const { result } = renderHook(() => useDeleteProduct())

  let ok = true
  await act(async () => {
    ok = await result.current.deleteProduct(99)
  })

  expect(ok).toBe(false)
  expect(result.current.error).toBe('Erro ao excluir produto')
})
