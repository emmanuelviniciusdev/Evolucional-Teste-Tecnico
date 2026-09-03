import { setupServer } from 'msw/node'
import { handlers, resetHandlerData } from './msw/handlers'

const server = setupServer(...handlers)

beforeAll(() => server.listen({ onUnhandledRequest: 'warn' }))
afterEach(() => {
  server.resetHandlers()
  resetHandlerData()
})
afterAll(() => server.close())
