import type { ReactNode } from 'react';

interface DataTableProps<T> {
  columns: string[];
  rows: T[];
  renderRow: (row: T) => ReactNode;
}

export function DataTable<T>({ columns, rows, renderRow }: DataTableProps<T>) {
  return (
    <div className="overflow-hidden rounded-lg bg-white shadow-sm ring-1 ring-zinc-200">
      <table className="min-w-full divide-y divide-zinc-200">
        <thead className="bg-zinc-50">
          <tr>
            {columns.map((column) => (
              <th key={column} className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-normal text-zinc-500">
                {column}
              </th>
            ))}
          </tr>
        </thead>
        <tbody className="divide-y divide-zinc-100">{rows.map(renderRow)}</tbody>
      </table>
    </div>
  );
}
