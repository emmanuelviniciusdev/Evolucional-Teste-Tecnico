import React from 'react'
import { render, screen } from '@testing-library/react'
import ErrorMessage from '../ErrorMessage'

test('renders the provided message text', () => {
  render(<ErrorMessage message="Algo deu errado" />)
  expect(screen.getByText('Algo deu errado')).not.toBeNull()
})

test('renders an alert role', () => {
  render(<ErrorMessage message="Erro de teste" />)
  expect(screen.getByRole('alert')).not.toBeNull()
})
