import { apiFetch } from './client'
import type { TourLog, CreateTourLogPayload, UpdateTourLogPayload } from '@/models/tourLog'

export const tourLogsApi = {
  getByTourId: (tourId: string) =>
    apiFetch<TourLog[]>(`/api/v1/tours/${tourId}/logs`),

  getById: (tourId: string, id: string) =>
    apiFetch<TourLog>(`/api/v1/tours/${tourId}/logs/${id}`),

  create: (tourId: string, payload: CreateTourLogPayload) =>
    apiFetch<TourLog>(`/api/v1/tours/${tourId}/logs`, {
      method: 'POST',
      body: JSON.stringify(payload),
    }),

  update: (tourId: string, id: string, payload: UpdateTourLogPayload) =>
    apiFetch<TourLog>(`/api/v1/tours/${tourId}/logs/${id}`, {
      method: 'PUT',
      body: JSON.stringify(payload),
    }),

  delete: (tourId: string, id: string) =>
    apiFetch<void>(`/api/v1/tours/${tourId}/logs/${id}`, { method: 'DELETE' }),
}
