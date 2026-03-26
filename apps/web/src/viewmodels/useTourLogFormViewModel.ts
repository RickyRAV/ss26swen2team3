import { useMutation, useQueryClient } from '@tanstack/react-query'
import { tourLogsApi } from '@/api/tourLogs'
import { TOUR_LOGS_QUERY_KEY } from './useTourLogListViewModel'
import type { CreateTourLogPayload, UpdateTourLogPayload } from '@/models/tourLog'

export function useTourLogFormViewModel(tourId: string, logId?: string) {
  const queryClient = useQueryClient()

  const createMutation = useMutation({
    mutationFn: (payload: CreateTourLogPayload) => tourLogsApi.create(tourId, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [TOUR_LOGS_QUERY_KEY, tourId] })
      queryClient.invalidateQueries({ queryKey: ['tours'] })
    },
  })

  const updateMutation = useMutation({
    mutationFn: (payload: UpdateTourLogPayload) => tourLogsApi.update(tourId, logId!, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [TOUR_LOGS_QUERY_KEY, tourId] })
    },
  })

  return {
    createLog: createMutation.mutateAsync,
    updateLog: updateMutation.mutateAsync,
    isCreating: createMutation.isPending,
    isUpdating: updateMutation.isPending,
    createError: createMutation.error?.message,
    updateError: updateMutation.error?.message,
  }
}
