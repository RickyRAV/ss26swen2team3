import { useForm } from '@tanstack/react-form'
import { z } from 'zod'
import { Link } from '@tanstack/react-router'
import { useAuthViewModel } from '@/viewmodels/useAuthViewModel'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card'

const loginSchema = z.object({
  email: z.string().email('Enter a valid email address'),
  password: z.string().min(1, 'Password is required'),
})

export function LoginView() {
  const { login, isLoggingIn, loginError } = useAuthViewModel()

  const form = useForm({
    defaultValues: { email: '', password: '' },
    onSubmit: async ({ value }) => {
      login(value)
    },
  })

  return (
    <Card className="shadow-xl border-0">
      <CardHeader>
        <CardTitle>Welcome back</CardTitle>
        <CardDescription>Sign in to your account</CardDescription>
      </CardHeader>
      <CardContent>
        <form
          onSubmit={(e) => {
            e.preventDefault()
            e.stopPropagation()
            form.handleSubmit()
          }}
          className="space-y-4"
        >
          <form.Field
            name="email"
            validators={{ onChange: ({ value }) => {
              const result = loginSchema.shape.email.safeParse(value)
              return result.success ? undefined : result.error.issues[0].message
            }}}
          >
            {(field) => (
              <div className="space-y-1.5">
                <Label htmlFor={field.name}>Email</Label>
                <Input
                  id={field.name}
                  type="email"
                  placeholder="you@example.com"
                  value={field.state.value}
                  onBlur={field.handleBlur}
                  onChange={(e) => field.handleChange(e.target.value)}
                  autoComplete="email"
                />
                {field.state.meta.errors?.[0] && (
                  <p className="text-xs text-red-600">{field.state.meta.errors[0]}</p>
                )}
              </div>
            )}
          </form.Field>

          <form.Field
            name="password"
            validators={{ onChange: ({ value }) => {
              const result = loginSchema.shape.password.safeParse(value)
              return result.success ? undefined : result.error.issues[0].message
            }}}
          >
            {(field) => (
              <div className="space-y-1.5">
                <Label htmlFor={field.name}>Password</Label>
                <Input
                  id={field.name}
                  type="password"
                  placeholder="••••••••"
                  value={field.state.value}
                  onBlur={field.handleBlur}
                  onChange={(e) => field.handleChange(e.target.value)}
                  autoComplete="current-password"
                />
                {field.state.meta.errors?.[0] && (
                  <p className="text-xs text-red-600">{field.state.meta.errors[0]}</p>
                )}
              </div>
            )}
          </form.Field>

          {loginError && (
            <p className="text-sm text-red-600 bg-red-50 border border-red-200 rounded-md px-3 py-2">
              {loginError}
            </p>
          )}

          <Button type="submit" className="w-full" disabled={isLoggingIn}>
            {isLoggingIn ? 'Signing in…' : 'Sign in'}
          </Button>

          <p className="text-center text-sm text-stone-500">
            Don&apos;t have an account?{' '}
            <Link to="/register" className="text-forest-600 hover:underline font-medium">
              Register
            </Link>
          </p>
        </form>
      </CardContent>
    </Card>
  )
}
