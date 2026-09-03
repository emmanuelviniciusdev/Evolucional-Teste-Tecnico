import React from 'react'
import { render, screen } from '@testing-library/react'
import LoadingSpinner from '../LoadingSpinner'

test('renders a loading status region', () => {
  render(<LoadingSpinner />)
  expect(screen.getByRole('status')).not.toBeNull()
})

test('contains accessible screen-reader text', () => {
  render(<LoadingSpinner />)
  expect(screen.getByText('Carregando...')).not.toBeNull()
})
