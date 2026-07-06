# Database Schema

PostgreSQL is accessed through Entity Framework Core migrations. Primary keys are UUIDs (`Guid`) to support distributed records and future imports.

## Entity Relationship Summary

```text
Users 1--* Vendors
Vendors 1--* Contracts
Vendors 1--* RiskAssessments
Contracts 1--* ApprovalRequests
ApprovalRequests 1--* ApprovalComments
Users 1--* ApprovalRequests as requester/reviewer
Users 1--* Notifications
Users 1--* AuditLogs
Contracts 1--* DocumentMetadata
```

## Users

| Column | Type | Notes |
| --- | --- | --- |
| Id | uuid | Primary key |
| FullName | varchar(160) | Required |
| Email | varchar(240) | Unique, required |
| PasswordHash | text | Required |
| Role | enum/string | Admin, ProcurementManager, Reviewer, Auditor |
| IsActive | boolean | Soft access control |
| CreatedAtUtc | timestamptz | Required |

## Vendors

| Column | Type | Notes |
| --- | --- | --- |
| Id | uuid | Primary key |
| Name | varchar(220) | Required, indexed |
| Category | varchar(120) | Required |
| Status | enum/string | Active, UnderReview, Suspended, Offboarded |
| Criticality | enum/string | Low, Medium, High, Critical |
| ComplianceStatus | enum/string | Compliant, ReviewRequired, NonCompliant |
| OwnerId | uuid | FK to Users |
| RiskTier | enum/string | Low, Medium, High, Critical |
| IncidentCount | int | Non-negative |
| CreatedAtUtc | timestamptz | Required |
| UpdatedAtUtc | timestamptz | Nullable |

## Contracts

| Column | Type | Notes |
| --- | --- | --- |
| Id | uuid | Primary key |
| VendorId | uuid | FK to Vendors |
| Title | varchar(220) | Required |
| StartDate | date | Required |
| EndDate | date | Required |
| RenewalDate | date | Required |
| Value | numeric(18,2) | Required |
| Currency | varchar(3) | Defaults to USD |
| Status | enum/string | Draft, Active, PendingRenewal, Expired, Terminated |
| CreatedAtUtc | timestamptz | Required |
| UpdatedAtUtc | timestamptz | Nullable |

## DocumentMetadata

| Column | Type | Notes |
| --- | --- | --- |
| Id | uuid | Primary key |
| ContractId | uuid | FK to Contracts |
| FileName | varchar(260) | Required |
| ContentType | varchar(120) | Required |
| StorageUri | text | Required |
| UploadedById | uuid | FK to Users |
| UploadedAtUtc | timestamptz | Required |

## RiskAssessments

| Column | Type | Notes |
| --- | --- | --- |
| Id | uuid | Primary key |
| VendorId | uuid | FK to Vendors |
| ContractId | uuid | Optional FK to Contracts |
| Score | int | 0-100 |
| Tier | enum/string | Low, Medium, High, Critical |
| Rationale | text | Rules explanation |
| CalculatedById | uuid | FK to Users |
| CalculatedAtUtc | timestamptz | Required |

## ApprovalRequests

| Column | Type | Notes |
| --- | --- | --- |
| Id | uuid | Primary key |
| ContractId | uuid | FK to Contracts |
| RequestedById | uuid | FK to Users |
| AssignedReviewerId | uuid | FK to Users |
| Status | enum/string | Draft, Submitted, Approved, Rejected, ChangesRequested |
| SubmittedAtUtc | timestamptz | Nullable |
| DecidedAtUtc | timestamptz | Nullable |
| DecisionComment | text | Nullable |
| CreatedAtUtc | timestamptz | Required |
| UpdatedAtUtc | timestamptz | Nullable |

## ApprovalComments

| Column | Type | Notes |
| --- | --- | --- |
| Id | uuid | Primary key |
| ApprovalRequestId | uuid | FK to ApprovalRequests |
| AuthorId | uuid | FK to Users |
| Comment | text | Required |
| CreatedAtUtc | timestamptz | Required |

## Notifications

| Column | Type | Notes |
| --- | --- | --- |
| Id | uuid | Primary key |
| UserId | uuid | FK to Users |
| Type | enum/string | RenewalDue, ApprovalAssigned, ApprovalDecision |
| Title | varchar(200) | Required |
| Message | text | Required |
| IsRead | boolean | Required |
| CreatedAtUtc | timestamptz | Required |

## AuditLogs

| Column | Type | Notes |
| --- | --- | --- |
| Id | uuid | Primary key |
| ActorUserId | uuid | Optional FK to Users |
| Action | varchar(120) | Login, Create, Update, Delete, ApprovalDecision, RoleChange, RiskCalculation |
| EntityName | varchar(120) | Required |
| EntityId | uuid | Nullable |
| Details | text | Required |
| IpAddress | varchar(80) | Nullable |
| CreatedAtUtc | timestamptz | Required |
