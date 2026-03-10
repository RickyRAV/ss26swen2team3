import { createFileRoute } from '@tanstack/react-router'
import { TourListView } from '@/views/tours/TourListView'

export const Route = createFileRoute('/_auth/tours')({
  component: TourListView,
})
