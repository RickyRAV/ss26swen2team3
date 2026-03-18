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

export const TRANSPORT_TYPES: TransportType[] = ['Car', 'Bicycle', 'Hiking', 'Running', 'Vacation']

export const TRANSPORT_ICONS: Record<TransportType, string> = {
  Car: '🚗',
  Bicycle: '🚴',
  Hiking: '🥾',
  Running: '🏃',
  Vacation: '✈️',
}
