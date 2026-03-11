export interface User {
  userId: string
  email: string
  userName: string
}

export interface AuthResponse {
  accessToken: string
  userId: string
  email: string
  userName: string
}
