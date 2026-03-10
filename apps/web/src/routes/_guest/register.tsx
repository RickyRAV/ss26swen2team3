import { createFileRoute } from '@tanstack/react-router'
import { RegisterView } from '@/views/auth/RegisterView'

export const Route = createFileRoute('/_guest/register')({
  component: RegisterView,
})
