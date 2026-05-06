import { useEffect } from 'react'
import { MapContainer, TileLayer, Polyline, Marker, useMap } from 'react-leaflet'
import L from 'leaflet'
import { MapPin } from 'lucide-react'

// Leaflet marker icon fix for bundled environments
const makePin = (color: string) =>
  L.divIcon({
    className: '',
    html: `<div style="width:14px;height:14px;background:${color};border:2.5px solid white;border-radius:50%;box-shadow:0 1px 4px rgba(0,0,0,0.35)"></div>`,
    iconSize: [14, 14],
    iconAnchor: [7, 7],
  })

const START_ICON = makePin('#3d7d3d') // forest-500
const END_ICON = makePin('#e07b3a')   // trail-500

function FitRoute({ coords }: { coords: [number, number][] }) {
  const map = useMap()
  useEffect(() => {
    if (coords.length > 1) {
      map.fitBounds(L.latLngBounds(coords), { padding: [24, 24], maxZoom: 14 })
    }
  }, [map, coords])
  return null
}

function MapFallback({
  from,
  to,
  className,
}: {
  from?: string
  to?: string
  className?: string
}) {
  return (
    <div
      className={`flex flex-col items-center justify-center gap-3 rounded-xl border-2 border-dashed border-stone-200 bg-stone-50 text-stone-400 ${className ?? 'h-64'}`}
    >
      <MapPin className="h-8 w-8 text-stone-300" />
      <div className="text-center text-sm">
        <p className="font-medium text-stone-500">Map Preview</p>
        {from && to ? (
          <p className="text-xs mt-1">
            {from} → {to}
          </p>
        ) : (
          <p className="text-xs mt-1">Select a tour to see the route</p>
        )}
        <p className="text-xs mt-0.5 text-stone-400">No route available</p>
      </div>
    </div>
  )
}

interface TourMapProps {
  coordinates?: [number, number][]
  from?: string
  to?: string
  className?: string
}

export function TourMap({ coordinates, from, to, className }: TourMapProps) {
  if (!coordinates || coordinates.length < 2) {
    return <MapFallback from={from} to={to} className={className} />
  }

  const center = coordinates[Math.floor(coordinates.length / 2)]

  return (
    <div className={`rounded-xl overflow-hidden ${className ?? 'h-64'}`} style={{ isolation: 'isolate' }}>
      <MapContainer
        center={center}
        zoom={12}
        scrollWheelZoom={false}
        style={{ height: '100%', width: '100%' }}
      >
        <TileLayer
          attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
          url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
        />
        <Marker position={coordinates[0]} icon={START_ICON} />
        <Marker position={coordinates[coordinates.length - 1]} icon={END_ICON} />
        <Polyline positions={coordinates} color="#3d7d3d" weight={4} opacity={0.85} />
        <FitRoute coords={coordinates} />
      </MapContainer>
    </div>
  )
}
