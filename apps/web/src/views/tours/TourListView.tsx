import { useState } from 'react'
import { Plus, Search, Pencil, Trash2, MapPin, Clock, Star } from 'lucide-react'
import { useTourListViewModel } from '@/viewmodels/useTourListViewModel'
import { ConfirmDialog } from '@/components/ConfirmDialog'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { TourDetailView } from './TourDetailView'
import { TourFormDialog } from './TourFormDialog'
import type { Tour } from '@/models/tour'
import { TRANSPORT_ICONS } from '@/models/tour'
import { formatDistance, formatDuration } from '@/lib/utils'

export function TourListView() {
  const {
    tours,
    isLoading,
    search,
    setSearch,
    selectedTourId,
    setSelectedTourId,
    deleteTour,
    isDeletingTour,
  } = useTourListViewModel()

  const [formOpen, setFormOpen] = useState(false)
  const [editTour, setEditTour] = useState<Tour | null>(null)
  const [newTourKey, setNewTourKey] = useState(0)
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false)
  const [tourToDelete, setTourToDelete] = useState<Tour | null>(null)


  return (
    <div className="flex h-[calc(100vh-3.5rem)] overflow-hidden">
      <aside className={`${selectedTourId ? 'hidden' : 'flex'} sm:flex w-full sm:w-80 lg:w-96 flex-shrink-0 border-r border-stone-200 bg-white flex-col`}>
        {/* Header */}
        <div className="p-4 border-b border-stone-100">
          <div className="flex items-center justify-between mb-3">
            <h2 className="font-semibold text-stone-900">My Tours</h2>
            <Button size="sm" onClick={() => { setEditTour(null); setNewTourKey(k => k + 1); setFormOpen(true) }}>
              <Plus className="h-4 w-4" />
              <span className="hidden sm:inline">New Tour</span>
            </Button>
          </div>
          <div className="relative">
            <Search className="absolute left-2.5 top-1/2 -translate-y-1/2 h-4 w-4 text-stone-400" />
            <Input
              placeholder="Search tours…"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              className="pl-8 h-8 text-sm"
            />
          </div>
        </div>

        {/* Tour list */}
        <div className="flex-1 overflow-y-auto">
          {isLoading ? (
            <div className="flex items-center justify-center py-16 text-stone-400 text-sm">
              Loading tours…
            </div>
          ) : tours.length === 0 ? (
            <div className="flex flex-col items-center justify-center py-16 text-stone-400 gap-2">
              <MapPin className="h-8 w-8 text-stone-300" />
              <p className="text-sm font-medium text-stone-500">No tours yet</p>
              <p className="text-xs">Create your first tour to get started</p>
            </div>
          ) : (
            <ul>
              {tours.map((tour) => (
                <li key={tour.id}>
                  <button
                    className={`w-full text-left px-4 py-3 border-b border-stone-50 transition-colors hover:bg-stone-50 ${
                      selectedTourId === tour.id ? 'bg-forest-50 border-l-2 border-l-forest-500' : ''
                    }`}
                    onClick={() => setSelectedTourId(tour.id)}
                  >
                    <div className="flex items-start gap-2.5">
                      <span className="text-xl mt-0.5">{TRANSPORT_ICONS[tour.transportType]}</span>
                      <div className="flex-1 min-w-0">
                        <p className="font-medium text-stone-900 text-sm truncate">{tour.name}</p>
                        <p className="text-xs text-stone-400 mt-0.5 truncate">
                          {tour.from} → {tour.to}
                        </p>
                        <div className="flex items-center gap-3 mt-1.5">
                          {tour.distance && (
                            <span className="flex items-center gap-1 text-xs text-stone-500">
                              <MapPin className="h-3 w-3" />
                              {formatDistance(tour.distance)}
                            </span>
                          )}
                          {tour.estimatedTimeSeconds && (
                            <span className="flex items-center gap-1 text-xs text-stone-500">
                              <Clock className="h-3 w-3" />
                              {formatDuration(tour.estimatedTimeSeconds)}
                            </span>
                          )}
                          <span className="flex items-center gap-1 text-xs text-stone-500">
                            <Star className="h-3 w-3" />
                            {tour.popularity} {tour.popularity === 1 ? 'log' : 'logs'}
                          </span>
                        </div>
                      </div>
                      <div className="flex-shrink-0 flex flex-col gap-1 items-end">
                        <button
                          className="p-1 rounded hover:bg-stone-200 text-stone-300 hover:text-stone-600 transition-colors"
                          onClick={(e) => { e.stopPropagation(); setEditTour(tour); setFormOpen(true) }}
                        >
                          <Pencil className="h-3 w-3" />
                        </button>
                        <button
                          className="p-1 rounded hover:bg-red-100 text-stone-300 hover:text-red-600 transition-colors"
                          onClick={(e) => { e.stopPropagation(); setTourToDelete(tour); setDeleteDialogOpen(true) }}
                        >
                          <Trash2 className="h-3 w-3" />
                        </button>
                      </div>
                    </div>
                  </button>
                </li>
              ))}
            </ul>
          )}
        </div>
      </aside>

      <main className={`${selectedTourId ? 'flex' : 'hidden'} sm:flex flex-1 overflow-hidden`}>
        {selectedTourId ? (
          <TourDetailView tourId={selectedTourId} onBack={() => setSelectedTourId(null)} />
        ) : (
          <div className="flex-1 flex flex-col items-center justify-center gap-4 text-stone-400 bg-stone-50">
            <MapPin className="h-12 w-12 text-stone-300" />
            <div className="text-center">
              <p className="font-medium text-stone-500">Select a tour</p>
              <p className="text-sm mt-1">Choose a tour from the list to view details</p>
            </div>
          </div>
        )}
      </main>

      <TourFormDialog
        open={formOpen}
        onOpenChange={(v) => { setFormOpen(v); if (!v) setEditTour(null) }}
        tour={editTour}
        formKey={newTourKey}
      />

      <ConfirmDialog
        open={deleteDialogOpen}
        onOpenChange={setDeleteDialogOpen}
        title="Delete tour"
        description={`Are you sure you want to delete "${tourToDelete?.name}"? This will also delete all associated tour logs.`}
        confirmLabel="Delete"
        destructive
        isLoading={isDeletingTour}
        onConfirm={() => {
          if (tourToDelete) {
            deleteTour(tourToDelete.id, {
              onSuccess: () => { setDeleteDialogOpen(false); setTourToDelete(null) },
            })
          }
        }}
      />
    </div>
  )
}
