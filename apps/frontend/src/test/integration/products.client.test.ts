import { expect, test } from 'vitest'
import { listProducts } from '@/shared/api/products'

test('listProducts parses X-Total-Count and items', async () => {
  const res = await listProducts({ page: 1, limit: 10 })
  expect(res.total).toBeGreaterThanOrEqual(2)
  expect(res.items.length).toBeGreaterThanOrEqual(1)
})

