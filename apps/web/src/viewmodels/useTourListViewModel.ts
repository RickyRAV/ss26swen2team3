import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { toursApi } from '@/api/tours'
import type { Tour } from '@/models/tour'

export const TOURS_QUERY_KEY = 'tours'

export function useTourListViewModel() {
  const queryClient = useQueryClient()
  const [search, setSearch] = useState('')
  const [selectedTourId, setSelectedTourId] = useState<string | null>(null)

  const { data: tours = [], isLoading, error } = useQuery({
    queryKey: [TOURS_QUERY_KEY, search],
    queryFn: () => toursApi.getAll(search || undefined),
  })

  const deleteMutation = useMutation({
    mutationFn: (id: string) => toursApi.delete(id),
    onSuccess: (_, id) => {
      queryClient.invalidateQueries({ queryKey: [TOURS_QUERY_KEY] })
      if (selectedTourId === id) setSelectedTourId(null)
    },
  })

  const selectedTour: Tour | undefined = tours.find(t => t.id === selectedTourId)

  return {
    tours,
    isLoading,
    error,
    search,
    setSearch,
    selectedTourId,
    selectedTour,
    setSelectedTourId,
    deleteTour: deleteMutation.mutate,
    isDeletingTour: deleteMutation.isPending,
    deleteError: deleteMutation.error?.message,
  }
}
