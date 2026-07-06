import { DataTable } from '../components/DataTable';
import type { Contract } from '../lib/types';

export function ContractsPage({ contracts }: { contracts: Contract[] }) {
  return (
    <div className="space-y-4">
      <h2 className="text-xl font-semibold text-ink">Contracts</h2>
      <DataTable
        columns={['Contract', 'Vendor', 'Renewal', 'Value', 'Status']}
        rows={contracts}
        renderRow={(contract) => (
          <tr key={contract.id}>
            <td className="px-4 py-3 font-medium text-ink">{contract.title}</td>
            <td className="px-4 py-3 text-sm text-zinc-600">{contract.vendorName}</td>
            <td className="px-4 py-3 text-sm text-zinc-600">{contract.renewalDate}</td>
            <td className="px-4 py-3 text-sm text-zinc-600">${contract.value.toLocaleString()}</td>
            <td className="px-4 py-3 text-sm text-zinc-600">{contract.status}</td>
          </tr>
        )}
      />
    </div>
  );
}
