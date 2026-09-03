import { renderHook, act } from '@testing-library/react'
import { useSaveProduct } from '../useSaveProduct'
import * as api from '@/shared/api/products'
import { Produto } from '@/shared/api/products'

vi.mock('@/shared/api/products')

const mockProduto: Produto = {
  id: 101, nome: 'Novo', categoria: 'Acessorios', preco: 50, estoque: 5, ativo: true,
}
const formData = { nome: 'Novo', categoria: 'Acessorios', preco: 50, estoque: 5, ativo: true }

afterEach(() => vi.clearAllMocks())

test('save (create) returns product and sets success', async () => {
  vi.mocked(api.createProduct).mockResolvedValue(mockProduto)
  const { result } = renderHook(() => useSaveProduct())

  let savedProduct: Produto | null = null
  await act(async () => {
    savedProduct = await result.current.save(formData)
  })

  expect(savedProduct).toEqual(mockProduto)
  expect(result.current.success).toBe(true)
  expect(result.current.error).toBeNull()
})

test('save (update) calls updateProduct with id', async () => {
  vi.mocked(api.updateProduct).mockResolvedValue({ ...mockProduto, id: 5 })
  const { result } = renderHook(() => useSaveProduct())

  await act(async () => {
    await result.current.save(formData, 5)
  })

  expect(api.updateProduct).toHaveBeenCalledWith(5, formData)
  expect(result.current.success).toBe(true)
})

test('sets error when save fails', async () => {
  vi.mocked(api.createProduct).mockRejectedValue(new Error('Erro ao criar produto'))
  const { result } = renderHook(() => useSaveProduct())

  let savedProduct: Produto | null = null
  await act(async () => {
    savedProduct = await result.current.save(formData)
  })

  expect(savedProduct).toBeNull()
  expect(result.current.success).toBe(false)
  expect(result.current.error).toBe('Erro ao criar produto')
})
