import { useForm } from '@tanstack/react-form'
import { z } from 'zod'
import { useTourLogFormViewModel } from '@/viewmodels/useTourLogFormViewModel'
import {
  Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter,
} from '@/components/ui/dialog'
import {
  Select, SelectContent, SelectItem, SelectTrigger, SelectValue,
} from '@/components/ui/select'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import type { TourLog, Difficulty, Rating } from '@/models/tourLog'
import { DIFFICULTIES, RATINGS, RATING_TO_NUMBER } from '@/models/tourLog'

const logSchema = z.object({
  dateTime: z.string().min(1, 'Date is required'),
  comment: z.string().max(2000),
  difficulty: z.enum(['Easy', 'Medium', 'Hard'] as const),
  totalDistanceKm: z.number().min(0, 'Distance must be ≥ 0'),
  totalTimeSeconds: z.number().min(0, 'Time must be ≥ 0'),
  rating: z.enum(['One', 'Two', 'Three', 'Four', 'Five'] as const),
})

interface TourLogFormDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  tourId: string
  log: TourLog | null
  formKey?: number
}

export function TourLogFormDialog({ open, onOpenChange, tourId, log, formKey }: TourLogFormDialogProps) {
  const isEditing = !!log
  const { createLog, updateLog, isCreating, isUpdating, createError, updateError } =
    useTourLogFormViewModel(tourId, log?.id)

  const toLocalDateTime = (iso?: string) => {
    if (!iso) return new Date().toISOString().slice(0, 16)
    return new Date(iso).toISOString().slice(0, 16)
  }

  const form = useForm({
    defaultValues: {
      dateTime: toLocalDateTime(log?.dateTime),
      comment: log?.comment ?? '',
      difficulty: (log?.difficulty ?? 'Medium') as Difficulty,
      totalDistanceKm: log?.totalDistanceKm ?? 0,
      totalTimeSeconds: log?.totalTimeSeconds ?? 0,
      rating: (log?.rating ?? 'Three') as Rating,
    },
    onSubmit: async ({ value }) => {
      try {
        const payload = {
          ...value,
          dateTime: new Date(value.dateTime).toISOString(),
        }
        if (isEditing) {
          await updateLog(payload)
        } else {
          await createLog(payload)
          form.reset()
        }
        onOpenChange(false)
      } catch { /* mutateAsync throws on error; mutation state surfaces it in UI */ }
    },
  })

  const error = createError ?? updateError
  const isPending = isCreating || isUpdating

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent key={log?.id ?? `new-${formKey}`}>
        <DialogHeader>
          <DialogTitle>{isEditing ? 'Edit Tour Log' : 'Add Tour Log'}</DialogTitle>
        </DialogHeader>

        <form
          onSubmit={(e) => { e.preventDefault(); e.stopPropagation(); form.handleSubmit() }}
          className="space-y-4 mt-2"
        >
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
            <form.Field name="dateTime">
              {(field) => (
                <div className="space-y-1.5 col-span-2 sm:col-span-1">
                  <Label>Date & Time <span className="text-red-500">*</span></Label>
                  <Input
                    type="datetime-local"
                    value={field.state.value}
                    onBlur={field.handleBlur}
                    onChange={(e) => field.handleChange(e.target.value)}
                  />
                </div>
              )}
            </form.Field>

            <form.Field name="difficulty">
              {(field) => (
                <div className="space-y-1.5">
                  <Label>Difficulty</Label>
                  <Select value={field.state.value} onValueChange={(v) => field.handleChange(v as Difficulty)}>
                    <SelectTrigger><SelectValue /></SelectTrigger>
                    <SelectContent>
                      {DIFFICULTIES.map(d => <SelectItem key={d} value={d}>{d}</SelectItem>)}
                    </SelectContent>
                  </Select>
                </div>
              )}
            </form.Field>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
            <form.Field
              name="totalDistanceKm"
              validators={{ onChange: ({ value }) => {
                const r = logSchema.shape.totalDistanceKm.safeParse(value)
                return r.success ? undefined : r.error.issues[0].message
              }}}
            >
              {(field) => (
                <div className="space-y-1.5">
                  <Label>Distance (km)</Label>
                  <Input
                    type="number"
                    min={0}
                    step={0.1}
                    value={field.state.value}
                    onBlur={field.handleBlur}
                    onChange={(e) => field.handleChange(parseFloat(e.target.value) || 0)}
                  />
                  {field.state.meta.errors?.[0] && (
                    <p className="text-xs text-red-600">{field.state.meta.errors[0]}</p>
                  )}
                </div>
              )}
            </form.Field>

            <form.Field
              name="totalTimeSeconds"
              validators={{ onChange: ({ value }) => {
                const r = logSchema.shape.totalTimeSeconds.safeParse(value)
                return r.success ? undefined : r.error.issues[0].message
              }}}
            >
              {(field) => (
                <div className="space-y-1.5">
                  <Label>Duration (minutes)</Label>
                  <Input
                    type="number"
                    min={0}
                    value={Math.round(field.state.value / 60)}
                    onBlur={field.handleBlur}
                    onChange={(e) => field.handleChange((parseFloat(e.target.value) || 0) * 60)}
                  />
                </div>
              )}
            </form.Field>
          </div>

          <form.Field name="rating">
            {(field) => (
              <div className="space-y-1.5">
                <Label>Rating</Label>
                <div className="flex gap-2">
                  {RATINGS.map((r) => {
                    const n = RATING_TO_NUMBER[r]
                    const selected = field.state.value === r
                    return (
                      <button
                        key={r}
                        type="button"
                        onClick={() => field.handleChange(r)}
                        className={`flex-1 py-1.5 rounded-md border text-xs font-medium transition-colors ${
                          selected
                            ? 'bg-trail-500 border-trail-500 text-white'
                            : 'border-stone-200 text-stone-600 hover:border-trail-300 hover:bg-trail-50'
                        }`}
                      >
                        {'★'.repeat(n)}
                      </button>
                    )
                  })}
                </div>
              </div>
            )}
          </form.Field>

          <form.Field name="comment">
            {(field) => (
              <div className="space-y-1.5">
                <Label>Comment</Label>
                <textarea
                  className="w-full min-h-[72px] rounded-md border border-stone-200 bg-white px-3 py-2 text-sm shadow-sm resize-none focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-forest-500 placeholder:text-stone-400"
                  placeholder="How did it go?"
                  value={field.state.value}
                  onBlur={field.handleBlur}
                  onChange={(e) => field.handleChange(e.target.value)}
                  maxLength={2000}
                />
              </div>
            )}
          </form.Field>

          {error && (
            <p className="text-sm text-red-600 bg-red-50 border border-red-200 rounded-md px-3 py-2">
              {error}
            </p>
          )}

          <DialogFooter>
            <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>Cancel</Button>
            <Button type="submit" disabled={isPending}>
              {isPending ? 'Saving…' : isEditing ? 'Save changes' : 'Add log'}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  )
}
