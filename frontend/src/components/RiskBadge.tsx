import type { RiskTier } from '../lib/types';

const styles: Record<RiskTier, string> = {
  Low: 'bg-emerald-50 text-emerald-700 ring-emerald-200',
  Medium: 'bg-amber-50 text-amber-700 ring-amber-200',
  High: 'bg-orange-50 text-orange-700 ring-orange-200',
  Critical: 'bg-rose-50 text-rose-700 ring-rose-200'
};

export function RiskBadge({ tier }: { tier: RiskTier }) {
  return <span className={`rounded px-2 py-1 text-xs font-semibold ring-1 ${styles[tier]}`}>{tier}</span>;
}
