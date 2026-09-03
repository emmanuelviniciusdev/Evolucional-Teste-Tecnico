import React, { useEffect } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { useProduct } from '../shared/hooks/useProduct'
import { useSaveProduct } from '../shared/hooks/useSaveProduct'
import LoadingSpinner from '../shared/components/LoadingSpinner'
import ErrorMessage from '../shared/components/ErrorMessage'

interface ProductFormData {
  nome: string
  categoria: string
  preco: number
  estoque: number
  ativo: boolean
}

const CATEGORIAS = [
  'Perifericos',
  'Monitores',
  'Audio',
  'Armazenamento',
  'Componentes',
  'Acessorios',
]

const ProductForm: React.FC = () => {
  const { id } = useParams<{ id?: string }>()
  const isEdit = id !== undefined
  const numericId = isEdit ? Number(id) : undefined

  const navigate = useNavigate()

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<ProductFormData>({
    defaultValues: {
      nome: '',
      categoria: '',
      preco: 0,
      estoque: 0,
      ativo: true,
    },
  })

  // In edit mode, load existing product and populate the form
  const { data: existing, loading: loadingProduct, error: loadError } = useProduct(
    numericId ?? 0,
  )

  useEffect(() => {
    if (isEdit && existing) {
      reset({
        nome: existing.nome,
        categoria: existing.categoria,
        preco: existing.preco,
        estoque: existing.estoque,
        ativo: existing.ativo,
      })
    }
  }, [isEdit, existing, reset])

  const { save, loading: saving, error: saveError, success } = useSaveProduct()

  const onSubmit = async (data: ProductFormData) => {
    const result = await save(
      {
        nome: data.nome,
        categoria: data.categoria,
        preco: Number(data.preco),
        estoque: Number(data.estoque),
        ativo: data.ativo,
      },
      numericId,
    )
    if (result) {
      // Navigate to the list after a short delay so the success message is visible
      setTimeout(() => navigate('/'), 1200)
    }
  }

  if (isEdit && loadingProduct) return <LoadingSpinner />
  if (isEdit && loadError) return <ErrorMessage message={loadError} />

  return (
    <section className="product-form-page">
      <nav aria-label="Navegação do formulário">
        <Link to="/" className="btn btn-secondary">
          ← Voltar ao Catálogo
        </Link>
      </nav>

      <h2>{isEdit ? 'Editar Produto' : 'Novo Produto'}</h2>

      {success && (
        <div role="status" className="success-message">
          Produto {isEdit ? 'atualizado' : 'criado'} com sucesso!
        </div>
      )}
      {saveError && <ErrorMessage message={saveError} />}

      <form onSubmit={handleSubmit(onSubmit)} noValidate className="product-form">
        {/* Nome */}
        <div className="form-field">
          <label htmlFor="nome">Nome *</label>
          <input
            id="nome"
            type="text"
            aria-invalid={errors.nome ? 'true' : 'false'}
            {...register('nome', {
              required: 'Nome é obrigatório',
              minLength: { value: 3, message: 'Nome deve ter ao menos 3 caracteres' },
            })}
          />
          {errors.nome && (
            <span role="alert" className="field-error">
              {errors.nome.message}
            </span>
          )}
        </div>

        {/* Categoria */}
        <div className="form-field">
          <label htmlFor="categoria">Categoria</label>
          <select id="categoria" {...register('categoria')}>
            <option value="">Selecione…</option>
            {CATEGORIAS.map((cat) => (
              <option key={cat} value={cat}>
                {cat}
              </option>
            ))}
          </select>
        </div>

        {/* Preço */}
        <div className="form-field">
          <label htmlFor="preco">Preço *</label>
          <input
            id="preco"
            type="number"
            step="0.01"
            aria-invalid={errors.preco ? 'true' : 'false'}
            {...register('preco', {
              required: 'Preço é obrigatório',
              validate: (v) => Number(v) > 0 || 'Preço deve ser maior que zero',
            })}
          />
          {errors.preco && (
            <span role="alert" className="field-error">
              {errors.preco.message}
            </span>
          )}
        </div>

        {/* Estoque */}
        <div className="form-field">
          <label htmlFor="estoque">Estoque *</label>
          <input
            id="estoque"
            type="number"
            aria-invalid={errors.estoque ? 'true' : 'false'}
            {...register('estoque', {
              required: 'Estoque é obrigatório',
              validate: (v) => Number(v) >= 0 || 'Estoque não pode ser negativo',
            })}
          />
          {errors.estoque && (
            <span role="alert" className="field-error">
              {errors.estoque.message}
            </span>
          )}
        </div>

        {/* Ativo */}
        <div className="form-field form-field--checkbox">
          <label>
            <input type="checkbox" {...register('ativo')} />
            &nbsp;Produto ativo
          </label>
        </div>

        <div className="form-actions">
          <button
            type="submit"
            className="btn btn-primary"
            disabled={isSubmitting || saving}
          >
            {isSubmitting || saving ? 'Salvando…' : 'Salvar'}
          </button>
          <Link to="/" className="btn btn-secondary">
            Cancelar
          </Link>
        </div>
      </form>
    </section>
  )
}

export default ProductForm
