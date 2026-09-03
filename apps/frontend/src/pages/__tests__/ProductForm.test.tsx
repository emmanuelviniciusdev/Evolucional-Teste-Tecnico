import React from 'react'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { MemoryRouter, Route, Routes } from 'react-router-dom'
import ProductForm from '../ProductForm'
import * as productHook from '@/shared/hooks/useProduct'
import * as saveHook from '@/shared/hooks/useSaveProduct'

vi.mock('@/shared/hooks/useProduct')
vi.mock('@/shared/hooks/useSaveProduct')

afterEach(() => vi.clearAllMocks())

const noopSave = { save: vi.fn().mockResolvedValue(null), loading: false, error: null, success: false, reset: vi.fn() }

const renderCreate = () =>
  render(
    <MemoryRouter initialEntries={['/produtos/novo']}>
      <Routes>
        <Route path="/produtos/novo" element={<ProductForm />} />
      </Routes>
    </MemoryRouter>,
  )

const renderEdit = (id = '1') =>
  render(
    <MemoryRouter initialEntries={[`/produtos/${id}/editar`]}>
      <Routes>
        <Route path="/produtos/:id/editar" element={<ProductForm />} />
      </Routes>
    </MemoryRouter>,
  )

test('renders empty form fields in create mode', () => {
  vi.mocked(productHook.useProduct).mockReturnValue({ data: null, loading: false, error: null })
  vi.mocked(saveHook.useSaveProduct).mockReturnValue(noopSave)
  renderCreate()
  expect(screen.getByLabelText('Nome *')).not.toBeNull()
  expect(screen.getByLabelText('Preço *')).not.toBeNull()
  expect(screen.getByLabelText('Estoque *')).not.toBeNull()
})

test('shows field-level error when name is too short', async () => {
  vi.mocked(productHook.useProduct).mockReturnValue({ data: null, loading: false, error: null })
  vi.mocked(saveHook.useSaveProduct).mockReturnValue(noopSave)
  renderCreate()

  fireEvent.change(screen.getByLabelText('Nome *'), { target: { value: 'AB' } })
  fireEvent.change(screen.getByLabelText('Preço *'), { target: { value: '10' } })
  fireEvent.change(screen.getByLabelText('Estoque *'), { target: { value: '1' } })
  fireEvent.click(screen.getByText('Salvar'))

  await waitFor(() =>
    expect(screen.getByText('Nome deve ter ao menos 3 caracteres')).not.toBeNull(),
  )
  expect(noopSave.save).not.toHaveBeenCalled()
})

test('shows field-level error when price is zero or negative', async () => {
  vi.mocked(productHook.useProduct).mockReturnValue({ data: null, loading: false, error: null })
  vi.mocked(saveHook.useSaveProduct).mockReturnValue(noopSave)
  renderCreate()

  fireEvent.change(screen.getByLabelText('Nome *'), { target: { value: 'Produto Teste' } })
  fireEvent.change(screen.getByLabelText('Preço *'), { target: { value: '0' } })
  fireEvent.change(screen.getByLabelText('Estoque *'), { target: { value: '1' } })
  fireEvent.click(screen.getByText('Salvar'))

  await waitFor(() =>
    expect(screen.getByText('Preço deve ser maior que zero')).not.toBeNull(),
  )
  expect(noopSave.save).not.toHaveBeenCalled()
})

test('shows field-level error when stock is negative', async () => {
  vi.mocked(productHook.useProduct).mockReturnValue({ data: null, loading: false, error: null })
  vi.mocked(saveHook.useSaveProduct).mockReturnValue(noopSave)
  renderCreate()

  fireEvent.change(screen.getByLabelText('Nome *'), { target: { value: 'Produto Teste' } })
  fireEvent.change(screen.getByLabelText('Preço *'), { target: { value: '50' } })
  fireEvent.change(screen.getByLabelText('Estoque *'), { target: { value: '-1' } })
  fireEvent.click(screen.getByText('Salvar'))

  await waitFor(() =>
    expect(screen.getByText('Estoque não pode ser negativo')).not.toBeNull(),
  )
  expect(noopSave.save).not.toHaveBeenCalled()
})

test('pre-populates form with existing product in edit mode', async () => {
  vi.mocked(productHook.useProduct).mockReturnValue({
    data: { id: 1, nome: 'Teclado', categoria: 'Perifericos', preco: 100, estoque: 10, ativo: true },
    loading: false,
    error: null,
  })
  vi.mocked(saveHook.useSaveProduct).mockReturnValue(noopSave)
  renderEdit('1')

  await waitFor(() => {
    const input = screen.getByLabelText('Nome *') as HTMLInputElement
    expect(input.value).toBe('Teclado')
  })
})

test('disables submit button when loading/saving', () => {
  vi.mocked(productHook.useProduct).mockReturnValue({ data: null, loading: false, error: null })
  vi.mocked(saveHook.useSaveProduct).mockReturnValue({ ...noopSave, loading: true })
  renderCreate()
  const btn = screen.getByText('Salvando…') as HTMLButtonElement
  expect(btn.disabled).toBe(true)
})
