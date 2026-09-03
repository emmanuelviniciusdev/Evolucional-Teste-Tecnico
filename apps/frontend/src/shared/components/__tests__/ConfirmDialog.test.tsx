import React from 'react'
import { render, screen, fireEvent } from '@testing-library/react'
import ConfirmDialog from '../ConfirmDialog'

// jsdom does not implement showModal/close — provide no-op stubs
beforeEach(() => {
  HTMLDialogElement.prototype.showModal = vi.fn()
  HTMLDialogElement.prototype.close = vi.fn()
})

test('renders message when open is true', () => {
  render(
    <ConfirmDialog
      open
      message="Tem certeza?"
      onConfirm={vi.fn()}
      onCancel={vi.fn()}
    />,
  )
  expect(screen.getByText('Tem certeza?')).not.toBeNull()
})

test('calls showModal when open becomes true', () => {
  render(
    <ConfirmDialog
      open
      message="Confirmar?"
      onConfirm={vi.fn()}
      onCancel={vi.fn()}
    />,
  )
  expect(HTMLDialogElement.prototype.showModal).toHaveBeenCalled()
})

test('fires onConfirm when the confirm button is clicked', () => {
  const onConfirm = vi.fn()
  render(
    <ConfirmDialog
      open
      message="Excluir?"
      onConfirm={onConfirm}
      onCancel={vi.fn()}
    />,
  )
  fireEvent.click(screen.getByRole('button', { name: 'Confirmar' }))
  expect(onConfirm).toHaveBeenCalledOnce()
})

test('fires onCancel when the cancel button is clicked', () => {
  const onCancel = vi.fn()
  render(
    <ConfirmDialog
      open
      message="Excluir?"
      onConfirm={vi.fn()}
      onCancel={onCancel}
    />,
  )
  fireEvent.click(screen.getByRole('button', { name: 'Cancelar' }))
  expect(onCancel).toHaveBeenCalledOnce()
})

test('does NOT call onConfirm when cancel is clicked', () => {
  const onConfirm = vi.fn()
  render(
    <ConfirmDialog
      open
      message="Excluir?"
      onConfirm={onConfirm}
      onCancel={vi.fn()}
    />,
  )
  fireEvent.click(screen.getByRole('button', { name: 'Cancelar' }))
  expect(onConfirm).not.toHaveBeenCalled()
})
