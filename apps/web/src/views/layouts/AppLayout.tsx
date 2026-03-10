import { Outlet, Link, useNavigate } from '@tanstack/react-router'
import { Mountain, Map, LogOut, User, Menu, X } from 'lucide-react'
import { useState } from 'react'
import { useAuthStore } from '@/stores/authStore'
import { authApi } from '@/api/auth'
import { Button } from '@/components/ui/button'

export function AppLayout() {
  const { user, clearAuth } = useAuthStore()
  const navigate = useNavigate()
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false)

  const handleLogout = async () => {
    try { await authApi.logout() } catch { /* best-effort */ }
    clearAuth()
    navigate({ to: '/login' })
  }

  return (
    <div className="min-h-screen bg-stone-50 flex flex-col">
      <header className="sticky top-0 z-40 border-b border-stone-200 bg-white/80 backdrop-blur-sm">
        <div className="flex h-14 items-center px-4 sm:px-6 gap-4">
          {/* Logo */}
          <Link to="/tours" className="flex items-center gap-2 font-semibold text-forest-700 mr-4">
            <Mountain className="h-5 w-5 text-forest-600" />
            <span className="hidden sm:inline">TourPlanner</span>
          </Link>

          {/* Desktop nav */}
          <nav className="hidden sm:flex items-center gap-1 flex-1">
            <Link
              to="/tours"
              className="flex items-center gap-1.5 px-3 py-1.5 rounded-md text-sm text-stone-600 hover:text-stone-900 hover:bg-stone-100 transition-colors [&.active]:text-forest-700 [&.active]:bg-forest-50"
            >
              <Map className="h-4 w-4" />
              Tours
            </Link>
          </nav>

          {/* Right side */}
          <div className="ml-auto flex items-center gap-2">
            {user && (
              <div className="hidden sm:flex items-center gap-2 text-sm text-stone-600">
                <User className="h-4 w-4" />
                <span>{user.userName}</span>
              </div>
            )}
            <Button
              variant="ghost"
              size="sm"
              onClick={handleLogout}
              className="hidden sm:flex gap-1.5 text-stone-600"
            >
              <LogOut className="h-4 w-4" />
              Logout
            </Button>

            {/* Mobile menu toggle */}
            <button
              className="sm:hidden p-2 rounded-md text-stone-500 hover:bg-stone-100"
              onClick={() => setMobileMenuOpen(v => !v)}
            >
              {mobileMenuOpen ? <X className="h-5 w-5" /> : <Menu className="h-5 w-5" />}
            </button>
          </div>
        </div>

        {/* Mobile dropdown */}
        {mobileMenuOpen && (
          <div className="sm:hidden border-t border-stone-100 bg-white px-4 pb-3 space-y-1">
            <Link
              to="/tours"
              className="flex items-center gap-2 px-3 py-2 rounded-md text-sm text-stone-700 hover:bg-stone-50"
              onClick={() => setMobileMenuOpen(false)}
            >
              <Map className="h-4 w-4" />Tours
            </Link>
            <button
              onClick={handleLogout}
              className="w-full flex items-center gap-2 px-3 py-2 rounded-md text-sm text-stone-700 hover:bg-stone-50"
            >
              <LogOut className="h-4 w-4" />Logout
            </button>
          </div>
        )}
      </header>

      <main className="flex-1 overflow-hidden">
        <Outlet />
      </main>
    </div>
  )
}

export function GuestLayout() {
  return (
    <div className="min-h-screen bg-gradient-to-br from-forest-800 via-forest-700 to-forest-600 flex items-center justify-center p-4">
      <div className="w-full max-w-md">
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center h-12 w-12 rounded-2xl bg-white/10 backdrop-blur mb-4">
            <Mountain className="h-6 w-6 text-white" />
          </div>
          <h1 className="text-2xl font-bold text-white">TourPlanner</h1>
          <p className="text-forest-200 text-sm mt-1">Plan, track, explore.</p>
        </div>
        <Outlet />
      </div>
    </div>
  )
}


