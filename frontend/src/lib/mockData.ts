import type { Approval, Contract, DashboardSummary, Vendor } from './types';

export const vendors: Vendor[] = [
  {
    id: 'v-001',
    name: 'Northwind Cloud Services',
    category: 'Cloud Infrastructure',
    ownerName: 'Procurement Manager',
    status: 'Active',
    riskTier: 'High',
    incidentCount: 2,
    complianceStatus: 'ReviewRequired'
  },
  {
    id: 'v-002',
    name: 'Contoso Legal Operations',
    category: 'Professional Services',
    ownerName: 'Admin User',
    status: 'UnderReview',
    riskTier: 'Medium',
    incidentCount: 1,
    complianceStatus: 'Compliant'
  },
  {
    id: 'v-003',
    name: 'Fabrikam Data Processors',
    category: 'Data Processing',
    ownerName: 'Procurement Manager',
    status: 'Active',
    riskTier: 'Critical',
    incidentCount: 5,
    complianceStatus: 'NonCompliant'
  }
];

export const contracts: Contract[] = [
  {
    id: 'c-001',
    vendorName: 'Northwind Cloud Services',
    title: 'Enterprise Cloud Hosting Agreement',
    renewalDate: '2026-11-15',
    value: 450000,
    status: 'Active'
  },
  {
    id: 'c-002',
    vendorName: 'Fabrikam Data Processors',
    title: 'Customer Analytics Data Addendum',
    renewalDate: '2026-08-02',
    value: 1250000,
    status: 'PendingRenewal'
  }
];

export const approvals: Approval[] = [
  {
    id: 'a-001',
    contractTitle: 'Customer Analytics Data Addendum',
    assignedReviewer: 'Risk Reviewer',
    status: 'Submitted',
    submittedAtUtc: '2026-07-03T10:30:00Z'
  },
  {
    id: 'a-002',
    contractTitle: 'Enterprise Cloud Hosting Agreement',
    assignedReviewer: 'Risk Reviewer',
    status: 'ChangesRequested',
    submittedAtUtc: '2026-06-28T14:10:00Z'
  }
];

export const dashboard: DashboardSummary = {
  totalVendors: 3,
  highRiskVendors: 2,
  expiringContracts30Days: 1,
  expiringContracts90Days: 2,
  pendingApprovals: 1,
  riskDistribution: {
    Low: 0,
    Medium: 1,
    High: 1,
    Critical: 1
  },
  recentAuditEvents: [
    {
      id: 'log-001',
      action: 'RiskCalculation',
      entityName: 'Vendor',
      details: 'Fabrikam score moved to Critical after incident update.',
      createdAtUtc: '2026-07-05T16:45:00Z'
    },
    {
      id: 'log-002',
      action: 'ApprovalDecision',
      entityName: 'ApprovalRequest',
      details: 'Reviewer requested changes for cloud hosting renewal.',
      createdAtUtc: '2026-07-04T12:20:00Z'
    }
  ]
};
