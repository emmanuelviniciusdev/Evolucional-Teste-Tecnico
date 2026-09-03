import { test, expect } from '@playwright/test'

test.describe('Product detail', () => {
  test('clicking a product row navigates to /produtos/:id', async ({ page }) => {
    await page.goto('/')
    await expect(page.locator('table tbody tr').first()).toBeVisible({ timeout: 10000 })

    // Click the first product link in the table
    await page.locator('table tbody tr').first().getByRole('link').first().click()
    await expect(page).toHaveURL(/\/produtos\/\d+$/)
  })

  test('detail screen shows all product fields', async ({ page }) => {
    await page.goto('/produtos/1')
    // Wait for loading to complete (spinner disappears or dl appears)
    await expect(page.locator('.product-detail')).toBeVisible({ timeout: 10000 })

    // All fields should be visible
    await expect(page.locator('.product-detail h2')).toBeVisible()
    await expect(page.locator('dt', { hasText: 'ID' })).toBeVisible()
    await expect(page.locator('dt', { hasText: 'Categoria' })).toBeVisible()
    await expect(page.locator('dt', { hasText: 'Preço' })).toBeVisible()
    await expect(page.locator('dt', { hasText: 'Estoque' })).toBeVisible()
    await expect(page.locator('dt', { hasText: 'Ativo' })).toBeVisible()
  })

  test('back link navigates to /', async ({ page }) => {
    await page.goto('/produtos/1')
    await expect(page.locator('.product-detail')).toBeVisible({ timeout: 10000 })

    await page.getByText('← Voltar ao Catálogo').click()
    await expect(page).toHaveURL((url) => url.pathname === '/')
  })

  test('edit link navigates to /produtos/:id/editar', async ({ page }) => {
    await page.goto('/produtos/1')
    await expect(page.locator('.product-detail')).toBeVisible({ timeout: 10000 })

    await page.locator('.detail-nav').getByText('Editar').click()
    await expect(page).toHaveURL('/produtos/1/editar')
  })
})
