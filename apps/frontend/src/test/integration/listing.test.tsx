import React from 'react'
import { render, screen, waitFor, fireEvent } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import Home from '@/pages/Home'

const wrap = (route = '/') =>
  render(
    <MemoryRouter initialEntries={[route]}>
      <Home />
    </MemoryRouter>,
  )

test('products appear on initial load', async () => {
  wrap()
  await waitFor(() => {
    expect(screen.getByText('Teclado')).not.toBeNull()
  })
})

test('shows total product count from X-Total-Count', async () => {
  wrap()
  await waitFor(() => {
    // Handlers have 11 products; expect count display
    expect(screen.getByText(/11 produto/)).not.toBeNull()
  })
})

test('pagination controls appear when more than PAGE_SIZE products exist', async () => {
  // 11 products > PAGE_SIZE(10) so pagination should appear
  wrap()
  await waitFor(() => {
    expect(screen.getByRole('navigation', { name: 'Paginação' })).not.toBeNull()
  })
})

test('search input triggers a filtered request', async () => {
  wrap()
  // Wait for initial load
  await waitFor(() => expect(screen.getByText('Teclado')).not.toBeNull())

  const searchInput = screen.getByRole('searchbox')
  fireEvent.change(searchInput, { target: { value: 'Monitor' } })

  await waitFor(() => {
    // After debounce (fake timers not used here — actual 300ms), MSW returns filtered results
    // The handler filters by nome_like, so "Teclado" should no longer appear
    expect(screen.queryByText('Teclado')).toBeNull()
    expect(screen.getByText('Monitor')).not.toBeNull()
  }, { timeout: 1000 })
})

test('category filter shows only matching products', async () => {
  wrap()
  await waitFor(() => expect(screen.getByText('Teclado')).not.toBeNull())

  const select = screen.getByRole('combobox', { name: 'Filtrar por categoria' })
  fireEvent.change(select, { target: { value: 'Audio' } })

  await waitFor(() => {
    expect(screen.getByText('Headset')).not.toBeNull()
    expect(screen.queryByText('Teclado')).toBeNull()
  }, { timeout: 1000 })
})

test('existing integration: listProducts parses X-Total-Count and items', async () => {
  // Keep backward compatibility with the original integration smoke
  const { listProducts } = await import('@/shared/api/products')
  const res = await listProducts({ page: 1, limit: 10 })
  expect(res.total).toBeGreaterThanOrEqual(2)
  expect(res.items.length).toBeGreaterThanOrEqual(1)
})
