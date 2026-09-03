import React from 'react'
import { Link, useParams } from 'react-router-dom'
import { useProduct } from '../shared/hooks/useProduct'
import LoadingSpinner from '../shared/components/LoadingSpinner'
import ErrorMessage from '../shared/components/ErrorMessage'

const ProductDetail: React.FC = () => {
  const { id } = useParams<{ id: string }>()
  const numericId = Number(id)

  const { data: produto, loading, error } = useProduct(numericId)

  if (loading) return <LoadingSpinner />
  if (error) return <ErrorMessage message={error} />
  if (!produto) return null

  return (
    <article className="product-detail">
      <nav className="detail-nav" aria-label="Navegação do detalhe">
        <Link to="/" className="btn btn-secondary">
          ← Voltar ao Catálogo
        </Link>
        <Link to={`/produtos/${produto.id}/editar`} className="btn btn-primary">
          Editar
        </Link>
      </nav>

      <h2>{produto.nome}</h2>

      <dl className="product-fields">
        <div className="field-row">
          <dt>ID</dt>
          <dd>{produto.id}</dd>
        </div>
        <div className="field-row">
          <dt>Categoria</dt>
          <dd>{produto.categoria}</dd>
        </div>
        <div className="field-row">
          <dt>Preço</dt>
          <dd>R$ {produto.preco.toFixed(2)}</dd>
        </div>
        <div className="field-row">
          <dt>Estoque</dt>
          <dd>{produto.estoque}</dd>
        </div>
        <div className="field-row">
          <dt>Ativo</dt>
          <dd>{produto.ativo ? 'Sim' : 'Não'}</dd>
        </div>
      </dl>
    </article>
  )
}

export default ProductDetail
