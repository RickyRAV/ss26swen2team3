import { createFileRoute, redirect } from '@tanstack/react-router'
import { AppLayout } from '@/views/layouts/AppLayout'
import { useAuthStore } from '@/stores/authStore'

export const Route = createFileRoute('/_auth')({
  beforeLoad: () => {
    const isAuthenticated = useAuthStore.getState().isAuthenticated
    if (!isAuthenticated) {
      throw redirect({ to: '/login' })
    }
  },
  component: AppLayout,
})
