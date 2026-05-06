import {
  AreaChart,
  Area,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
} from 'recharts'

interface ElevationChartProps {
  elevations: number[]
  distanceMeters: number
  className?: string
}

interface ChartPoint {
  km: number
  elevation: number
}

function downsample(elevations: number[], distanceMeters: number, maxPoints = 200): ChartPoint[] {
  if (elevations.length === 0) return []

  const step = Math.max(1, Math.floor(elevations.length / maxPoints))
  const sampled: ChartPoint[] = []

  for (let i = 0; i < elevations.length; i += step) {
    sampled.push({
      km: parseFloat(((i / (elevations.length - 1)) * (distanceMeters / 1000)).toFixed(2)),
      elevation: Math.round(elevations[i]),
    })
  }

  // Always include the last point
  const last = elevations.length - 1
  if (sampled[sampled.length - 1]?.km !== parseFloat((distanceMeters / 1000).toFixed(2))) {
    sampled.push({
      km: parseFloat((distanceMeters / 1000).toFixed(2)),
      elevation: Math.round(elevations[last]),
    })
  }

  return sampled
}

function formatKm(value: number) {
  return `${value} km`
}

function formatElevation(value: number) {
  return `${value} m`
}

export function ElevationChart({ elevations, distanceMeters, className }: ElevationChartProps) {
  if (elevations.length < 2) return null

  const data = downsample(elevations, distanceMeters)
  const elevValues = data.map((d) => d.elevation)
  const minElev = Math.min(...elevValues)
  const maxElev = Math.max(...elevValues)
  // Pad y-axis by 5% of range so the line doesn't hug the edges
  const pad = Math.max(10, Math.round((maxElev - minElev) * 0.05))
  const yMin = Math.max(0, minElev - pad)
  const yMax = maxElev + pad

  return (
    <div className={className}>
      <ResponsiveContainer width="100%" height="100%">
        <AreaChart data={data} margin={{ top: 4, right: 8, left: 0, bottom: 0 }}>
          <defs>
            <linearGradient id="elevGradient" x1="0" y1="0" x2="0" y2="1">
              <stop offset="5%" stopColor="#3d7d3d" stopOpacity={0.3} />
              <stop offset="95%" stopColor="#3d7d3d" stopOpacity={0.03} />
            </linearGradient>
          </defs>
          <CartesianGrid strokeDasharray="3 3" stroke="#e7e5e4" vertical={false} />
          <XAxis
            dataKey="km"
            tickFormatter={formatKm}
            tick={{ fontSize: 11, fill: '#78716c' }}
            axisLine={false}
            tickLine={false}
            interval="preserveStartEnd"
          />
          <YAxis
            domain={[yMin, yMax]}
            tickFormatter={formatElevation}
            tick={{ fontSize: 11, fill: '#78716c' }}
            axisLine={false}
            tickLine={false}
            width={52}
          />
          <Tooltip
            formatter={(value) => [`${value} m`, 'Elevation']}
            labelFormatter={(label) => `${label} km`}
            contentStyle={{
              fontSize: 12,
              borderRadius: 8,
              border: '1px solid #e7e5e4',
              boxShadow: '0 1px 6px rgba(0,0,0,0.08)',
            }}
          />
          <Area
            type="monotone"
            dataKey="elevation"
            stroke="#3d7d3d"
            strokeWidth={2}
            fill="url(#elevGradient)"
            dot={false}
            activeDot={{ r: 4, strokeWidth: 0 }}
          />
        </AreaChart>
      </ResponsiveContainer>
    </div>
  )
}
