import { test, expect } from '@playwright/test'

const timestamp = () => Date.now()

test.describe('Delete product', () => {
  let targetId: number
  let targetName: string

  test.beforeEach(async ({ request }) => {
    targetName = `E2E Delete ${timestamp()}`
    const res = await request.post('http://127.0.0.1:3001/produtos', {
      data: { nome: targetName, categoria: 'Acessorios', preco: 30, estoque: 1, ativo: true },
    })
    const created = await res.json() as { id: number }
    targetId = created.id
  })

  test.afterEach(async ({ request }) => {
    // Safety clean-up in case test didn't delete it
    await request.delete(`http://127.0.0.1:3001/produtos/${targetId}`).catch(() => {})
  })

  test('confirmation dialog appears and confirming removes the product from the list', async ({ page }) => {
    await page.goto('/')
    // Search for the test product to make sure it's visible
    await page.getByRole('searchbox').fill(targetName)
    await page.waitForTimeout(600)
    await expect(page.getByText(targetName)).toBeVisible({ timeout: 5000 })

    // Click the delete button for the test product
    await page.getByRole('button', { name: `Excluir ${targetName}` }).click()

    // Confirmation dialog must be visible
    await expect(page.getByText('Tem certeza que deseja excluir este produto?')).toBeVisible({ timeout: 3000 })

    // Confirm deletion
    await page.locator('.confirm-dialog').getByRole('button', { name: 'Excluir', exact: true }).click()

    // Product is gone from the list
    await expect(page.getByText(targetName)).not.toBeVisible({ timeout: 5000 })
    targetId = -1 // already deleted
  })

  test('cancelling the dialog leaves the product in the list', async ({ page }) => {
    await page.goto('/')
    await page.getByRole('searchbox').fill(targetName)
    await page.waitForTimeout(600)
    await expect(page.getByText(targetName)).toBeVisible({ timeout: 5000 })

    await page.getByRole('button', { name: `Excluir ${targetName}` }).click()

    // Dialog appears
    await expect(page.getByText('Tem certeza que deseja excluir este produto?')).toBeVisible({ timeout: 3000 })

    // Cancel
    await page.locator('.confirm-dialog').getByText('Cancelar').click()

    // Product still visible
    await expect(page.getByText(targetName)).toBeVisible({ timeout: 3000 })
  })
})
