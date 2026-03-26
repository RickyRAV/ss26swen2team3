import { useState } from 'react'
import { type ColumnDef } from '@tanstack/react-table'
import { Plus, Pencil, Trash2, Star } from 'lucide-react'
import { useTourLogListViewModel } from '@/viewmodels/useTourLogListViewModel'
import { DataTable } from '@/components/DataTable'
import { ConfirmDialog } from '@/components/ConfirmDialog'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { TourLogFormDialog } from './TourLogFormDialog'
import type { TourLog } from '@/models/tourLog'
import { RATING_TO_NUMBER } from '@/models/tourLog'
import { formatDistance, formatDuration, formatDate } from '@/lib/utils'

interface TourLogListViewProps {
  tourId: string
}

const DIFFICULTY_VARIANT = {
  Easy: 'default',
  Medium: 'trail',
  Hard: 'destructive',
} as const

export function TourLogListView({ tourId }: TourLogListViewProps) {
  const { logs, isLoading, deleteLog, isDeletingLog } = useTourLogListViewModel(tourId)
  const [formOpen, setFormOpen] = useState(false)
  const [editLog, setEditLog] = useState<TourLog | null>(null)
  const [newLogKey, setNewLogKey] = useState(0)
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false)
  const [logToDelete, setLogToDelete] = useState<TourLog | null>(null)

  const columns: ColumnDef<TourLog, unknown>[] = [
    {
      accessorKey: 'dateTime',
      header: 'Date',
      cell: ({ getValue }) => (
        <span className="text-stone-600 text-xs">{formatDate(getValue() as string)}</span>
      ),
    },
    {
      accessorKey: 'comment',
      header: 'Comment',
      cell: ({ getValue }) => (
        <span className="text-stone-700 text-xs line-clamp-1">{(getValue() as string) || '—'}</span>
      ),
    },
    {
      accessorKey: 'difficulty',
      header: 'Difficulty',
      cell: ({ getValue }) => {
        const d = getValue() as TourLog['difficulty']
        return <Badge variant={DIFFICULTY_VARIANT[d]}>{d}</Badge>
      },
    },
    {
      accessorKey: 'totalDistanceKm',
      header: 'Distance',
      cell: ({ getValue }) => (
        <span className="text-stone-600 text-xs">{formatDistance(getValue() as number)}</span>
      ),
    },
    {
      accessorKey: 'totalTimeSeconds',
      header: 'Time',
      cell: ({ getValue }) => (
        <span className="text-stone-600 text-xs">{formatDuration(getValue() as number)}</span>
      ),
    },
    {
      accessorKey: 'rating',
      header: 'Rating',
      cell: ({ getValue }) => {
        const n = RATING_TO_NUMBER[getValue() as TourLog['rating']]
        return (
          <div className="flex gap-0.5">
            {Array.from({ length: 5 }, (_, i) => (
              <Star
                key={i}
                className={`h-3 w-3 ${i < n ? 'fill-trail-500 text-trail-500' : 'text-stone-300'}`}
              />
            ))}
          </div>
        )
      },
    },
    {
      id: 'actions',
      header: '',
      cell: ({ row }) => (
        <div className="flex items-center gap-1">
          <button
            className="p-1.5 rounded hover:bg-stone-100 text-stone-400 hover:text-stone-700 transition-colors"
            onClick={() => { setEditLog(row.original); setFormOpen(true) }}
          >
            <Pencil className="h-3.5 w-3.5" />
          </button>
          <button
            className="p-1.5 rounded hover:bg-red-50 text-stone-400 hover:text-red-600 transition-colors"
            onClick={() => { setLogToDelete(row.original); setDeleteDialogOpen(true) }}
          >
            <Trash2 className="h-3.5 w-3.5" />
          </button>
        </div>
      ),
    },
  ]

  return (
    <div className="px-4 sm:px-6 py-4">
      <div className="flex items-center justify-between mb-4">
        <h3 className="text-sm font-medium text-stone-700">
          Tour Logs
          <span className="ml-2 text-xs text-stone-400">({logs.length})</span>
        </h3>
        <Button size="sm" variant="outline" onClick={() => { setEditLog(null); setNewLogKey(k => k + 1); setFormOpen(true) }}>
          <Plus className="h-3.5 w-3.5" />
          Add Log
        </Button>
      </div>

      <DataTable
        data={logs}
        columns={columns}
        isLoading={isLoading}
        emptyMessage="No logs yet. Record your first trip!"
        getRowId={(row) => row.id}
      />

      <TourLogFormDialog
        open={formOpen}
        onOpenChange={(v) => { setFormOpen(v); if (!v) setEditLog(null) }}
        tourId={tourId}
        log={editLog}
        formKey={newLogKey}
      />

      <ConfirmDialog
        open={deleteDialogOpen}
        onOpenChange={setDeleteDialogOpen}
        title="Delete tour log"
        description="Are you sure you want to delete this log entry? This action cannot be undone."
        confirmLabel="Delete"
        destructive
        isLoading={isDeletingLog}
        onConfirm={() => {
          if (logToDelete) {
            deleteLog({ id: logToDelete.id }, {
              onSuccess: () => { setDeleteDialogOpen(false); setLogToDelete(null) },
            })
          }
        }}
      />
    </div>
  )
}
