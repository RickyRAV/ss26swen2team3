import { useMutation } from '@tanstack/react-query'
import { useNavigate } from '@tanstack/react-router'
import { authApi } from '@/api/auth'
import { useAuthStore } from '@/stores/authStore'
import type { RegisterPayload, LoginPayload } from '@/api/auth'

export function useAuthViewModel() {
  const { setAuth, clearAuth, isAuthenticated, user } = useAuthStore()
  const navigate = useNavigate()

  const registerMutation = useMutation({
    mutationFn: (payload: RegisterPayload) => authApi.register(payload),
    onSuccess: (data) => {
      setAuth(data.accessToken, { userId: data.userId, email: data.email, userName: data.userName })
      navigate({ to: '/tours' })
    },
  })

  const loginMutation = useMutation({
    mutationFn: (payload: LoginPayload) => authApi.login(payload),
    onSuccess: (data) => {
      setAuth(data.accessToken, { userId: data.userId, email: data.email, userName: data.userName })
      navigate({ to: '/tours' })
    },
  })

  const logoutMutation = useMutation({
    mutationFn: () => authApi.logout(),
    onSettled: () => {
      clearAuth()
      navigate({ to: '/login' })
    },
  })

  return {
    isAuthenticated,
    user,
    register: registerMutation.mutate,
    login: loginMutation.mutate,
    logout: logoutMutation.mutate,
    isLoggingIn: loginMutation.isPending,
    isRegistering: registerMutation.isPending,
    loginError: loginMutation.error?.message,
    registerError: registerMutation.error?.message,
  }
}
