# Vendor Risk & Contract Renewal Management Platform

Production-style portfolio project for managing vendors, contracts, renewal deadlines, risk assessments, approvals, notifications, and audit logs.

## Screenshots

Add screenshots here after running the app locally:

- `docs/screenshots/dashboard.png`
- `docs/screenshots/vendors.png`
- `docs/screenshots/approval-workflow.png`
- `docs/screenshots/swagger.png`

## Stack

- Frontend: React, TypeScript, Vite, Tailwind CSS, Vitest, React Testing Library
- Backend: C#, ASP.NET Core Web API, Entity Framework Core, PostgreSQL, JWT, Swagger/OpenAPI, xUnit
- Risk engine: C CLI integrated through the C# backend
- DevOps: Docker Compose, GitHub Actions CI

## Architecture

```text
React UI -> ASP.NET Core REST API -> EF Core -> PostgreSQL
                          |
                          +-> C risk engine CLI
```

Key backend boundaries:

- Controllers expose REST resources and enforce authorization.
- Services own workflow, audit, notification, token, and risk-scoring logic.
- Entity Framework Core maps the domain model to PostgreSQL.
- The C risk engine returns JSON with score, tier, and rationale.

Full documentation:

- [Architecture](docs/architecture.md)
- [Database schema](docs/database-schema.md)
- [API design](docs/api-design.md)

## Demo Credentials

| Role | Email | Password |
| --- | --- | --- |
| Admin | `admin@demo.local` | `Admin123!` |
| Procurement Manager | `procurement@demo.local` | `Procure123!` |
| Reviewer | `reviewer@demo.local` | `Review123!` |
| Auditor | `auditor@demo.local` | `Audit123!` |

## Run Locally With Docker Compose

Build and start everything:

```bash
docker compose up --build
```

Open:

- Frontend: <http://localhost:5173>
- API Swagger: <http://localhost:5000/swagger>
- PostgreSQL: `localhost:5433`

Docker Compose configures the API to create the local development schema on startup. For production-style migrations, use the EF commands below.

## Run Backend Locally

Start PostgreSQL:

```bash
docker compose up postgres redis
```

Run the API:

```bash
dotnet run --project backend/src/VendorRisk.Api/VendorRisk.Api.csproj
```

Create and apply EF Core migrations:

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate --project backend/src/VendorRisk.Api --startup-project backend/src/VendorRisk.Api
dotnet ef database update --project backend/src/VendorRisk.Api --startup-project backend/src/VendorRisk.Api
```

## Run Frontend Locally

On Windows PowerShell, use `npm.cmd` if script execution blocks `npm.ps1`.

```bash
cd frontend
npm install
npm run dev
```

## Build C Risk Engine

Linux/macOS or a GCC-enabled shell:

```bash
cd risk-engine
make test
make all
```

The backend reads `RiskEngine:Path`. If the compiled CLI is unavailable, it uses a managed fallback with the same scoring rules so local development still works.

## Test Commands

Backend:

```bash
dotnet test backend/tests/VendorRisk.Api.Tests/VendorRisk.Api.Tests.csproj
```

Frontend:

```bash
cd frontend
npm test
```

C risk engine:

```bash
cd risk-engine
make test
```

## REST API Highlights

- `POST /api/auth/login`
- `GET /api/dashboard/summary`
- `GET/POST/PUT/DELETE /api/vendors`
- `POST /api/vendors/{id}/risk-assessments`
- `GET/POST/PUT/DELETE /api/contracts`
- `POST /api/contracts/{id}/documents`
- `GET/POST /api/approvals`
- `POST /api/approvals/{id}/submit`
- `POST /api/approvals/{id}/approve`
- `POST /api/approvals/{id}/reject`
- `POST /api/approvals/{id}/request-changes`
- `GET /api/notifications`
- `GET /api/audit-logs`

## Role Model

- Admin: manages users, settings, destructive actions, audit logs.
- Procurement Manager: manages vendors/contracts and submits approvals.
- Reviewer: reviews assigned requests and records decisions.
- Auditor: read-only access to records and audit history.

## CI/CD

GitHub Actions runs:

- Backend restore and xUnit tests
- Frontend install, build, and Vitest tests
- C risk engine build and tests

Pipeline file: [.github/workflows/ci.yml](.github/workflows/ci.yml)

## Deployment Notes

For a production deployment:

- Replace the demo password hasher with ASP.NET Core Identity or a hardened password hashing service.
- Store JWT keys and connection strings in a secret manager.
- Run EF migrations as a release step.
- Add HTTPS termination and strict CORS origins.
- Replace stored document metadata URIs with an object storage integration.
- Add background jobs for renewal notification generation and email delivery.
