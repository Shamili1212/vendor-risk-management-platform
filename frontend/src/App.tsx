import { useEffect, useState } from 'react';
import { BarChart3, ClipboardCheck, FileText, Gauge, History, Shield, Users } from 'lucide-react';
import { DashboardPage } from './pages/DashboardPage';
import { VendorsPage } from './pages/VendorsPage';
import { ContractsPage } from './pages/ContractsPage';
import { ApprovalsPage } from './pages/ApprovalsPage';
import { api } from './lib/api';
import { approvals as fallbackApprovals, contracts as fallbackContracts, dashboard as fallbackDashboard, vendors as fallbackVendors } from './lib/mockData';
import type { Approval, Contract, DashboardSummary, Role, Vendor } from './lib/types';

type View = 'dashboard' | 'vendors' | 'contracts' | 'approvals';

const navItems: Array<{ id: View; label: string; icon: typeof Gauge }> = [
  { id: 'dashboard', label: 'Dashboard', icon: Gauge },
  { id: 'vendors', label: 'Vendors', icon: Users },
  { id: 'contracts', label: 'Contracts', icon: FileText },
  { id: 'approvals', label: 'Approvals', icon: ClipboardCheck }
];

export function App() {
  const [view, setView] = useState<View>('dashboard');
  const [role, setRole] = useState<Role>('ProcurementManager');
  const [summary, setSummary] = useState<DashboardSummary>(fallbackDashboard);
  const [vendors, setVendors] = useState<Vendor[]>(fallbackVendors);
  const [contracts, setContracts] = useState<Contract[]>(fallbackContracts);
  const [approvals, setApprovals] = useState<Approval[]>(fallbackApprovals);

  useEffect(() => {
    void Promise.all([api.dashboard(), api.vendors(), api.contracts(), api.approvals()]).then(([nextSummary, nextVendors, nextContracts, nextApprovals]) => {
      setSummary(nextSummary);
      setVendors(nextVendors);
      setContracts(nextContracts);
      setApprovals(nextApprovals);
    });
  }, []);

  return (
    <div className="min-h-screen bg-surface">
      <header className="border-b border-zinc-200 bg-white">
        <div className="mx-auto flex max-w-7xl flex-col gap-4 px-4 py-4 sm:px-6 lg:flex-row lg:items-center lg:justify-between lg:px-8">
          <div className="flex items-center gap-3">
            <span className="grid h-10 w-10 place-items-center rounded-lg bg-marine text-white shadow-sm">
              <Shield className="h-5 w-5" />
            </span>
            <div>
              <h1 className="text-lg font-semibold text-ink">Vendor Risk & Contract Renewal Management</h1>
              <p className="text-sm text-zinc-500">Enterprise procurement control center</p>
            </div>
          </div>
          <div className="flex items-center gap-3">
            <label className="text-sm font-medium text-zinc-600" htmlFor="role">Role</label>
            <select
              id="role"
              className="rounded-md border border-zinc-300 bg-white px-3 py-2 text-sm outline-none focus:border-marine focus:ring-2 focus:ring-teal-100"
              value={role}
              onChange={(event) => setRole(event.target.value as Role)}
            >
              <option value="Admin">Admin</option>
              <option value="ProcurementManager">Procurement Manager</option>
              <option value="Reviewer">Reviewer</option>
              <option value="Auditor">Auditor</option>
            </select>
          </div>
        </div>
      </header>

      <main className="mx-auto grid max-w-7xl gap-6 px-4 py-6 sm:px-6 lg:grid-cols-[220px_1fr] lg:px-8">
        <nav className="flex gap-2 overflow-x-auto lg:block lg:space-y-2">
          {navItems.map((item) => {
            const Icon = item.icon;
            const active = view === item.id;
            return (
              <button
                key={item.id}
                type="button"
                onClick={() => setView(item.id)}
                className={`inline-flex min-h-10 items-center gap-2 rounded-md px-3 py-2 text-sm font-semibold ring-1 lg:w-full ${
                  active ? 'bg-marine text-white ring-marine' : 'bg-white text-zinc-700 ring-zinc-200 hover:bg-zinc-50'
                }`}
              >
                <Icon className="h-4 w-4" />
                {item.label}
              </button>
            );
          })}
          {(role === 'Admin' || role === 'Auditor') && (
            <div className="mt-4 hidden rounded-lg bg-white p-4 shadow-sm ring-1 ring-zinc-200 lg:block">
              <div className="flex items-center gap-2">
                <History className="h-4 w-4 text-marine" />
                <p className="text-sm font-semibold text-ink">Audit access</p>
              </div>
              <p className="mt-2 text-sm text-zinc-500">{summary.recentAuditEvents.length} recent events available</p>
            </div>
          )}
        </nav>

        <section>
          {view === 'dashboard' && <DashboardPage summary={summary} />}
          {view === 'vendors' && <VendorsPage vendors={vendors} />}
          {view === 'contracts' && <ContractsPage contracts={contracts} />}
          {view === 'approvals' && <ApprovalsPage approvals={approvals} />}
        </section>
      </main>
    </div>
  );
}
