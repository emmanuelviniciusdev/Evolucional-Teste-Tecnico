import { renderHook, act } from '@testing-library/react'
import { useDebounce } from '../useDebounce'

beforeEach(() => vi.useFakeTimers())
afterEach(() => vi.useRealTimers())

test('returns the initial value immediately', () => {
  const { result } = renderHook(() => useDebounce('hello', 300))
  expect(result.current).toBe('hello')
})

test('does not update before the delay elapses', () => {
  const { result, rerender } = renderHook(({ v }) => useDebounce(v, 300), {
    initialProps: { v: 'first' },
  })
  rerender({ v: 'second' })
  act(() => { vi.advanceTimersByTime(100) })
  expect(result.current).toBe('first')
})

test('updates after the delay elapses', () => {
  const { result, rerender } = renderHook(({ v }) => useDebounce(v, 300), {
    initialProps: { v: 'first' },
  })
  rerender({ v: 'second' })
  act(() => { vi.advanceTimersByTime(300) })
  expect(result.current).toBe('second')
})

test('cancels intermediate values — only the last one is emitted', () => {
  const { result, rerender } = renderHook(({ v }) => useDebounce(v, 300), {
    initialProps: { v: 'a' },
  })
  rerender({ v: 'b' })
  act(() => { vi.advanceTimersByTime(100) })
  rerender({ v: 'c' })
  act(() => { vi.advanceTimersByTime(300) })
  expect(result.current).toBe('c')
})
