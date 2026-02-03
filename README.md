# TourPlanner — SS26 SWEN2 Team 3

Tour planning web application: plan, track, and manage hiking, cycling, running and vacation tours.

**Stack**: React (Vite + TanStack) · C# ASP.NET 8 · PostgreSQL · Docker

**Live**: https://tourplanner.w11core.cc · **API**: https://tourplanner.w11core.cc/api/v1 · **Swagger**: https://tourplanner.w11core.cc/api/v1/swagger

PR previews are deployed automatically to `https://pr-{N}.tourplanner.w11core.cc` when a pull request is opened.

## Quick Start

```bash
# 1. Clone and copy env
cp .env.example .env

# 2. Start everything (Postgres + backend + frontend with hot reload)
docker compose up
```

- Frontend: http://localhost:5173
- API:      http://localhost:5173/api/v1
- Swagger:  http://localhost:5173/api/v1/swagger

That's it. No local .NET or Node required.

On first start the backend automatically runs EF Core migrations and seeds demo data:

| Field    | Value                  |
|----------|------------------------|
| Email    | `demo@tourplanner.dev` |
| Password | `Demo123!`             |

The seed includes 4 tours (hiking, cycling, running, car) and 7 tour logs — enough to see the full UI immediately.

## Developer Setup Guide

### Prerequisites

| Tool | Version | Install |
|---|---|---|
| Docker Desktop | latest | https://docs.docker.com/get-docker/ |
| Git | any | https://git-scm.com |
| Node.js + pnpm | Node 20+, pnpm 9+ | `npm i -g pnpm` |
| .NET SDK | 8.0 | https://dotnet.microsoft.com/download/dotnet/8 |

Node and .NET are only needed if you work outside Docker (migrations, IDE tooling). For running the app, Docker is sufficient.

### First-time setup

```bash
# Clone
git clone https://github.com/RickyRAV/ss26swen2team3.git
cd ss26swen2team3

# Copy environment variables
cp .env.example .env
# Edit .env if needed (defaults work for local dev)

# Install frontend deps (needed for IDE autocomplete and type checking outside Docker)
cd apps/web && pnpm install && cd ../..

# Start the full stack
docker compose up
```

On first run Docker will build the images — this takes ~2 minutes. Subsequent starts are fast.

### Working on the frontend

The frontend runs with Vite hot reload inside Docker, so changes are reflected instantly in the browser. You can also run it locally:

```bash
cd apps/web
pnpm dev
```

By default the frontend uses relative `/api` requests. In Docker, Vite proxies those requests to the backend container. Outside Docker, `pnpm dev` proxies `/api` to `http://localhost:5000` unless you override `API_PROXY_TARGET` or `VITE_API_BASE_URL`.

**Key directories:**

```
apps/web/src/
├── api/          # API client functions — never import these in views directly
├── models/       # TypeScript interfaces (Model layer)
├── viewmodels/   # TanStack Query/Form hooks (ViewModel layer)
├── views/        # React components (View layer)
├── stores/       # Zustand — auth state only
└── components/   # Shared UI components
```

MVVM rule: **Views only import from `viewmodels/`**, never from `api/` directly.

### Working on the backend

The backend runs with `dotnet watch` inside Docker. For local development (e.g. to run migrations):

```bash
cd apps/api
cp TourPlanner.Api/appsettings.Development.json.example TourPlanner.Api/appsettings.Development.json
# Edit appsettings.Development.json with your local DB connection string

dotnet restore TourPlanner.sln
dotnet run --project TourPlanner.Api
```

**Layer rules (strictly graded):**

```
TourPlanner.Api  →  controllers, DTOs, validators, middleware
TourPlanner.BL   →  services, business logic, BL exceptions
TourPlanner.DAL  →  repositories, EF Core, DAL exceptions
TourPlanner.Models → domain entities, enums
```

Controllers never touch repositories. BL never touches EF Core directly.

### Database migrations

Always run these from the `apps/api/` directory:

```bash
# Create a new migration
dotnet ef migrations add <MigrationName> \
  --project TourPlanner.DAL \
  --startup-project TourPlanner.Api

# Apply migrations to local DB
dotnet ef database update \
  --project TourPlanner.DAL \
  --startup-project TourPlanner.Api
```

Migrations are applied automatically on backend startup (both Docker and local).

### Running Tests

```bash
# Backend (NUnit) — from repo root
cd apps/api
dotnet restore TourPlanner.sln
dotnet build TourPlanner.sln --no-restore --no-incremental
dotnet test TourPlanner.sln

# Frontend (typecheck + build)
cd apps/web
pnpm typecheck
pnpm build
```

> **macOS note:** Always run `dotnet restore` before `dotnet build`. Running them together can hit a file-lock race condition with `GlobalUsings`.

### Common issues

| Problem | Fix |
|---|---|
| `docker compose up` fails on first run | Run it again — DB health check occasionally races |
| Frontend shows "Failed to fetch" | Backend isn't up yet, wait ~10s then refresh |
| `dotnet build` fails with missing GlobalUsings | Run `dotnet restore` first, then `dotnet build --no-restore` |
| Port 5432 already in use | Stop local Postgres: `brew services stop postgresql` |
| `routeTree.gen.ts` not found | Run `pnpm build` once in `apps/web/` to generate it |

### Useful commands

```bash
# Tail backend logs
docker compose logs -f backend

# Open psql in the running DB container
docker compose exec db psql -U tourplanner -d tourplanner

# Rebuild a single service after Dockerfile change
docker compose up --build backend

# Stop everything and remove volumes (full reset)
docker compose down -v
```

## Local Development (without Docker)

```bash
# Backend
cd apps/api
cp TourPlanner.Api/appsettings.Development.json.example TourPlanner.Api/appsettings.Development.json
dotnet run --project TourPlanner.Api

# Frontend
cd apps/web
pnpm install
pnpm dev
```

## Project Resources

- Live app: [tourplanner.w11core.cc](https://tourplanner.w11core.cc)
- Issue board: [plane.w11core.cc](https://plane.w11core.cc)
- GitHub: [RickyRAV/ss26swen2team3](https://github.com/RickyRAV/ss26swen2team3)
- Main branch: `main`
- Project documents: `Docs/`

## Team Workflow

All implementation work should be tracked on the team board before code is started. Each code change should be linked to an issue, developed in its own branch, and merged through a pull request.

The standard flow is:

1. Create or assign an issue on the board.
2. Create a branch from `main`.
3. Implement the change and keep commits focused.
4. Open a pull request to `main`.
5. Request review, address feedback, and merge once approved.

## Branch Naming Convention

Branches should follow this format:

```text
<issue-type>/<initials>/<ticket-number-or-noissue>-<short-description>
```

### Field Breakdown

- `<issue-type>`: The kind of work being done.
- `<initials>`: The contributor's initials.
- `<ticket-number-or-noissue>`: The issue identifier from the board, or `NOISSUE` if the work is not tracked on the board.
- `<short-description>`: A brief, lowercase, hyphen-separated summary of the task.

### Recommended Issue Types

Use one of the following branch type prefixes whenever possible:

- `feature` for new functionality
- `fix` for bug fixes
- `chore` for maintenance or housekeeping work
- `docs` for documentation-only changes
- `refactor` for structural code improvements without behavior changes
- `test` for adding or updating tests
- `hotfix` for urgent production fixes

### Examples

```text
feature/rr/SWEN2-43-small-description
fix/rr/SWEN2-18-login-validation
chore/rr/SWEN2-27-update-dependencies
chore/rr/NOISSUE-repository-cleanup
docs/rr/SWEN2-12-improve-readme
refactor/rr/SWEN2-51-clean-up-service-layer
test/rr/SWEN2-62-add-tour-import-tests
```

### Example Explained

```text
feature/rr/SWEN2-43-small-description
```

- `feature`: This branch introduces new functionality.
- `rr`: These are the contributor's initials.
- `SWEN2-43`: This is the issue key from the board.
- `small-description`: This is a short summary of the change.

If no board ticket exists for the work, replace the ticket key with `NOISSUE`.

```text
chore/rr/NOISSUE-repository-cleanup
```

- `chore`: This branch is for maintenance work.
- `rr`: These are the contributor's initials.
- `NOISSUE`: This indicates that no board ticket exists for this task.
- `repository-cleanup`: This is a short summary of the change.

## Pull Request Guidelines

Each pull request should:

- reference the related board ticket
- have a clear and specific title
- describe what changed and why
- stay focused on a single concern whenever possible
- be reviewed before merging

If a pull request closes an issue, mention the ticket in the description so the connection is visible to the team.

## Working With Issues

Before starting work:

- verify that an issue exists on the board when the work should be tracked there
- assign the issue if ownership is needed
- use `NOISSUE` only when there is intentionally no board ticket for the task
- make sure the branch name matches the issue key exactly when a ticket exists

While working:

- keep the branch up to date with `main`
- use small, understandable commits
- update the issue status on the board as progress changes

When finishing:

- open a pull request against `main`
- request review from the team
- merge only after feedback is addressed

## Documentation

Additional project material is stored in `Docs/`.
