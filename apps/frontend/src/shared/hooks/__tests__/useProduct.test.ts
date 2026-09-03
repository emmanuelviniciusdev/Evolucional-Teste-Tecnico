import { renderHook, waitFor } from '@testing-library/react'
import { useProduct } from '../useProduct'
import * as api from '@/shared/api/products'
import { Produto } from '@/shared/api/products'

vi.mock('@/shared/api/products')

const mockProduto: Produto = {
  id: 1, nome: 'Teclado', categoria: 'Perifericos', preco: 100, estoque: 10, ativo: true,
}

afterEach(() => vi.clearAllMocks())

test('starts in loading state', () => {
  vi.mocked(api.getProduct).mockResolvedValue(mockProduto)
  const { result } = renderHook(() => useProduct(1))
  expect(result.current.loading).toBe(true)
  expect(result.current.data).toBeNull()
  expect(result.current.error).toBeNull()
})

test('resolves with product data', async () => {
  vi.mocked(api.getProduct).mockResolvedValue(mockProduto)
  const { result } = renderHook(() => useProduct(1))
  await waitFor(() => expect(result.current.loading).toBe(false))
  expect(result.current.data).toEqual(mockProduto)
  expect(result.current.error).toBeNull()
})

test('sets error when fetch fails', async () => {
  vi.mocked(api.getProduct).mockRejectedValue(new Error('Erro ao obter produto'))
  const { result } = renderHook(() => useProduct(99))
  await waitFor(() => expect(result.current.loading).toBe(false))
  expect(result.current.data).toBeNull()
  expect(result.current.error).toBe('Erro ao obter produto')
})
