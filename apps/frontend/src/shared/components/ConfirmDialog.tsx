import React, { useEffect, useRef } from 'react'

interface ConfirmDialogProps {
  open: boolean
  title?: string
  message: string
  confirmLabel?: string
  cancelLabel?: string
  onConfirm: () => void
  onCancel: () => void
}

const ConfirmDialog: React.FC<ConfirmDialogProps> = ({
  open,
  title = 'Confirmar',
  message,
  confirmLabel = 'Confirmar',
  cancelLabel = 'Cancelar',
  onConfirm,
  onCancel,
}) => {
  const dialogRef = useRef<HTMLDialogElement>(null)

  useEffect(() => {
    const dialog = dialogRef.current
    if (!dialog) return
    if (open) {
      if (typeof dialog.showModal === 'function') dialog.showModal()
    } else {
      if (typeof dialog.close === 'function') dialog.close()
    }
  }, [open])

  return (
    <dialog
      ref={dialogRef}
      open={open}
      className="confirm-dialog"
      aria-labelledby="dialog-title"
      aria-describedby="dialog-desc"
      onClose={onCancel}
    >
      <h3 id="dialog-title">{title}</h3>
      <p id="dialog-desc">{message}</p>
      <div className="dialog-actions">
        <button type="button" className="btn btn-danger" onClick={onConfirm}>
          {confirmLabel}
        </button>
        <button type="button" className="btn btn-secondary" onClick={onCancel}>
          {cancelLabel}
        </button>
      </div>
    </dialog>
  )
}

export default ConfirmDialog
