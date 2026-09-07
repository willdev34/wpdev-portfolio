# WPDev Portfolio

**[🇧🇷 Ler em Português](./README.pt-br.md)**

A professional portfolio for a Senior Full Stack Developer, built with an editorial, minimalist visual identity. Live projects, a magazine-style blog, an interactive timeline, and a full admin panel, all backed by a REST API built with Clean Architecture.

**Live:** [www.wpdevbr.com](https://www.wpdevbr.com)
**API:** same domain · [Swagger](https://www.wpdevbr.com/swagger)

---

## Stack

| Layer | Technology |
|---|---|
| API | .NET 8, ASP.NET Core Web API |
| Frontend | Blazor WebAssembly |
| Database | PostgreSQL (Docker locally, [Neon](https://neon.tech) in production) |
| Architecture | Clean Architecture, CQRS via MediatR |
| Validation / Mapping | FluentValidation, AutoMapper |
| Auth | ASP.NET Core Identity + JWT |
| Media | Cloudinary |
| Logging | Serilog |
| Testing | xUnit, Moq, coverlet |
| CI/CD | GitHub Actions |
| Hosting | Vercel (API and Web, single project, container runtime) |

## Architecture

The solution follows Clean Architecture with four layers plus a background worker:

```
src/
├── Portfolio.Domain          # Entities, no external dependencies
├── Portfolio.Application     # CQRS (commands/queries via MediatR), DTOs, validators
├── Portfolio.Infrastructure  # EF Core, repositories, external services (Cloudinary, etc.)
├── Portfolio.Api             # REST API, controllers, authentication
├── Portfolio.Web             # Blazor WebAssembly frontend
└── Portfolio.Worker          # Background service (scaffolded, not yet wired to a feature)

tests/
├── Portfolio.UnitTests        # Application and Domain layer tests
└── Portfolio.IntegrationTests # WebApplicationFactory + Testcontainers
```

Dependencies point inward: `Api` and `Web` depend on `Application` and `Infrastructure`; `Application` depends only on `Domain`. `Infrastructure` implements interfaces defined in `Application`.

## Infrastructure

The API and the frontend run as two separate containers ([Vercel Services](https://vercel.com/docs/services)) inside a single Vercel project, sharing one domain and routed by path:

- `/api/*` and `/swagger*` → the `api` container (.NET 8, Kestrel)
- everything else → the `web` container (nginx serving the Blazor WebAssembly build)

Because both services share an origin, the frontend calls the API on the same domain with no CORS needed in production. The frontend's `HttpClient` uses `HostEnvironment.BaseAddress` for this reason, so it works unmodified on the production domain, on Vercel's auto-generated preview URLs, and on any future domain, with zero configuration.

**Environments**

| Environment | Trigger | Database |
|---|---|---|
| Production | Merge to `main` | Neon `production` branch |
| Preview | Push to `develop` | Neon `develop` branch (a copy-on-write branch of `production`) |
| Local | `dotnet run` | Docker Compose PostgreSQL |

Migrations are not applied automatically on every container start. They previously were, which added several seconds to the first request after Vercel scaled the container down; they're now applied deliberately with `dotnet ef database update` against Neon's direct (non-pooled) connection.

Migrated from Render + Supabase to Vercel + Neon in September 2026.

## Features

**Public site**
- Editorial home page with a strong hero section
- Project listing with large thumbnails, a year/status badge per card, and a detailed case-study page per project
- Interactive timeline
- Magazine-style mini-blog
- "What I'm doing now" section
- Photo gallery
- Minimalist contact form

**Admin panel**
- JWT-authenticated area with full CRUD for Projects, Blog Posts, Timeline events, the Now section, and contact messages
- Dashboard with a visual card per section, background image per topic
- Image upload via Cloudinary
- Responsive layout with an animated off-canvas menu on tablet and mobile

## API

8 controllers, REST endpoints for authentication, projects, blog posts, timeline events, the now section, gallery images, contact messages, and a health check. Full endpoint documentation is available via Swagger at `/swagger` when running the API.

## Getting started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/) (for local PostgreSQL, pgAdmin, Redis, and MailDev)

### 1. Clone and start local infrastructure

```bash
git clone https://github.com/willdev34/wpdev-portfolio.git
cd wpdev-portfolio
docker-compose up -d
```

This starts:

| Service | Port | Purpose |
|---|---|---|
| PostgreSQL | `5432` | Application database |
| pgAdmin | `5050` | Database admin UI |
| Redis | `6379` | Provisioned for future caching, not yet used by the application |
| MailDev | `1025` (SMTP) / `1080` (UI) | Captures outgoing emails locally for testing |

### 2. Configure local secrets

The API needs a `src/Portfolio.Api/appsettings.Development.json` file, which is git-ignored and never committed. Create it with your own local values:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=portfolio_dev;Username=wpdev;Password=Dev2024"
  },
  "Jwt": {
    "Secret": "REPLACE_WITH_A_LOCAL_RANDOM_SECRET_AT_LEAST_32_CHARS"
  },
  "Admin": {
    "Email": "admin@wpdev.com",
    "Password": "REPLACE_WITH_A_LOCAL_PASSWORD"
  }
}
```

The application throws a startup exception if any of these are missing, by design, so a misconfigured environment fails fast instead of falling back to a weak default.

### 3. Run the API and the frontend

```bash
dotnet run --project src/Portfolio.Api    # http://localhost:5277 (Swagger opens automatically)
dotnet run --project src/Portfolio.Web    # http://localhost:5237
```

## Testing

```bash
dotnet test
```

Unit tests cover the Application and Domain layers with xUnit and Moq. Coverage is measured with coverlet and enforced in CI: pull requests are blocked if method coverage drops below 80%. Integration tests (`Portfolio.IntegrationTests`) use `WebApplicationFactory` and Testcontainers for PostgreSQL.

## CI/CD

GitHub Actions runs on every push and pull request:

1. **build-backend** — restores, builds, and runs unit tests for the .NET solution
2. **quality-gate** — enforces the 80% coverage threshold, blocking the PR if not met
3. **build-frontend** — builds the Blazor WebAssembly frontend

The `main` branch is protected: it only accepts changes through pull requests that pass all checks. Vercel deploys automatically on every push: to `main` for production, to `develop` for a preview environment backed by its own database branch.

## Monitoring

- `GET /api/health` checks database connectivity
- Both containers scale to zero after a period of inactivity, by design, not a limitation to work around. A `keep-alive` GitHub Actions job, needed on the previous Render/Supabase free tiers, was removed once the infrastructure moved to Vercel + Neon.

## 🔒 Security

Secrets (JWT signing key, database connection string, admin credentials) are provided via environment variables in production and via a git-ignored `appsettings.Development.json` locally. No secret values are committed to this repository.

### Historical note: Row Level Security on Supabase

The production database ran on Supabase until September 2026. All tables in the `public` schema had RLS enabled, including business tables (`Projects`, `BlogPosts`, `ContactMessages`, `GalleryImages`, `NowSections`, `TimelineEvents`) and ASP.NET Identity tables (`AspNetUsers`, `AspNetRoles`, etc), because Supabase automatically exposes a REST API (PostgREST) for any public table via the `anon key`. Without RLS, this API allowed direct read/write access, bypassing the application's JWT authentication entirely. The application connected using Supabase's default role, which has RLS bypass privilege, so RLS was enabled on every table without creating any policies: the backend kept working normally while the public REST API started denying access by default.

This mitigation doesn't apply to the current production database (Neon), which doesn't expose an equivalent public REST surface. Kept here as a record of a real finding and fix.

Reference: [Supabase Advisor](https://supabase.com/docs/guides/database/database-advisors)

## License

© 2026 Will — WPDev. All rights reserved. This is a personal portfolio project; source code is public for demonstration purposes and is not licensed for reuse.

## Contact

Built by Will, WPDev — Senior Full Stack Developer based in Brazil, working with clients in Rio de Janeiro, Fortaleza, and internationally.