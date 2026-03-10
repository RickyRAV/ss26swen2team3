import { createRootRoute, Outlet } from '@tanstack/react-router'
import { useAuthStore } from '@/stores/authStore'

/**
 * On every cold load (page refresh / direct URL navigation), attempt a silent
 * token refresh using the httpOnly refresh-token cookie.  If the cookie is
 * valid the user stays logged in; if it is missing or expired they land on the
 * login page as normal.
 *
 * This runs before any child route's beforeLoad, so _auth.tsx sees the correct
 * isAuthenticated value when it checks whether to redirect.
 */
async function bootstrapAuth() {
  if (useAuthStore.getState().isAuthenticated) return

  try {
    const base = (import.meta.env.VITE_API_BASE_URL ?? '').replace(/\/$/, '')
    const res = await fetch(`${base}/api/v1/auth/refresh`, {
      method: 'POST',
      credentials: 'include',
    })
    if (!res.ok) return

    const data = await res.json()
    useAuthStore.getState().setAuth(data.accessToken, {
      userId: data.userId,
      email: data.email,
      userName: data.userName,
    })
  } catch {
    // No valid session — user will be redirected to login by _auth.tsx
  }
}

export const Route = createRootRoute({
  beforeLoad: bootstrapAuth,
  component: () => <Outlet />,
})
