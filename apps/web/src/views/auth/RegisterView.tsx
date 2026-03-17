import { useForm } from '@tanstack/react-form'
import { z } from 'zod'
import { Link } from '@tanstack/react-router'
import { useAuthViewModel } from '@/viewmodels/useAuthViewModel'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card'

const registerSchema = z.object({
  email: z.string().email('Enter a valid email address'),
  userName: z
    .string()
    .min(3, 'Username must be at least 3 characters')
    .max(50)
    .regex(/^[a-zA-Z0-9_-]+$/, 'Only letters, numbers, hyphens, underscores'),
  password: z
    .string()
    .min(8, 'Password must be at least 8 characters')
    .regex(/[A-Z]/, 'Must contain an uppercase letter')
    .regex(/[0-9]/, 'Must contain a digit'),
})

export function RegisterView() {
  const { register, isRegistering, registerError } = useAuthViewModel()

  const form = useForm({
    defaultValues: { email: '', userName: '', password: '' },
    onSubmit: async ({ value }) => {
      register(value)
    },
  })

  return (
    <Card className="shadow-xl border-0">
      <CardHeader>
        <CardTitle>Create account</CardTitle>
        <CardDescription>Start planning your adventures</CardDescription>
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
          {(['email', 'userName', 'password'] as const).map((fieldName) => {
            const meta = {
              email: { label: 'Email', type: 'email', placeholder: 'you@example.com', autoComplete: 'email' },
              userName: { label: 'Username', type: 'text', placeholder: 'trailblazer42', autoComplete: 'username' },
              password: { label: 'Password', type: 'password', placeholder: '••••••••', autoComplete: 'new-password' },
            }[fieldName]

            return (
              <form.Field
                key={fieldName}
                name={fieldName}
                validators={{ onChange: ({ value }) => {
                  const result = registerSchema.shape[fieldName].safeParse(value)
                  return result.success ? undefined : result.error.issues[0].message
                }}}
              >
                {(field) => (
                  <div className="space-y-1.5">
                    <Label htmlFor={field.name}>{meta.label}</Label>
                    <Input
                      id={field.name}
                      type={meta.type}
                      placeholder={meta.placeholder}
                      value={field.state.value}
                      onBlur={field.handleBlur}
                      onChange={(e) => field.handleChange(e.target.value)}
                      autoComplete={meta.autoComplete}
                    />
                    {field.state.meta.errors?.[0] && (
                      <p className="text-xs text-red-600">{field.state.meta.errors[0]}</p>
                    )}
                  </div>
                )}
              </form.Field>
            )
          })}

          {registerError && (
            <p className="text-sm text-red-600 bg-red-50 border border-red-200 rounded-md px-3 py-2">
              {registerError}
            </p>
          )}

          <Button type="submit" className="w-full" disabled={isRegistering}>
            {isRegistering ? 'Creating account…' : 'Create account'}
          </Button>

          <p className="text-center text-sm text-stone-500">
            Already have an account?{' '}
            <Link to="/login" className="text-forest-600 hover:underline font-medium">
              Sign in
            </Link>
          </p>
        </form>
      </CardContent>
    </Card>
  )
}
