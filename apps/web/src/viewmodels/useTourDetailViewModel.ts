import { useQuery } from '@tanstack/react-query'
import { toursApi } from '@/api/tours'
import { TOURS_QUERY_KEY } from './useTourListViewModel'

export function useTourDetailViewModel(tourId: string | null) {
  const { data: tour, isLoading, error } = useQuery({
    queryKey: [TOURS_QUERY_KEY, 'detail', tourId],
    queryFn: () => toursApi.getById(tourId!),
    enabled: !!tourId,
  })

  return { tour, isLoading, error }
}
