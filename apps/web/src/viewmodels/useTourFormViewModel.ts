import { useMutation, useQueryClient } from '@tanstack/react-query'
import { toursApi } from '@/api/tours'
import { TOURS_QUERY_KEY } from './useTourListViewModel'
import type { CreateTourPayload, UpdateTourPayload } from '@/models/tour'

export function useTourFormViewModel(tourId?: string) {
  const queryClient = useQueryClient()

  const createMutation = useMutation({
    mutationFn: (payload: CreateTourPayload) => toursApi.create(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [TOURS_QUERY_KEY] })
    },
  })

  const updateMutation = useMutation({
    mutationFn: (payload: UpdateTourPayload) => toursApi.update(tourId!, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [TOURS_QUERY_KEY] })
      queryClient.invalidateQueries({ queryKey: [TOURS_QUERY_KEY, 'detail', tourId] })
    },
  })

  const uploadImageMutation = useMutation({
    mutationFn: ({ id, file }: { id: string; file: File }) => toursApi.uploadImage(id, file),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [TOURS_QUERY_KEY] })
      if (tourId) queryClient.invalidateQueries({ queryKey: [TOURS_QUERY_KEY, 'detail', tourId] })
    },
  })

  return {
    createTour: createMutation.mutateAsync,
    updateTour: updateMutation.mutateAsync,
    uploadImage: uploadImageMutation.mutateAsync,
    isCreating: createMutation.isPending,
    isUpdating: updateMutation.isPending,
    isUploadingImage: uploadImageMutation.isPending,
    createError: createMutation.error?.message,
    updateError: updateMutation.error?.message,
    uploadImageError: uploadImageMutation.error?.message,
  }
}
