import { useRef, useState } from 'react'
import { useForm } from '@tanstack/react-form'
import { z } from 'zod'
import { ImagePlus, X } from 'lucide-react'
import { useTourFormViewModel } from '@/viewmodels/useTourFormViewModel'
import {
  Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter,
} from '@/components/ui/dialog'
import {
  Select, SelectContent, SelectItem, SelectTrigger, SelectValue,
} from '@/components/ui/select'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import type { Tour, TransportType } from '@/models/tour'
import { TRANSPORT_TYPES, TRANSPORT_ICONS } from '@/models/tour'

const tourSchema = z.object({
  name: z.string().min(1, 'Name is required').max(200),
  description: z.string().max(2000),
  from: z.string().min(1, 'Origin is required').max(300),
  to: z.string().min(1, 'Destination is required').max(300),
  transportType: z.enum(['Car', 'Bicycle', 'Hiking', 'Running', 'Vacation'] as const),
})

interface TourFormDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  tour: Tour | null
  formKey?: number
}

export function TourFormDialog({ open, onOpenChange, tour, formKey }: TourFormDialogProps) {
  const isEditing = !!tour
  const { createTour, updateTour, uploadImage, isCreating, isUpdating, isUploadingImage, createError, updateError, uploadImageError } =
    useTourFormViewModel(tour?.id)

  const [imageFile, setImageFile] = useState<File | null>(null)
  const [imagePreview, setImagePreview] = useState<string | null>(null)
  const fileInputRef = useRef<HTMLInputElement>(null)

  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0]
    if (!file) return
    setImageFile(file)
    setImagePreview(URL.createObjectURL(file))
  }

  const clearImage = () => {
    setImageFile(null)
    setImagePreview(null)
    if (fileInputRef.current) fileInputRef.current.value = ''
  }

  const form = useForm({
    defaultValues: {
      name: tour?.name ?? '',
      description: tour?.description ?? '',
      from: tour?.from ?? '',
      to: tour?.to ?? '',
      transportType: (tour?.transportType ?? 'Hiking') as TransportType,
    },
    onSubmit: async ({ value }) => {
      try {
        let savedTour: Tour
        if (isEditing) {
          savedTour = await updateTour({
            ...value,
            distance: tour.distance,
            estimatedTimeSeconds: tour.estimatedTimeSeconds,
            routeInformation: tour.routeInformation,
          })
        } else {
          savedTour = await createTour(value)
        }
        if (imageFile) {
          await uploadImage({ id: savedTour.id, file: imageFile })
        }
        clearImage()
        form.reset()
        onOpenChange(false)
      } catch { /* mutateAsync throws on error; mutation state surfaces it in UI */ }
    },
  })

  const error = createError ?? updateError ?? uploadImageError
  const isPending = isCreating || isUpdating || isUploadingImage

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent key={tour?.id ?? `new-${formKey}`}>
        <DialogHeader>
          <DialogTitle>{isEditing ? 'Edit Tour' : 'Create New Tour'}</DialogTitle>
        </DialogHeader>

        <form
          onSubmit={(e) => {
            e.preventDefault()
            e.stopPropagation()
            form.handleSubmit()
          }}
          className="space-y-4 mt-2"
        >
          <form.Field
            name="name"
            validators={{ onChange: ({ value }) => {
              const r = tourSchema.shape.name.safeParse(value)
              return r.success ? undefined : r.error.issues[0].message
            }}}
          >
            {(field) => (
              <div className="space-y-1.5">
                <Label>Tour name <span className="text-red-500">*</span></Label>
                <Input
                  placeholder="Morning Bike Ride"
                  value={field.state.value}
                  onBlur={field.handleBlur}
                  onChange={(e) => field.handleChange(e.target.value)}
                />
                {field.state.meta.errors?.[0] && (
                  <p className="text-xs text-red-600">{field.state.meta.errors[0]}</p>
                )}
              </div>
            )}
          </form.Field>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
            <form.Field
              name="from"
              validators={{ onChange: ({ value }) => {
                const r = tourSchema.shape.from.safeParse(value)
                return r.success ? undefined : r.error.issues[0].message
              }}}
            >
              {(field) => (
                <div className="space-y-1.5">
                  <Label>From <span className="text-red-500">*</span></Label>
                  <Input
                    placeholder="Vienna"
                    value={field.state.value}
                    onBlur={field.handleBlur}
                    onChange={(e) => field.handleChange(e.target.value)}
                  />
                  {field.state.meta.errors?.[0] && (
                    <p className="text-xs text-red-600">{field.state.meta.errors[0]}</p>
                  )}
                </div>
              )}
            </form.Field>

            <form.Field
              name="to"
              validators={{ onChange: ({ value }) => {
                const r = tourSchema.shape.to.safeParse(value)
                return r.success ? undefined : r.error.issues[0].message
              }}}
            >
              {(field) => (
                <div className="space-y-1.5">
                  <Label>To <span className="text-red-500">*</span></Label>
                  <Input
                    placeholder="Graz"
                    value={field.state.value}
                    onBlur={field.handleBlur}
                    onChange={(e) => field.handleChange(e.target.value)}
                  />
                  {field.state.meta.errors?.[0] && (
                    <p className="text-xs text-red-600">{field.state.meta.errors[0]}</p>
                  )}
                </div>
              )}
            </form.Field>
          </div>

          <form.Field name="transportType">
            {(field) => (
              <div className="space-y-1.5">
                <Label>Transport type</Label>
                <Select value={field.state.value} onValueChange={(v) => field.handleChange(v as TransportType)}>
                  <SelectTrigger>
                    <SelectValue placeholder="Select type" />
                  </SelectTrigger>
                  <SelectContent>
                    {TRANSPORT_TYPES.map((type) => (
                      <SelectItem key={type} value={type}>
                        {TRANSPORT_ICONS[type]} {type}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
            )}
          </form.Field>

          <form.Field name="description">
            {(field) => (
              <div className="space-y-1.5">
                <Label>Description</Label>
                <textarea
                  className="w-full min-h-[80px] rounded-md border border-stone-200 bg-white px-3 py-2 text-sm shadow-sm resize-none focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-forest-500 placeholder:text-stone-400"
                  placeholder="A scenic route through the hills…"
                  value={field.state.value}
                  onBlur={field.handleBlur}
                  onChange={(e) => field.handleChange(e.target.value)}
                  maxLength={2000}
                />
              </div>
            )}
          </form.Field>

          {/* Image upload */}
          <div className="space-y-1.5">
            <Label>Tour Image</Label>
            {imagePreview ? (
              <div className="relative">
                <img
                  src={imagePreview}
                  alt="Preview"
                  className="w-full h-32 object-cover rounded-md border border-stone-200"
                />
                <button
                  type="button"
                  onClick={clearImage}
                  className="absolute top-1.5 right-1.5 p-1 rounded-full bg-white/80 hover:bg-white border border-stone-200 text-stone-600"
                >
                  <X className="h-3.5 w-3.5" />
                </button>
              </div>
            ) : (
              <button
                type="button"
                onClick={() => fileInputRef.current?.click()}
                className="flex w-full items-center gap-2 rounded-md border border-dashed border-stone-300 bg-stone-50 px-3 py-3 text-sm text-stone-500 hover:border-stone-400 hover:bg-stone-100 transition-colors"
              >
                <ImagePlus className="h-4 w-4" />
                <span>{isEditing && tour.imagePath ? 'Replace image' : 'Upload image'}</span>
              </button>
            )}
            <input
              ref={fileInputRef}
              type="file"
              accept="image/jpeg,image/png,image/webp"
              className="hidden"
              onChange={handleImageChange}
            />
          </div>

          {error && (
            <p className="text-sm text-red-600 bg-red-50 border border-red-200 rounded-md px-3 py-2">
              {error}
            </p>
          )}

          <DialogFooter>
            <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
              Cancel
            </Button>
            <Button type="submit" disabled={isPending}>
              {isPending ? 'Saving…' : isEditing ? 'Save changes' : 'Create tour'}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  )
}
