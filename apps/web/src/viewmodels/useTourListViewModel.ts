import { useState, useEffect } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { toursApi } from '@/api/tours'
import type { Tour } from '@/models/tour'

export const TOURS_QUERY_KEY = 'tours'

export function useTourListViewModel() {
  const queryClient = useQueryClient()
  const [search, setSearch] = useState('')
  const [debouncedSearch, setDebouncedSearch] = useState('')
  const [selectedTourId, setSelectedTourId] = useState<string | null>(null)

  useEffect(() => {
    const timer = setTimeout(() => setDebouncedSearch(search), 300)
    return () => clearTimeout(timer)
  }, [search])

  const { data: tours = [], isLoading, error } = useQuery({
    queryKey: [TOURS_QUERY_KEY, debouncedSearch],
    queryFn: () => toursApi.getAll(debouncedSearch || undefined),
  })

  const deleteMutation = useMutation({
    mutationFn: (id: string) => toursApi.delete(id),
    onSuccess: (_, id) => {
      queryClient.invalidateQueries({ queryKey: [TOURS_QUERY_KEY] })
      if (selectedTourId === id) setSelectedTourId(null)
    },
  })

  const importMutation = useMutation({
    mutationFn: (data: unknown) => toursApi.importTours(data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: [TOURS_QUERY_KEY] }),
  })

  const exportTours = async () => {
    const data = await toursApi.exportTours()
    const blob = new Blob([JSON.stringify(data, null, 2)], { type: 'application/json' })
    const url = URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url
    a.download = `tourplanner-export-${new Date().toISOString().slice(0, 10)}.json`
    a.click()
    URL.revokeObjectURL(url)
  }

  const importTours = async (file: File) => {
    let data: unknown
    try {
      data = JSON.parse(await file.text())
    } catch {
      throw new Error('That file is not valid JSON.')
    }
    return importMutation.mutateAsync(data)
  }

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
    exportTours,
    importTours,
    isImporting: importMutation.isPending,
  }
}
