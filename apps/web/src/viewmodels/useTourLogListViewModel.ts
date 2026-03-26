import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { tourLogsApi } from '@/api/tourLogs'

export const TOUR_LOGS_QUERY_KEY = 'tourLogs'

export function useTourLogListViewModel(tourId: string | null) {
  const queryClient = useQueryClient()

  const { data: logs = [], isLoading, error } = useQuery({
    queryKey: [TOUR_LOGS_QUERY_KEY, tourId],
    queryFn: () => tourLogsApi.getByTourId(tourId!),
    enabled: !!tourId,
  })

  const deleteMutation = useMutation({
    mutationFn: ({ id }: { id: string }) => tourLogsApi.delete(tourId!, id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [TOUR_LOGS_QUERY_KEY, tourId] })
      // Also invalidate tour list to refresh popularity/child-friendliness
      queryClient.invalidateQueries({ queryKey: ['tours'] })
    },
  })

  return {
    logs,
    isLoading,
    error,
    deleteLog: deleteMutation.mutate,
    isDeletingLog: deleteMutation.isPending,
  }
}
