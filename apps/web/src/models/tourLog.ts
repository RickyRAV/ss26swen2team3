export type Difficulty = 'Easy' | 'Medium' | 'Hard'
export type Rating = 'One' | 'Two' | 'Three' | 'Four' | 'Five'

export const RATING_TO_NUMBER: Record<Rating, number> = {
  One: 1,
  Two: 2,
  Three: 3,
  Four: 4,
  Five: 5,
}

export const NUMBER_TO_RATING: Record<number, Rating> = {
  1: 'One',
  2: 'Two',
  3: 'Three',
  4: 'Four',
  5: 'Five',
}

export interface TourLog {
  id: string
  tourId: string
  dateTime: string
  comment: string
  difficulty: Difficulty
  totalDistanceKm: number
  totalTimeSeconds: number
  rating: Rating
  createdAt: string
  updatedAt: string
}

export interface CreateTourLogPayload {
  dateTime: string
  comment: string
  difficulty: Difficulty
  totalDistanceKm: number
  totalTimeSeconds: number
  rating: Rating
}

export type UpdateTourLogPayload = CreateTourLogPayload

export const DIFFICULTIES: Difficulty[] = ['Easy', 'Medium', 'Hard']
export const RATINGS: Rating[] = ['One', 'Two', 'Three', 'Four', 'Five']
