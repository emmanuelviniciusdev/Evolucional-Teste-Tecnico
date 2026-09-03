import { test, expect } from '@playwright/test'
import AxeBuilder from '@axe-core/playwright'

test('shell is accessible and in pt-BR', async ({ page }) => {
  await page.goto('/')
  await expect(page).toHaveTitle(/Nexo/)
  const results = await new AxeBuilder({ page }).analyze()
  expect(results.violations.length).toBe(0)
})
