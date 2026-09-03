import { test, expect } from '@playwright/test'

const timestamp = () => Date.now()

test.describe('Create product', () => {
  let createdProductName: string

  test.afterEach(async ({ request }) => {
    // Clean up: delete any product created during the test
    if (createdProductName) {
      const res = await request.get('http://127.0.0.1:3001/produtos?nome_like=' + encodeURIComponent(createdProductName))
      const products = await res.json() as Array<{ id: number }>
      for (const p of products) {
        await request.delete(`http://127.0.0.1:3001/produtos/${p.id}`)
      }
    }
  })

  test('fills form, submits, shows success, and product appears in list', async ({ page }) => {
    createdProductName = `E2E Produto ${timestamp()}`

    await page.goto('/produtos/novo')
    await expect(page.getByText('Novo Produto')).toBeVisible({ timeout: 5000 })

    await page.getByLabel('Nome *').fill(createdProductName)
    await page.getByLabel('Preço *').fill('99.90')
    await page.getByLabel('Estoque *').fill('5')

    await page.getByText('Salvar').click()

    // Success banner
    await expect(page.getByText('Produto criado com sucesso!')).toBeVisible({ timeout: 5000 })

    // Wait for navigation back to list
    await expect(page).toHaveURL((url) => url.pathname === '/', { timeout: 3000 })

    // Product appears in list (search for it)
    await page.getByRole('searchbox').fill(createdProductName)
    await page.waitForTimeout(600)
    await expect(page.getByText(createdProductName)).toBeVisible({ timeout: 5000 })
  })

  test('shows field-level validation errors without submitting', async ({ page }) => {
    await page.goto('/produtos/novo')
    await expect(page.getByText('Novo Produto')).toBeVisible({ timeout: 5000 })

    // Fill invalid data
    await page.getByLabel('Nome *').fill('AB')
    await page.getByLabel('Preço *').fill('0')
    await page.getByLabel('Estoque *').fill('-1')

    await page.getByText('Salvar').click()

    await expect(page.getByText('Nome deve ter ao menos 3 caracteres')).toBeVisible({ timeout: 3000 })
    await expect(page.getByText('Preço deve ser maior que zero')).toBeVisible()
    await expect(page.getByText('Estoque não pode ser negativo')).toBeVisible()
    // URL should NOT change (no navigation)
    await expect(page).toHaveURL('/produtos/novo')
  })
})

test.describe('Edit product', () => {
  let editedProductName: string
  let targetId: number

  test.beforeEach(async ({ request }) => {
    // Create a fresh product to edit
    editedProductName = `E2E Edit ${timestamp()}`
    const res = await request.post('http://127.0.0.1:3001/produtos', {
      data: { nome: editedProductName, categoria: 'Acessorios', preco: 50, estoque: 3, ativo: true },
    })
    const created = await res.json() as { id: number }
    targetId = created.id
  })

  test.afterEach(async ({ request }) => {
    // Clean up
    await request.delete(`http://127.0.0.1:3001/produtos/${targetId}`).catch(() => {})
  })

  test('pre-populates form, edits a field, submits, and shows updated value', async ({ page }) => {
    await page.goto(`/produtos/${targetId}/editar`)
    await expect(page.getByLabel('Nome *')).toBeVisible({ timeout: 10000 })

    const input = page.getByLabel('Nome *')
    await expect(input).toHaveValue(editedProductName, { timeout: 5000 })

    const updatedName = `${editedProductName} UPDATED`
    await input.fill(updatedName)
    editedProductName = updatedName

    await page.getByText('Salvar').click()

    await expect(page.getByText('Produto atualizado com sucesso!')).toBeVisible({ timeout: 5000 })

    // Navigate to list and verify updated value
    await expect(page).toHaveURL((url) => url.pathname === '/', { timeout: 3000 })
    await page.getByRole('searchbox').fill(updatedName)
    await page.waitForTimeout(600)
    await expect(page.getByText(updatedName)).toBeVisible({ timeout: 5000 })
  })
})
