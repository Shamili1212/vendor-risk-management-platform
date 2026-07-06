import { Check, MessageSquareWarning, X } from 'lucide-react';
import type { Approval } from '../lib/types';

export function ApprovalsPage({ approvals }: { approvals: Approval[] }) {
  return (
    <div className="space-y-4">
      <h2 className="text-xl font-semibold text-ink">Approval Queue</h2>
      <div className="grid gap-4 lg:grid-cols-2">
        {approvals.map((approval) => (
          <article key={approval.id} className="rounded-lg bg-white p-5 shadow-sm ring-1 ring-zinc-200">
            <div className="flex items-start justify-between gap-3">
              <div>
                <p className="font-semibold text-ink">{approval.contractTitle}</p>
                <p className="mt-1 text-sm text-zinc-500">Reviewer: {approval.assignedReviewer}</p>
              </div>
              <span className="rounded bg-teal-50 px-2 py-1 text-xs font-semibold text-marine ring-1 ring-teal-200">{approval.status}</span>
            </div>
            <div className="mt-4 flex flex-wrap gap-2">
              <button className="inline-flex items-center gap-2 rounded-md bg-marine px-3 py-2 text-sm font-semibold text-white shadow-sm">
                <Check className="h-4 w-4" /> Approve
              </button>
              <button className="inline-flex items-center gap-2 rounded-md bg-white px-3 py-2 text-sm font-semibold text-berry ring-1 ring-rose-200">
                <X className="h-4 w-4" /> Reject
              </button>
              <button className="inline-flex items-center gap-2 rounded-md bg-white px-3 py-2 text-sm font-semibold text-saffron ring-1 ring-amber-200">
                <MessageSquareWarning className="h-4 w-4" /> Changes
              </button>
            </div>
          </article>
        ))}
      </div>
    </div>
  );
}
