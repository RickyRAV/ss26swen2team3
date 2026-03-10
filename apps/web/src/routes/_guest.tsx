import { createFileRoute, redirect } from '@tanstack/react-router'
import { GuestLayout } from '@/views/layouts/AppLayout'
import { useAuthStore } from '@/stores/authStore'

export const Route = createFileRoute('/_guest')({
  beforeLoad: () => {
    const isAuthenticated = useAuthStore.getState().isAuthenticated
    if (isAuthenticated) {
      throw redirect({ to: '/tours' })
    }
  },
  component: GuestLayout,
})
