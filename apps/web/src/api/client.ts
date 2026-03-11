import { useAuthStore } from '@/stores/authStore'

const BASE_URL = (import.meta.env.VITE_API_BASE_URL ?? '').replace(/\/$/, '')

export class ApiError extends Error {
  readonly status: number
  readonly detail: string

  constructor(status: number, detail: string, message?: string) {
    super(message ?? detail)
    this.status = status
    this.detail = detail
  }
}

async function parseError(res: Response): Promise<ApiError> {
  try {
    const body = await res.json()
    return new ApiError(res.status, body.detail ?? body.title ?? res.statusText)
  } catch {
    return new ApiError(res.status, res.statusText)
  }
}

export async function apiFetch<T>(
  path: string,
  options: RequestInit = {},
): Promise<T> {
  const token = useAuthStore.getState().accessToken
  const headers: HeadersInit = {
    'Content-Type': 'application/json',
    ...(options.headers as Record<string, string>),
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
  }

  const res = await fetch(`${BASE_URL}${path}`, { ...options, headers, credentials: 'include' })

  if (res.status === 401) {
    // Try refreshing
    const refreshed = await tryRefresh()
    if (refreshed) {
      const newToken = useAuthStore.getState().accessToken
      const retryHeaders: HeadersInit = {
        'Content-Type': 'application/json',
        ...(newToken ? { Authorization: `Bearer ${newToken}` } : {}),
      }
      const retryRes = await fetch(`${BASE_URL}${path}`, { ...options, headers: retryHeaders, credentials: 'include' })
      if (!retryRes.ok) throw await parseError(retryRes)
      if (retryRes.status === 204) return undefined as T
      return retryRes.json()
    } else {
      useAuthStore.getState().clearAuth()
      throw new ApiError(401, 'Session expired. Please log in again.')
    }
  }

  if (!res.ok) throw await parseError(res)
  if (res.status === 204) return undefined as T
  return res.json()
}

export async function apiUpload<T>(path: string, body: FormData): Promise<T> {
  const token = useAuthStore.getState().accessToken
  const headers: HeadersInit = token ? { Authorization: `Bearer ${token}` } : {}

  const res = await fetch(`${BASE_URL}${path}`, {
    method: 'POST',
    headers,
    body,
    credentials: 'include',
  })

  if (!res.ok) throw await parseError(res)
  return res.json()
}

let refreshing: Promise<boolean> | null = null

async function tryRefresh(): Promise<boolean> {
  if (refreshing) return refreshing

  refreshing = (async () => {
    try {
      const res = await fetch(`${BASE_URL}/api/v1/auth/refresh`, {
        method: 'POST',
        credentials: 'include',
      })
      if (!res.ok) return false
      const data = await res.json()
      useAuthStore.getState().setAuth(data.accessToken, {
        userId: data.userId,
        email: data.email,
        userName: data.userName,
      })
      return true
    } catch {
      return false
    } finally {
      refreshing = null
    }
  })()

  return refreshing
}
