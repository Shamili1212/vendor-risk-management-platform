# REST API Design

Base URL: `/api`

All protected endpoints require:

```http
Authorization: Bearer <jwt>
```

## Authentication

| Method | Endpoint | Roles | Description |
| --- | --- | --- | --- |
| POST | `/auth/login` | Public | Authenticate and return JWT |
| GET | `/auth/me` | Authenticated | Return current user profile |

## Vendors

| Method | Endpoint | Roles | Description |
| --- | --- | --- | --- |
| GET | `/vendors` | All roles | Search/filter vendors |
| GET | `/vendors/{id}` | All roles | Get vendor details |
| POST | `/vendors` | Admin, Procurement Manager | Create vendor |
| PUT | `/vendors/{id}` | Admin, Procurement Manager | Update vendor |
| DELETE | `/vendors/{id}` | Admin | Delete vendor |
| POST | `/vendors/{id}/risk-assessments` | Admin, Procurement Manager | Calculate risk score |

Query filters:

- `search`
- `status`
- `category`
- `ownerId`
- `riskTier`

## Contracts

| Method | Endpoint | Roles | Description |
| --- | --- | --- | --- |
| GET | `/contracts` | All roles | Search/filter contracts |
| GET | `/contracts/{id}` | All roles | Get contract details |
| POST | `/contracts` | Admin, Procurement Manager | Create contract |
| PUT | `/contracts/{id}` | Admin, Procurement Manager | Update contract |
| DELETE | `/contracts/{id}` | Admin | Delete contract |
| POST | `/contracts/{id}/documents` | Admin, Procurement Manager | Add document metadata |

Query filters:

- `vendorId`
- `status`
- `renewalBefore`
- `renewalAfter`

## Approval Workflow

| Method | Endpoint | Roles | Description |
| --- | --- | --- | --- |
| GET | `/approvals` | Admin, Procurement Manager, Reviewer, Auditor | List approval requests |
| GET | `/approvals/{id}` | Admin, Procurement Manager, Reviewer, Auditor | Get approval details |
| POST | `/approvals` | Admin, Procurement Manager | Create approval request |
| POST | `/approvals/{id}/submit` | Admin, Procurement Manager | Submit approval |
| POST | `/approvals/{id}/approve` | Reviewer, Admin | Approve request |
| POST | `/approvals/{id}/reject` | Reviewer, Admin | Reject request |
| POST | `/approvals/{id}/request-changes` | Reviewer, Admin | Request changes |
| POST | `/approvals/{id}/comments` | Authenticated | Add comment |

## Dashboard

| Method | Endpoint | Roles | Description |
| --- | --- | --- | --- |
| GET | `/dashboard/summary` | All roles | KPI cards and chart data |

Response contains:

- total vendors
- high-risk vendors
- contracts expiring in 30/60/90 days
- pending approvals
- risk tier distribution
- recent audit events

## Notifications

| Method | Endpoint | Roles | Description |
| --- | --- | --- | --- |
| GET | `/notifications` | Authenticated | List current user's notifications |
| POST | `/notifications/{id}/read` | Authenticated | Mark notification as read |

## Audit Logs

| Method | Endpoint | Roles | Description |
| --- | --- | --- | --- |
| GET | `/audit-logs` | Admin, Auditor | Search audit logs |

Query filters:

- `actorUserId`
- `action`
- `entityName`
- `from`
- `to`

## Validation and Errors

The API returns RFC 7807-style problem responses where practical.

Common status codes:

- `400 Bad Request`: validation or illegal workflow transition
- `401 Unauthorized`: missing/invalid token
- `403 Forbidden`: insufficient role
- `404 Not Found`: missing resource
- `409 Conflict`: duplicate or conflicting state
- `500 Internal Server Error`: unexpected server failure
