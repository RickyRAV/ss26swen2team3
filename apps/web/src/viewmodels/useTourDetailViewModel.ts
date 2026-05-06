import { useMemo } from 'react'
import { useQuery } from '@tanstack/react-query'
import { toursApi } from '@/api/tours'
import { parseRoute } from '@/models/tour'
import { TOURS_QUERY_KEY } from './useTourListViewModel'

export function useTourDetailViewModel(tourId: string | null) {
  const { data: tour, isLoading, error } = useQuery({
    queryKey: [TOURS_QUERY_KEY, 'detail', tourId],
    queryFn: () => toursApi.getById(tourId!),
    enabled: !!tourId,
  })

  // Map + elevation render from the route ORS computed and stored at create time.
  const route = useMemo(() => parseRoute(tour?.routeInformation), [tour?.routeInformation])

  return { tour, route, isLoading, error }
}
