import React from 'react'
import { createRoot } from 'react-dom/client'
import { BrowserRouter } from 'react-router-dom'
import AppShell from './app/AppShell'
import './styles.css'

const root = createRoot(document.getElementById('root')!)

root.render(
  <React.StrictMode>
    <BrowserRouter>
      <AppShell />
    </BrowserRouter>
  </React.StrictMode>
)

