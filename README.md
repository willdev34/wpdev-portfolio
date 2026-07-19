# WPDev Portfolio

**[🇧🇷 Ler em Português](./README.pt-br.md)**

A professional portfolio for a Senior Full Stack Developer, built with an editorial, minimalist visual identity. Live projects, a magazine-style blog, an interactive timeline, and a full admin panel, all backed by a REST API built with Clean Architecture.

**Live:** [wpdev-portfolio-web.onrender.com](https://wpdev-portfolio-web.onrender.com)
**API:** [wpdev-portfolio-api.onrender.com](https://wpdev-portfolio-api.onrender.com) · [Swagger](https://wpdev-portfolio-api.onrender.com/swagger)

---

## Stack

| Layer | Technology |
|---|---|
| API | .NET 8, ASP.NET Core Web API |
| Frontend | Blazor WebAssembly |
| Database | PostgreSQL (Docker locally, Supabase in production) |
| Architecture | Clean Architecture, CQRS via MediatR |
| Validation / Mapping | FluentValidation, AutoMapper |
| Auth | ASP.NET Core Identity + JWT |
| Media | Cloudinary |
| Logging | Serilog |
| Testing | xUnit, Moq, coverlet |
| CI/CD | GitHub Actions |
| Hosting | Render (API + Web), Supabase (database) |

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
└── Portfolio.IntegrationTests # WebApplicationFactory + Testcontainers (in progress)
```

Dependencies point inward: `Api` and `Web` depend on `Application` and `Infrastructure`; `Application` depends only on `Domain`. `Infrastructure` implements interfaces defined in `Application`.

## Features

**Public site**
- Editorial home page with a strong hero section
- Project listing with large thumbnails and a detailed case-study page per project
- Interactive timeline
- Magazine-style mini-blog
- "What I'm doing now" section
- Photo gallery
- Minimalist contact form

**Admin panel**
- JWT-authenticated area with full CRUD for Projects, Blog Posts, Timeline events, the Now section, and contact messages
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

Unit tests cover the Application and Domain layers with xUnit and Moq. Coverage is measured with coverlet and enforced in CI: pull requests are blocked if line/statement coverage drops below 80%. Integration tests (`Portfolio.IntegrationTests`) use `WebApplicationFactory` and Testcontainers for PostgreSQL, and are actively being expanded.

## CI/CD

GitHub Actions runs on every push and pull request:

1. **build-backend** — restores, builds, and runs unit tests for the .NET solution
2. **quality-gate** — enforces the 80% coverage threshold, blocking the PR if not met
3. **build-frontend** — builds the Blazor WebAssembly frontend

The `main` branch is protected: it only accepts changes through pull requests that pass all checks. Deploys to Render trigger automatically on merge to `main`.

## Monitoring

- `GET /api/health` checks database connectivity
- [UptimeRobot](https://uptimerobot.com/) pings both the API and the frontend every 5 minutes
- A scheduled GitHub Actions job keeps the Supabase database active on its free tier

## 🔒 Security

Secrets (JWT signing key, database connection string, admin credentials) are provided via environment variables in production and via a git-ignored `appsettings.Development.json` locally. No secret values are committed to this repository.

### Row Level Security (RLS) on Supabase

All tables in the `public` schema have RLS enabled as of July 2026, including business tables (`Projects`, `BlogPosts`, `ContactMessages`, `GalleryImages`, `NowSections`, `TimelineEvents`) and ASP.NET Identity tables (`AspNetUsers`, `AspNetRoles`, etc).

Supabase automatically exposes a REST API (PostgREST) for any public table, accessible through the `anon key`. Without RLS, this API allows direct read/write access, bypassing the application's JWT authentication entirely.

The application connects to PostgreSQL using Supabase's default role (via `ConnectionStrings__DefaultConnection`), which has RLS bypass privilege. Because of this, RLS was enabled on every table without creating any policies: the backend keeps working normally, while Supabase's public REST API now denies access by default.

Reference: [Supabase Advisor](https://supabase.com/docs/guides/database/database-advisors)

## License

© 2026 Will — WPDev. All rights reserved. This is a personal portfolio project; source code is public for demonstration purposes and is not licensed for reuse.

## Contact

Built by Will, WPDev — Senior Full Stack Developer based in Brazil, working with clients in Rio de Janeiro, Fortaleza, and internationally.