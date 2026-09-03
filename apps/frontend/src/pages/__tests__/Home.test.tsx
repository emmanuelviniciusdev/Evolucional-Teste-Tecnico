import React from 'react'
import { render, screen, fireEvent } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import Home from '../Home'
import * as useProductsModule from '@/shared/hooks/useProducts'
import * as useDeleteModule from '@/shared/hooks/useDeleteProduct'

vi.mock('@/shared/hooks/useProducts')
vi.mock('@/shared/hooks/useDeleteProduct')

const noopDelete = { deleteProduct: vi.fn(), loading: false, error: null }

afterEach(() => vi.clearAllMocks())

const wrap = (ui: React.ReactElement) =>
  render(<MemoryRouter>{ui}</MemoryRouter>)

test('shows loading spinner while fetching', () => {
  vi.mocked(useProductsModule.useProducts).mockReturnValue({
    data: [], total: 0, loading: true, error: null, refetch: vi.fn(),
  })
  vi.mocked(useDeleteModule.useDeleteProduct).mockReturnValue(noopDelete)
  wrap(<Home />)
  expect(screen.getByRole('status')).not.toBeNull()
})

test('shows error message when fetch fails', () => {
  vi.mocked(useProductsModule.useProducts).mockReturnValue({
    data: [], total: 0, loading: false, error: 'Erro de rede', refetch: vi.fn(),
  })
  vi.mocked(useDeleteModule.useDeleteProduct).mockReturnValue(noopDelete)
  wrap(<Home />)
  expect(screen.getByRole('alert')).not.toBeNull()
  expect(screen.getByText('Erro de rede')).not.toBeNull()
})

test('shows empty state when no products are returned', () => {
  vi.mocked(useProductsModule.useProducts).mockReturnValue({
    data: [], total: 0, loading: false, error: null, refetch: vi.fn(),
  })
  vi.mocked(useDeleteModule.useDeleteProduct).mockReturnValue(noopDelete)
  wrap(<Home />)
  expect(screen.getByText('Nenhum produto encontrado.')).not.toBeNull()
})

test('renders product rows when data is available', () => {
  vi.mocked(useProductsModule.useProducts).mockReturnValue({
    data: [
      { id: 1, nome: 'Teclado', categoria: 'Perifericos', preco: 100, estoque: 10, ativo: true },
    ],
    total: 1,
    loading: false,
    error: null,
    refetch: vi.fn(),
  })
  vi.mocked(useDeleteModule.useDeleteProduct).mockReturnValue(noopDelete)
  wrap(<Home />)
  expect(screen.getByText('Teclado')).not.toBeNull()
})

test('cancel on delete dialog does NOT call deleteProduct', () => {
  const del = vi.fn()
  vi.mocked(useProductsModule.useProducts).mockReturnValue({
    data: [
      { id: 1, nome: 'Teclado', categoria: 'Perifericos', preco: 100, estoque: 10, ativo: true },
    ],
    total: 1,
    loading: false,
    error: null,
    refetch: vi.fn(),
  })
  vi.mocked(useDeleteModule.useDeleteProduct).mockReturnValue({
    deleteProduct: del, loading: false, error: null,
  })

  // jsdom stub
  HTMLDialogElement.prototype.showModal = vi.fn()
  HTMLDialogElement.prototype.close = vi.fn()

  const { getByRole } = wrap(<Home />)

  // Open dialog via the row's aria-label button
  fireEvent.click(getByRole('button', { name: 'Excluir Teclado' }))
  // Cancel via the dialog's cancel button
  fireEvent.click(getByRole('button', { name: 'Cancelar', hidden: true }))

  expect(del).not.toHaveBeenCalled()
})
