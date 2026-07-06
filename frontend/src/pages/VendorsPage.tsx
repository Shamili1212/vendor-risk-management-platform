import { Search } from 'lucide-react';
import { DataTable } from '../components/DataTable';
import { RiskBadge } from '../components/RiskBadge';
import type { Vendor } from '../lib/types';

export function VendorsPage({ vendors }: { vendors: Vendor[] }) {
  return (
    <div className="space-y-4">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <h2 className="text-xl font-semibold text-ink">Vendors</h2>
        <label className="relative block sm:w-80">
          <Search className="pointer-events-none absolute left-3 top-2.5 h-4 w-4 text-zinc-400" />
          <input className="w-full rounded-md border border-zinc-300 bg-white py-2 pl-9 pr-3 text-sm outline-none focus:border-marine focus:ring-2 focus:ring-teal-100" placeholder="Search vendors" />
        </label>
      </div>
      <DataTable
        columns={['Vendor', 'Category', 'Owner', 'Status', 'Risk', 'Incidents']}
        rows={vendors}
        renderRow={(vendor) => (
          <tr key={vendor.id}>
            <td className="px-4 py-3 font-medium text-ink">{vendor.name}</td>
            <td className="px-4 py-3 text-sm text-zinc-600">{vendor.category}</td>
            <td className="px-4 py-3 text-sm text-zinc-600">{vendor.ownerName}</td>
            <td className="px-4 py-3 text-sm text-zinc-600">{vendor.status}</td>
            <td className="px-4 py-3"><RiskBadge tier={vendor.riskTier} /></td>
            <td className="px-4 py-3 text-sm text-zinc-600">{vendor.incidentCount}</td>
          </tr>
        )}
      />
    </div>
  );
}
