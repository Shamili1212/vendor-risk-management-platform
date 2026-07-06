import { approvals, contracts, dashboard, vendors } from './mockData';
import type { Approval, Contract, DashboardSummary, Vendor } from './types';

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? '';

async function getJson<T>(path: string, fallback: T): Promise<T> {
  try {
    const token = window.localStorage.getItem('vendorRiskToken');
    const response = await fetch(`${apiBaseUrl}${path}`, {
      headers: token ? { Authorization: `Bearer ${token}` } : undefined
    });

    if (!response.ok) {
      return fallback;
    }

    return (await response.json()) as T;
  } catch {
    return fallback;
  }
}

export const api = {
  dashboard: () => getJson<DashboardSummary>('/api/dashboard/summary', dashboard),
  vendors: () => getJson<Vendor[]>('/api/vendors', vendors),
  contracts: () => getJson<Contract[]>('/api/contracts', contracts),
  approvals: () => getJson<Approval[]>('/api/approvals', approvals)
};
