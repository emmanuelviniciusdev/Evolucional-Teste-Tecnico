import React from 'react'
import { render } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import AppShell from '@/app/AppShell'

test('shell renders with pt-BR lang and main landmark', () => {
  render(
    <MemoryRouter>
      <AppShell />
    </MemoryRouter>,
  )
  expect(document.documentElement.lang).toBe('pt-BR')
  expect(document.querySelector('main')).not.toBeNull()
})
