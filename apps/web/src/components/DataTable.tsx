import {
  useReactTable,
  getCoreRowModel,
  flexRender,
  type ColumnDef,
} from '@tanstack/react-table'
import { cn } from '@/lib/utils'

interface DataTableProps<TData> {
  data: TData[]
  columns: ColumnDef<TData, unknown>[]
  isLoading?: boolean
  emptyMessage?: string
  onRowClick?: (row: TData) => void
  selectedId?: string | null
  getRowId?: (row: TData) => string
}

export function DataTable<TData>({
  data,
  columns,
  isLoading = false,
  emptyMessage = 'No items found.',
  onRowClick,
  selectedId,
  getRowId,
}: DataTableProps<TData>) {
  // eslint-disable-next-line react-hooks/incompatible-library
  const table = useReactTable({
    data,
    columns,
    getCoreRowModel: getCoreRowModel(),
    getRowId: getRowId as ((row: TData, index: number) => string) | undefined,
  })

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-12 text-stone-400 text-sm">
        Loading…
      </div>
    )
  }

  if (data.length === 0) {
    return (
      <div className="flex items-center justify-center py-12 text-stone-400 text-sm">
        {emptyMessage}
      </div>
    )
  }

  return (
    <div className="w-full overflow-auto">
      <table className="w-full text-sm">
        <thead>
          {table.getHeaderGroups().map(headerGroup => (
            <tr key={headerGroup.id} className="border-b border-stone-200">
              {headerGroup.headers.map(header => (
                <th
                  key={header.id}
                  className="px-3 py-2.5 text-left text-xs font-medium text-stone-500 uppercase tracking-wide"
                >
                  {header.isPlaceholder
                    ? null
                    : flexRender(header.column.columnDef.header, header.getContext())}
                </th>
              ))}
            </tr>
          ))}
        </thead>
        <tbody>
          {table.getRowModel().rows.map(row => {
            const rowId = getRowId ? getRowId(row.original) : row.id
            const isSelected = selectedId ? rowId === selectedId : false
            return (
              <tr
                key={row.id}
                onClick={() => onRowClick?.(row.original)}
                className={cn(
                  'border-b border-stone-100 transition-colors',
                  onRowClick && 'cursor-pointer hover:bg-stone-50',
                  isSelected && 'bg-forest-50 hover:bg-forest-50',
                )}
              >
                {row.getVisibleCells().map(cell => (
                  <td key={cell.id} className="px-3 py-3">
                    {flexRender(cell.column.columnDef.cell, cell.getContext())}
                  </td>
                ))}
              </tr>
            )
          })}
        </tbody>
      </table>
    </div>
  )
}
