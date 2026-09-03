import React from 'react'
import { render, screen } from '@testing-library/react'
import { MemoryRouter, Route, Routes } from 'react-router-dom'
import ProductDetail from '../ProductDetail'
import * as hook from '@/shared/hooks/useProduct'
import { Produto } from '@/shared/api/products'

vi.mock('@/shared/hooks/useProduct')

const mockProduto: Produto = {
  id: 1, nome: 'Teclado Mecanico', categoria: 'Perifericos', preco: 349.9, estoque: 15, ativo: true,
}

afterEach(() => vi.clearAllMocks())

const wrap = (id = '1') =>
  render(
    <MemoryRouter initialEntries={[`/produtos/${id}`]}>
      <Routes>
        <Route path="/produtos/:id" element={<ProductDetail />} />
      </Routes>
    </MemoryRouter>,
  )

test('shows loading spinner while fetching', () => {
  vi.mocked(hook.useProduct).mockReturnValue({ data: null, loading: true, error: null })
  wrap()
  expect(screen.getByRole('status')).not.toBeNull()
})

test('shows error message on fetch failure', () => {
  vi.mocked(hook.useProduct).mockReturnValue({ data: null, loading: false, error: 'Produto não encontrado' })
  wrap()
  expect(screen.getByRole('alert')).not.toBeNull()
  expect(screen.getByText('Produto não encontrado')).not.toBeNull()
})

test('renders all product fields when loaded', () => {
  vi.mocked(hook.useProduct).mockReturnValue({ data: mockProduto, loading: false, error: null })
  wrap()
  expect(screen.getByText('Teclado Mecanico')).not.toBeNull()
  expect(screen.getByText('Perifericos')).not.toBeNull()
  expect(screen.getByText('R$ 349.90')).not.toBeNull()
  expect(screen.getByText('15')).not.toBeNull()
  expect(screen.getAllByText('Sim').length).toBeGreaterThan(0)
})

test('renders a back link to / and an edit link', () => {
  vi.mocked(hook.useProduct).mockReturnValue({ data: mockProduto, loading: false, error: null })
  wrap()
  const backLink = screen.getByText('← Voltar ao Catálogo') as HTMLAnchorElement
  expect(backLink.getAttribute('href')).toBe('/')
  const editLink = screen.getByText('Editar') as HTMLAnchorElement
  expect(editLink.getAttribute('href')).toBe('/produtos/1/editar')
})
