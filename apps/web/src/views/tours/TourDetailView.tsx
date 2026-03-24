import { ArrowLeft, MapPin, Clock, Ruler, Users, Baby } from 'lucide-react'
import { useTourDetailViewModel } from '@/viewmodels/useTourDetailViewModel'
import { TourMap } from '@/components/TourMap'
import { Badge } from '@/components/ui/badge'
import { TourLogListView } from '@/views/tourLogs/TourLogListView'
import { TRANSPORT_ICONS } from '@/models/tour'
import { formatDistance, formatDuration } from '@/lib/utils'

interface TourDetailViewProps {
  tourId: string
  onBack?: () => void
}

const CHILD_FRIENDLY_MAP = {
  VerySuitable: { label: 'Very suitable', variant: 'default' as const },
  Suitable: { label: 'Suitable', variant: 'secondary' as const },
  NotSuitable: { label: 'Not suitable', variant: 'outline' as const },
}

export function TourDetailView({ tourId, onBack }: TourDetailViewProps) {
  const { tour, isLoading } = useTourDetailViewModel(tourId)

  if (isLoading) {
    return (
      <div className="flex-1 flex items-center justify-center text-stone-400 text-sm">
        Loading tour details…
      </div>
    )
  }

  if (!tour) return null

  const cfInfo = CHILD_FRIENDLY_MAP[tour.childFriendliness]

  return (
    <div className="flex-1 flex flex-col overflow-hidden bg-white">
      <div className="px-4 sm:px-6 py-4 sm:py-5 border-b border-stone-100">
        {onBack && (
          <button
            className="sm:hidden flex items-center gap-1.5 text-xs text-stone-500 hover:text-stone-700 mb-3 -ml-0.5"
            onClick={onBack}
          >
            <ArrowLeft className="h-3.5 w-3.5" />
            Back to tours
          </button>
        )}
        <div className="flex items-start gap-3">
          <span className="text-3xl">{TRANSPORT_ICONS[tour.transportType]}</span>
          <div className="flex-1 min-w-0">
            <h2 className="text-xl font-semibold text-stone-900 truncate">{tour.name}</h2>
            <p className="text-sm text-stone-500 mt-0.5">
              <MapPin className="inline h-3.5 w-3.5 mr-0.5 -mt-0.5" />
              {tour.from} → {tour.to}
            </p>
            {tour.description && (
              <p className="text-sm text-stone-600 mt-2 line-clamp-2">{tour.description}</p>
            )}
          </div>
        </div>

        {/* Stats row */}
        <div className="grid grid-cols-2 sm:grid-cols-4 gap-3 mt-4">
          <StatCard
            icon={<Ruler className="h-4 w-4" />}
            label="Distance"
            value={formatDistance(tour.distance)}
          />
          <StatCard
            icon={<Clock className="h-4 w-4" />}
            label="Est. Time"
            value={formatDuration(tour.estimatedTimeSeconds)}
          />
          <StatCard
            icon={<Users className="h-4 w-4" />}
            label="Popularity"
            value={`${tour.popularity} ${tour.popularity === 1 ? 'log' : 'logs'}`}
          />
          <div className="bg-stone-50 rounded-lg px-3 py-2.5 flex flex-col gap-1">
            <div className="flex items-center gap-1.5 text-stone-400 text-xs">
              <Baby className="h-4 w-4" />
              <span>Child-Friendly</span>
            </div>
            <Badge variant={cfInfo.variant} className="self-start text-xs">
              {cfInfo.label}
            </Badge>
          </div>
        </div>
      </div>

      <div className="flex-1 overflow-y-auto">
        {/* Tour image (if uploaded) */}
        {tour.imagePath && (
          <div className="px-4 sm:px-6 pt-4">
            <h3 className="text-sm font-medium text-stone-700 mb-3">Tour Image</h3>
            <img
              src={`/images/${tour.imagePath}`}
              alt={tour.name}
              className="w-full h-48 sm:h-56 object-cover rounded-xl border border-stone-200"
            />
          </div>
        )}

        {/* Map section */}
        <div className="px-4 sm:px-6 py-4">
          <h3 className="text-sm font-medium text-stone-700 mb-3">Route Map</h3>
          <TourMap from={tour.from} to={tour.to} transportType={tour.transportType} className="h-48 sm:h-56" />
        </div>

        {/* Tour Logs */}
        <div className="border-t border-stone-100">
          <TourLogListView tourId={tourId} />
        </div>
      </div>
    </div>
  )
}

function StatCard({
  icon,
  label,
  value,
}: {
  icon: React.ReactNode
  label: string
  value: string
}) {
  return (
    <div className="bg-stone-50 rounded-lg px-3 py-2.5 flex flex-col gap-1">
      <div className="flex items-center gap-1.5 text-stone-400 text-xs">
        {icon}
        <span>{label}</span>
      </div>
      <span className="font-semibold text-stone-800 text-sm">{value}</span>
    </div>
  )
}
