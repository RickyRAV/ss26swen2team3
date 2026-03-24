import { apiFetch } from '@/api/client'
import type { TransportType } from '@/models/tour'

export interface RouteResult {
  /** [lat, lon] pairs ready for Leaflet */
  coordinates: [number, number][]
  distanceMeters: number
  durationSeconds: number
}

interface RouteResponse {
  coordinates: number[][]
  distanceMeters: number
  durationSeconds: number
}

export async function fetchRoute(
  from: string,
  to: string,
  transportType: TransportType,
): Promise<RouteResult> {
  const params = new URLSearchParams({ from, to, transportType })
  const res = await apiFetch<RouteResponse>(`/api/v1/map/route?${params}`)

  return {
    // Backend returns [lat, lon] already (swapped from ORS GeoJSON)
    coordinates: res.coordinates as [number, number][],
    distanceMeters: res.distanceMeters,
    durationSeconds: res.durationSeconds,
  }
}
