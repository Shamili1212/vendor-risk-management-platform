# Vendor Risk & Contract Renewal Management Platform - Architecture

## Final Folder Structure

```text
vendor-risk-management-platform/
  .github/workflows/ci.yml
  backend/
    src/VendorRisk.Api/
      Controllers/
      Data/
      Domain/Entities/
      Domain/Enums/
      Dtos/
      Middleware/
      Services/
      Program.cs
      VendorRisk.Api.csproj
      appsettings.json
    tests/VendorRisk.Api.Tests/
      VendorRisk.Api.Tests.csproj
  frontend/
    src/
      components/
      lib/
      pages/
      test/
      App.tsx
      main.tsx
      index.css
    package.json
    tailwind.config.ts
    vite.config.ts
  risk-engine/
    src/risk_engine.c
    tests/risk_engine_tests.c
    Makefile
  docs/
    api-design.md
    architecture.md
    database-schema.md
  docker-compose.yml
  .env.example
  README.md
```

## System Overview

The platform is a production-style enterprise application for managing third-party vendors, contracts, renewal deadlines, approval workflows, risk assessments, notifications, document metadata, and audit history.

The application is split into three deployable concerns:

1. **React + TypeScript frontend**
   - Vite app with Tailwind CSS.
   - Role-aware dashboard and workflow pages.
   - API client layer isolates HTTP details from UI components.
   - Vitest and React Testing Library cover UI behavior.

2. **ASP.NET Core Web API backend**
   - REST API secured by JWT bearer authentication.
   - Role-based authorization for Admin, Procurement Manager, Reviewer, and Auditor.
   - Entity Framework Core maps domain entities to PostgreSQL.
   - Swagger/OpenAPI documents all endpoints.
   - xUnit tests cover services and controller-level behavior.

3. **C risk engine**
   - Rules-based CLI that calculates normalized risk scores.
   - Backend invokes the compiled executable through a small adapter service.
   - This keeps the C boundary explicit, testable, and easy to explain in interviews.

## Backend Layers

```text
Controllers -> Services -> DbContext/Entities -> PostgreSQL
                 |
                 +-> C risk engine process adapter
```

- **Controllers** validate API intent, apply authorization, and return RESTful responses.
- **Services** contain business logic such as approval transitions, audit logging, notification creation, and risk calculations.
- **Data** contains `AppDbContext` and seed data.
- **Domain** contains entities and enums.
- **Dtos** contains request/response contracts.
- **Middleware** centralizes exception handling.

## Security Model

- JWT authentication is configured in the API.
- Role-based policies protect endpoints:
  - `AdminOnly`
  - `ProcurementOrAdmin`
  - `ReviewerOrAdmin`
  - `AuditorRead`
- Passwords are represented as hashes in the seed data and DTOs. For portfolio/local demo purposes, a simple deterministic demo hash service is provided; production deployment should replace it with ASP.NET Core Identity or a hardened password hasher.

## Risk Scoring Model

Risk is calculated from:

- Contract value
- Vendor criticality
- Compliance status
- Incident count
- Renewal urgency

The C engine returns:

- Numeric score from 0 to 100
- Risk tier: Low, Medium, High, Critical
- Human-readable rationale

The backend persists risk assessment results and writes an audit log entry for each calculation.

## Notifications

Notifications are generated when:

- A contract renewal date is within a configured window.
- An approval request is submitted or assigned.
- A reviewer makes a decision.

The current implementation stores notifications in PostgreSQL and exposes read/mark-read endpoints. A later production deployment could add background jobs and email integration.

## DevOps

- Docker Compose starts PostgreSQL, backend, frontend, and a risk-engine build stage for local development.
- GitHub Actions runs backend tests, frontend tests, and C engine tests/build.
- Environment configuration is driven through `.env.example`.

## Incremental Implementation Plan

1. **Commit 1: Project foundation**
   - Add final folder structure, documentation, `.gitignore`, `.env.example`, Docker Compose baseline.

2. **Commit 2: Backend domain and database**
   - Add ASP.NET Core Web API project.
   - Add entities, enums, DbContext, seed data, EF Core setup.

3. **Commit 3: Authentication and authorization**
   - Add JWT auth service, login endpoint, role policies, exception middleware.

4. **Commit 4: Vendor and contract APIs**
   - Add vendor CRUD with search/filtering.
   - Add contract CRUD linked to vendors and document metadata.

5. **Commit 5: C risk engine integration**
   - Add C CLI, Makefile, backend adapter, risk assessment endpoint, tests.

6. **Commit 6: Approval workflow and audit logs**
   - Add submit/approve/reject/request-changes transitions.
   - Add comments, reviewer assignment, timestamps, audit logs.

7. **Commit 7: Dashboard and notifications**
   - Add KPI aggregation, risk distribution, renewal alerts, pending approval notifications.

8. **Commit 8: React frontend**
   - Add Vite React TypeScript app, Tailwind, pages, reusable components, API client.

9. **Commit 9: Tests and CI/CD**
   - Add xUnit, Vitest/RTL, C tests, GitHub Actions pipeline.

10. **Commit 10: README and deployment notes**
    - Add demo credentials, local commands, architecture notes, screenshots placeholders, deployment instructions.
