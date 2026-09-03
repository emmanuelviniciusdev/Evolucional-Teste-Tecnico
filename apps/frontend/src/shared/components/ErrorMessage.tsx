import React from 'react'

interface ErrorMessageProps {
  message: string
}

const ErrorMessage: React.FC<ErrorMessageProps> = ({ message }) => (
  <div role="alert" className="error-message">
    <strong>Erro:</strong> {message}
  </div>
)

export default ErrorMessage
