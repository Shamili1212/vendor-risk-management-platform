import { AlertTriangle, Bell, ClipboardCheck, FileClock, ShieldCheck } from 'lucide-react';
import { KpiCard } from '../components/KpiCard';
import type { DashboardSummary } from '../lib/types';

export function DashboardPage({ summary }: { summary: DashboardSummary }) {
  const maxDistribution = Math.max(...Object.values(summary.riskDistribution), 1);

  return (
    <div className="space-y-6">
      <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
        <KpiCard label="Total vendors" value={String(summary.totalVendors)} tone="ink" icon={ShieldCheck} />
        <KpiCard label="High-risk vendors" value={String(summary.highRiskVendors)} tone="berry" icon={AlertTriangle} />
        <KpiCard label="Renewals in 30 days" value={String(summary.expiringContracts30Days)} tone="saffron" icon={FileClock} />
        <KpiCard label="Pending approvals" value={String(summary.pendingApprovals)} tone="marine" icon={ClipboardCheck} />
      </div>

      <div className="grid gap-4 lg:grid-cols-[1fr_1.2fr]">
        <section className="rounded-lg bg-white p-5 shadow-sm ring-1 ring-zinc-200">
          <div className="flex items-center gap-2">
            <Bell className="h-5 w-5 text-marine" />
            <h2 className="text-base font-semibold text-ink">Risk Distribution</h2>
          </div>
          <div className="mt-5 space-y-4">
            {Object.entries(summary.riskDistribution).map(([tier, count]) => (
              <div key={tier}>
                <div className="mb-1 flex items-center justify-between text-sm">
                  <span className="font-medium text-zinc-700">{tier}</span>
                  <span className="text-zinc-500">{count}</span>
                </div>
                <div className="h-3 rounded bg-zinc-100">
                  <div
                    className="h-3 rounded bg-marine"
                    style={{ width: `${Math.max(8, (count / maxDistribution) * 100)}%` }}
                    aria-label={`${tier} vendors ${count}`}
                  />
                </div>
              </div>
            ))}
          </div>
        </section>

        <section className="rounded-lg bg-white p-5 shadow-sm ring-1 ring-zinc-200">
          <h2 className="text-base font-semibold text-ink">Recent Audit Activity</h2>
          <div className="mt-4 divide-y divide-zinc-100">
            {summary.recentAuditEvents.map((event) => (
              <article key={event.id} className="py-3">
                <div className="flex flex-wrap items-center justify-between gap-2">
                  <p className="font-medium text-ink">{event.action}</p>
                  <time className="text-xs text-zinc-500">{new Date(event.createdAtUtc).toLocaleString()}</time>
                </div>
                <p className="mt-1 text-sm text-zinc-600">{event.details}</p>
              </article>
            ))}
          </div>
        </section>
      </div>
    </div>
  );
}
