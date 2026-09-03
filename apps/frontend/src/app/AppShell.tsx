import React, { useEffect } from 'react'
import { Routes, Route, Link } from 'react-router-dom'
import Home from '../pages/Home'
import ProductDetail from '../pages/ProductDetail'
import ProductForm from '../pages/ProductForm'
import NotFound from '../pages/NotFound'

const AppShell: React.FC = () => {
  useEffect(() => {
    document.documentElement.lang = 'pt-BR'
  }, [])

  return (
    <>
      <a href="#main" className="skip-link">Ir para o conteúdo</a>
      <header className="site-header" role="banner">
        <div className="container">
          <h1>Nexo — Catálogo</h1>
          <nav aria-label="Principal">
            <Link to="/">Catálogo</Link>
          </nav>
        </div>
      </header>
      <main id="main" role="main" className="container">
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/produtos/novo" element={<ProductForm />} />
          <Route path="/produtos/:id" element={<ProductDetail />} />
          <Route path="/produtos/:id/editar" element={<ProductForm />} />
          <Route path="*" element={<NotFound />} />
        </Routes>
      </main>
      <footer className="site-footer container" role="contentinfo">
        <small>Nexo — Gestão de Produtos</small>
      </footer>
    </>
  )
}

export default AppShell
