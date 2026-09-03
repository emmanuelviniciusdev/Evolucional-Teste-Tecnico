import React from 'react'

const LoadingSpinner: React.FC = () => (
  <div role="status" className="loading-spinner">
    <span className="spinner-icon" aria-hidden="true" />
    <span className="sr-only">Carregando...</span>
  </div>
)

export default LoadingSpinner
