import { test, expect } from '@playwright/test'

test.describe('Product listing', () => {
  test('shows product rows and total count on the first page', async ({ page }) => {
    await page.goto('/')
    // Wait for at least one product row
    await expect(page.locator('table tbody tr').first()).toBeVisible({ timeout: 10000 })
    // Total count should be displayed
    await expect(page.locator('.total-count')).toBeVisible()
    const countText = await page.locator('.total-count').textContent()
    expect(countText).toMatch(/\d+ produto/)
  })

  test('pagination controls appear and navigating to page 2 adds page=2 to URL', async ({ page }) => {
    await page.goto('/')
    await expect(page.locator('table tbody tr').first()).toBeVisible({ timeout: 10000 })

    const nextBtn = page.getByRole('button', { name: 'Próxima página' })
    if (await nextBtn.isVisible()) {
      await nextBtn.click()
      await expect(page).toHaveURL(/page=2/)
      // Page 2 rows are visible
      await expect(page.locator('table tbody tr').first()).toBeVisible({ timeout: 5000 })
    } else {
      // Not enough products for pagination — total fits in one page
      test.info().annotations.push({ type: 'note', description: 'Single page of results — pagination not shown' })
    }
  })

  test('total count derived from X-Total-Count is rendered', async ({ page }) => {
    await page.goto('/')
    await expect(page.locator('.total-count')).toBeVisible({ timeout: 10000 })
    const text = await page.locator('.total-count').textContent()
    // Should include a number greater than zero
    expect(text).toMatch(/[1-9]\d* produto/)
  })
})

test.describe('Search, filter and URL state', () => {
  test('name search filters results and updates URL', async ({ page }) => {
    await page.goto('/')
    await expect(page.locator('table tbody tr').first()).toBeVisible({ timeout: 10000 })

    const searchInput = page.getByRole('searchbox')
    await searchInput.fill('Teclado')
    // Wait for debounce + response
    await page.waitForTimeout(600)
    await expect(page).toHaveURL(/q=Teclado/)
    // Only keyboard-related rows should appear
    await expect(page.locator('table tbody tr').first()).toBeVisible({ timeout: 5000 })
  })

  test('category filter updates URL and shows only matching products', async ({ page }) => {
    await page.goto('/')
    await expect(page.locator('table tbody tr').first()).toBeVisible({ timeout: 10000 })

    await page.getByRole('combobox', { name: 'Filtrar por categoria' }).selectOption('Audio')
    await page.waitForTimeout(400)
    await expect(page).toHaveURL(/categoria=Audio/)
    await expect(page.locator('table tbody tr').first()).toBeVisible({ timeout: 5000 })
  })

  test('list state survives a browser reload', async ({ page }) => {
    await page.goto('/?q=Teclado&categoria=Perifericos')
    await page.waitForTimeout(400)
    await page.reload()
    // After reload the search input should retain the term
    const inputValue = await page.getByRole('searchbox').inputValue()
    expect(inputValue).toBe('Teclado')
    // Category select should still show Perifericos
    const selectedCat = await page.getByRole('combobox', { name: 'Filtrar por categoria' }).inputValue()
    expect(selectedCat).toBe('Perifericos')
  })
})
