export type TransportType = 'Car' | 'Bicycle' | 'Hiking' | 'Running' | 'Vacation'
export type ChildFriendliness = 'NotSuitable' | 'Suitable' | 'VerySuitable'

export interface Tour {
  id: string
  name: string
  description: string
  from: string
  to: string
  transportType: TransportType
  distance?: number | null
  estimatedTimeSeconds?: number | null
  routeInformation?: string | null
  imagePath?: string | null
  popularity: number
  childFriendliness: ChildFriendliness
  createdAt: string
  updatedAt: string
}

export interface CreateTourPayload {
  name: string
  description: string
  from: string
  to: string
  transportType: TransportType
}

export interface UpdateTourPayload {
  name: string
  description: string
  from: string
  to: string
  transportType: TransportType
  distance?: number | null
  estimatedTimeSeconds?: number | null
  routeInformation?: string | null
}

export interface TourRoute {
  coordinates: [number, number][]
  elevations: number[]
  distanceMeters: number
  durationSeconds: number
}

export function parseRoute(routeInformation?: string | null): TourRoute | null {
  if (!routeInformation) return null
  try {
    const r = JSON.parse(routeInformation)
    if (!Array.isArray(r.coordinates) || r.coordinates.length < 2) return null
    return {
      coordinates: r.coordinates as [number, number][],
      elevations: Array.isArray(r.elevations) ? r.elevations : [],
      distanceMeters: typeof r.distanceMeters === 'number' ? r.distanceMeters : 0,
      durationSeconds: typeof r.durationSeconds === 'number' ? r.durationSeconds : 0,
    }
  } catch {
    return null
  }
}

export const TRANSPORT_TYPES: TransportType[] = ['Car', 'Bicycle', 'Hiking', 'Running', 'Vacation']

export const TRANSPORT_ICONS: Record<TransportType, string> = {
  Car: '🚗',
  Bicycle: '🚴',
  Hiking: '🥾',
  Running: '🏃',
  Vacation: '✈️',
}
