export type Role = 'Admin' | 'ProcurementManager' | 'Reviewer' | 'Auditor';
export type RiskTier = 'Low' | 'Medium' | 'High' | 'Critical';
export type ApprovalStatus = 'Draft' | 'Submitted' | 'Approved' | 'Rejected' | 'ChangesRequested';

export interface Vendor {
  id: string;
  name: string;
  category: string;
  ownerName: string;
  status: string;
  riskTier: RiskTier;
  incidentCount: number;
  complianceStatus: string;
}

export interface Contract {
  id: string;
  vendorName: string;
  title: string;
  renewalDate: string;
  value: number;
  status: string;
}

export interface Approval {
  id: string;
  contractTitle: string;
  assignedReviewer: string;
  status: ApprovalStatus;
  submittedAtUtc?: string;
}

export interface AuditEvent {
  id: string;
  action: string;
  entityName: string;
  details: string;
  createdAtUtc: string;
}

export interface DashboardSummary {
  totalVendors: number;
  highRiskVendors: number;
  expiringContracts30Days: number;
  expiringContracts90Days: number;
  pendingApprovals: number;
  riskDistribution: Record<RiskTier, number>;
  recentAuditEvents: AuditEvent[];
}
