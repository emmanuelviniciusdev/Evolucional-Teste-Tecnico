import React, { useEffect, useRef, useState } from 'react'
import { Link, useSearchParams } from 'react-router-dom'
import { useProducts } from '../shared/hooks/useProducts'
import { useDeleteProduct } from '../shared/hooks/useDeleteProduct'
import { useDebounce } from '../shared/hooks/useDebounce'
import LoadingSpinner from '../shared/components/LoadingSpinner'
import ErrorMessage from '../shared/components/ErrorMessage'
import ConfirmDialog from '../shared/components/ConfirmDialog'

const PAGE_SIZE = 10

const CATEGORIAS = [
  'Perifericos',
  'Monitores',
  'Audio',
  'Armazenamento',
  'Componentes',
  'Acessorios',
]

const Home: React.FC = () => {
  const [searchParams, setSearchParams] = useSearchParams()

  const page = Number(searchParams.get('page') || '1')
  const urlQ = searchParams.get('q') || ''
  const urlCategoria = searchParams.get('categoria') || ''

  // Local input state — initialised from URL so reload restores the field value
  const [inputValue, setInputValue] = useState(urlQ)
  const debouncedSearch = useDebounce(inputValue, 300)

  // Skip URL sync on the very first render (value already matches the URL)
  const mountedRef = useRef(false)
  useEffect(() => {
    if (!mountedRef.current) {
      mountedRef.current = true
      return
    }
    setSearchParams(
      (prev) => {
        const next = new URLSearchParams(prev)
        if (debouncedSearch) next.set('q', debouncedSearch)
        else next.delete('q')
        next.set('page', '1')
        return next
      },
      { replace: true },
    )
  }, [debouncedSearch, setSearchParams])

  const handleCategoriaChange = (value: string) => {
    setSearchParams(
      (prev) => {
        const next = new URLSearchParams(prev)
        if (value) next.set('categoria', value)
        else next.delete('categoria')
        next.set('page', '1')
        return next
      },
      { replace: true },
    )
  }

  const handlePageChange = (newPage: number) => {
    setSearchParams((prev) => {
      const next = new URLSearchParams(prev)
      next.set('page', String(newPage))
      return next
    })
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }

  const { data: produtos, total, loading, error, refetch } = useProducts({
    page,
    limit: PAGE_SIZE,
    search: debouncedSearch,
    categoria: urlCategoria,
  })

  const totalPages = Math.ceil(total / PAGE_SIZE)

  // Delete flow
  const [deleteTargetId, setDeleteTargetId] = useState<number | null>(null)
  const [deleteSuccessMsg, setDeleteSuccessMsg] = useState<string | null>(null)
  const { deleteProduct, loading: deleteLoading, error: deleteError } = useDeleteProduct()

  const handleDeleteConfirm = async () => {
    if (deleteTargetId === null) return
    const ok = await deleteProduct(deleteTargetId)
    setDeleteTargetId(null)
    if (ok) {
      setDeleteSuccessMsg('Produto excluído com sucesso.')
      if (produtos.length === 1 && page > 1) {
        handlePageChange(page - 1)
      } else {
        refetch()
      }
    }
  }

  return (
    <section>
      <div className="listing-header">
        <h2>Catálogo de Produtos</h2>
        <Link to="/produtos/novo" className="btn btn-primary">
          + Novo Produto
        </Link>
      </div>

      {/* Search and filter bar */}
      <div className="filter-bar">
        <input
          type="search"
          placeholder="Buscar por nome…"
          aria-label="Buscar produto por nome"
          value={inputValue}
          onChange={(e) => setInputValue(e.target.value)}
          className="search-input"
        />
        <select
          aria-label="Filtrar por categoria"
          value={urlCategoria}
          onChange={(e) => handleCategoriaChange(e.target.value)}
          className="category-select"
        >
          <option value="">Todas as categorias</option>
          {CATEGORIAS.map((cat) => (
            <option key={cat} value={cat}>
              {cat}
            </option>
          ))}
        </select>
      </div>

      {/* Feedback banners */}
      {deleteError && <ErrorMessage message={deleteError} />}
      {deleteSuccessMsg && (
        <div role="status" className="success-message">
          {deleteSuccessMsg}
        </div>
      )}

      {/* Main content area */}
      {loading ? (
        <LoadingSpinner />
      ) : error ? (
        <ErrorMessage message={error} />
      ) : produtos.length === 0 ? (
        <p className="empty-state">Nenhum produto encontrado.</p>
      ) : (
        <>
          <p className="total-count">
            {total} {total === 1 ? 'produto encontrado' : 'produtos encontrados'}
          </p>

          <table className="products-table">
            <thead>
              <tr>
                <th>Nome</th>
                <th>Categoria</th>
                <th>Preço</th>
                <th>Estoque</th>
                <th>Ativo</th>
                <th>Ações</th>
              </tr>
            </thead>
            <tbody>
              {produtos.map((p) => (
                <tr key={p.id}>
                  <td>
                    <Link to={`/produtos/${p.id}`}>{p.nome}</Link>
                  </td>
                  <td>{p.categoria}</td>
                  <td>R$ {p.preco.toFixed(2)}</td>
                  <td>{p.estoque}</td>
                  <td>{p.ativo ? 'Sim' : 'Não'}</td>
                  <td className="actions-cell">
                    <Link to={`/produtos/${p.id}/editar`} className="btn btn-sm">
                      Editar
                    </Link>
                    <button
                      type="button"
                      className="btn btn-sm btn-danger"
                      onClick={() => setDeleteTargetId(p.id)}
                      disabled={deleteLoading}
                      aria-label={`Excluir ${p.nome}`}
                    >
                      Excluir
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>

          {/* Pagination */}
          {totalPages > 1 && (
            <nav aria-label="Paginação" className="pagination">
              <button
                type="button"
                onClick={() => handlePageChange(page - 1)}
                disabled={page <= 1}
                aria-label="Página anterior"
              >
                ‹
              </button>
              <span>
                Página {page} de {totalPages}
              </span>
              <button
                type="button"
                onClick={() => handlePageChange(page + 1)}
                disabled={page >= totalPages}
                aria-label="Próxima página"
              >
                ›
              </button>
            </nav>
          )}
        </>
      )}

      {/* Delete confirmation dialog */}
      <ConfirmDialog
        open={deleteTargetId !== null}
        title="Excluir produto"
        message="Tem certeza que deseja excluir este produto? Esta ação não pode ser desfeita."
        confirmLabel="Excluir"
        cancelLabel="Cancelar"
        onConfirm={handleDeleteConfirm}
        onCancel={() => setDeleteTargetId(null)}
      />
    </section>
  )
}

export default Home
