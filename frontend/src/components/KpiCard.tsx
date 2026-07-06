import type { LucideIcon } from 'lucide-react';

interface KpiCardProps {
  label: string;
  value: string;
  tone: 'marine' | 'saffron' | 'berry' | 'ink';
  icon: LucideIcon;
}

const toneMap = {
  marine: 'bg-teal-50 text-marine ring-teal-100',
  saffron: 'bg-amber-50 text-saffron ring-amber-100',
  berry: 'bg-rose-50 text-berry ring-rose-100',
  ink: 'bg-zinc-50 text-ink ring-zinc-100'
};

export function KpiCard({ label, value, tone, icon: Icon }: KpiCardProps) {
  return (
    <section className="rounded-lg bg-white p-4 shadow-sm ring-1 ring-zinc-200">
      <div className="flex items-center justify-between gap-3">
        <p className="text-sm font-medium text-zinc-500">{label}</p>
        <span className={`grid h-9 w-9 place-items-center rounded-md ring-1 ${toneMap[tone]}`}>
          <Icon aria-hidden className="h-5 w-5" />
        </span>
      </div>
      <p className="mt-3 text-3xl font-semibold tracking-normal text-ink">{value}</p>
    </section>
  );
}
