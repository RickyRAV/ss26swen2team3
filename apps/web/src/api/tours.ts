import { apiFetch, apiUpload } from './client'
import type { Tour, CreateTourPayload, UpdateTourPayload } from '@/models/tour'

export const toursApi = {
  getAll: (search?: string) => {
    const params = search ? `?q=${encodeURIComponent(search)}` : ''
    return apiFetch<Tour[]>(`/api/v1/tours${params}`)
  },

  getById: (id: string) => apiFetch<Tour>(`/api/v1/tours/${id}`),

  create: (payload: CreateTourPayload) =>
    apiFetch<Tour>('/api/v1/tours', {
      method: 'POST',
      body: JSON.stringify(payload),
    }),

  update: (id: string, payload: UpdateTourPayload) =>
    apiFetch<Tour>(`/api/v1/tours/${id}`, {
      method: 'PUT',
      body: JSON.stringify(payload),
    }),

  delete: (id: string) =>
    apiFetch<void>(`/api/v1/tours/${id}`, { method: 'DELETE' }),

  uploadImage: (id: string, file: File) => {
    const form = new FormData()
    form.append('file', file)
    return apiUpload<Tour>(`/api/v1/tours/${id}/image`, form)
  },
}
