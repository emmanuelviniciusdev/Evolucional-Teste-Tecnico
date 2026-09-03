import React from 'react'
import { Routes, Route, Link } from 'react-router-dom'
import Home from '../pages/Home'
import NotFound from '../pages/NotFound'

const AppShell: React.FC = () => {
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
          <Route path="/produtos/novo" element={<Home />} />
          <Route path="/produtos/:id" element={<Home />} />
          <Route path="/produtos/:id/editar" element={<Home />} />
          <Route path="*" element={<NotFound />} />
        </Routes>
      </main>
      <footer className="site-footer container" role="contentinfo">
        <small>Fundação do app — telas serão implementadas em mudanças posteriores.</small>
      </footer>
    </>
  )
}

export default AppShell

