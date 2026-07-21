# AI Workspace

A collaborative project management platform integrated with AI capabilities, built with a modern **.NET 8 + React 19** stack. Teams can manage projects and tasks with role-based access, session management, and multi-language support — all in one workspace.

> 🚧 This project is currently under active development.
> Features, APIs, UI, and architecture may change over time.

## ✨ Features

- **Team Collaboration** — Create teams, manage members with role-based access (Admin, CoAdmin, Member)
- **Project Management** — Organize work into projects with public/private visibility
- **Task Tracking** — Full task lifecycle (ToDo → Doing → Done) with priority, status, and assignment
- **Image Upload** — Upload images to ImageKit cloud storage with validation
- **Session Management** — View and revoke active login sessions across devices
- **JWT Authentication** — Secure login with HttpOnly cookies, refresh token rotation, and absolute expiry
- **Role-Based Authorization** — Granular permission control via MediatR pipeline behaviors
- **Multi-Language UI** — English and Vietnamese (i18n) with per-user language preference
- **Dark / Light Theme** — Theme toggle persisted across sessions
- **Dashboard** — At-a-glance overview with stats, recent tasks, task breakdown, and team summaries
- **Rate Limiting** — Brute-force protection on login endpoint
- **Background Cleanup** — Automatic cleanup of expired tokens and orphaned images

## 🧱 Tech Stack

### Backend

| Technology                        | Purpose                             |
| --------------------------------- | ----------------------------------- |
| **.NET 8** (C#)                   | Runtime / Language                  |
| **ASP.NET Core Web API**          | REST API layer                      |
| **Entity Framework Core 8**       | ORM (SQL Server)                    |
| **MediatR**                       | CQRS / mediator pattern             |
| **FluentValidation**              | Input validation                    |
| **JWT Bearer Authentication**     | Token-based auth (HttpOnly cookies) |
| **Swashbuckle**                   | Swagger / OpenAPI docs              |
| **System.Threading.RateLimiting** | Rate limiting (login endpoint)      |
| **ImageKit**                      | Cloud image storage                 |

### Frontend

| Technology               | Version | Purpose                    |
| ------------------------ | ------- | -------------------------- |
| **React**                | ^19.2   | UI library                 |
| **TypeScript**           | ~6.0    | Type-safe JavaScript       |
| **Vite**                 | ^8.1    | Build tool / dev server    |
| **Ant Design**           | ^6.5    | UI component library       |
| **TanStack React Query** | ^5.101  | Server state / caching     |
| **React Router**         | ^7.18   | Client-side routing        |
| **Axios**                | ^1.18   | HTTP client (auto-refresh) |
| **i18next**              | ^26.3   | Internationalization       |
| **React Context**        | —       | Theming (dark/light)       |
| **oxlint**               | ^1.69   | Rust-based linter          |

## 📁 Project Structure

```
AI-Workspace/
├── Backend/                     # .NET 8 solution (Clean Architecture)
│   ├── AIWorkspace.sln
│   ├── dockerfile
│   ├── script/                 # Helper scripts (build, run, migrate)
│   │   ├── run.sh
│   │   ├── run-with-seed.sh
│   │   ├── add-migration.sh
│   │   ├── update-database.sh
│   │   ├── list-migration.sh
│   │   └── clean-and-build.sh
│   └── src/
│       ├── Domain/             # Entities, Enums (pure business logic)
│       ├── Application/        # CQRS commands/queries, interfaces, behaviors
│       ├── Infrastructure/     # EF Core DbContext, migrations, services, settings
│       └── Api/               # Controllers, middleware, filters, DI registration
│
├── Frontend/                    # React + TypeScript + Vite app
│   ├── src/
│   │   ├── api/                # Axios API clients (auth, team, project, user, summary)
│   │   ├── components/
│   │   │   ├── ui/             # Generic UI (AICard, AIList, AIModal, AIPagination...)
│   │   │   ├── business/       # task/, team/, visibility/, project/
│   │   │   └── layout/         # AISidebar (desktop sider / mobile drawer)
│   │   ├── constants/          # Routes, endpoints, theme tokens
│   │   ├── contexts/           # ThemeContext (dark/light)
│   │   ├── hooks/
│   │   │   ├── api/            # TanStack Query hooks (useAuth, useTeam, useProject, useSummary, useUser)
│   │   │   └── *.hook.ts       # useSearch, useDebounce, useGetCacheData, useTheme, useLanguage
│   │   ├── i18n/               # Locale files (en, vi)
│   │   ├── layouts/            # AppLayout (sidebar + header + breadcrumb + content)
│   │   ├── pages/              # HomePage, LoginPage, OverviewPage, ProfilePage,
│   │   │                       #   TeamPage, TeamDetailPage, ProjectPage, ProjectDetailPage
│   │   ├── router/             # Route definitions + ProtectedRoute guard
│   │   ├── types/              # TypeScript definitions (auth, team, project, task, summary, user, common)
│   │   ├── utils/              # Date/error/common/image/userAgent utilities
│   │   ├── App.tsx             # Root component (providers, theme, query client)
│   │   ├── main.tsx            # Application entry point
│   │   └── index.css           # Global styles
│   ├── dist/                   # Production build output
│   └── vite.config.ts
├── .gitignore
└── README.md
```

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) v20+
- [SQL Server](https://www.microsoft.com/en-us/sql-server/) (LocalDB, Docker, or remote)

### 1. Configure Backend

Set up `Backend/src/Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AIWorkspace;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "AuthSetting": {
    "JwtKey": "your-secure-key-at-least-32-chars-long",
    "Issuer": "AiWorkspace",
    "Audience": "AiWorkspaceClients",
    "AccessTokenMinutes": 15,
    "RefreshTokenDays": 7
  },
  "UploadSetting": {
    "PublicKey": "PublicKey",
    "PrivateKey": "PrivateKey",
    "UrlEndpoint": "UrlEndpoint"
  },
  "RateLimitSetting": {
    "PermitLimit": 5,
    "WindowMinutes": 1
  },
  "FrontendSetting": {
    "Url": "http://localhost:5173"
  }
}
```

### 2. Run the Backend

```bash
cd Backend
dotnet restore AIWorkspace.sln
dotnet run --project src/Api
# With seed data: dotnet run --project src/Api --seed
```

The API starts on `http://localhost:5157`. Swagger UI at `/swagger` (Development mode only).

### 3. Run the Frontend

```bash
cd Frontend
npm install
npm run dev
```

The dev server starts on `http://localhost:5173` and sends API requests to `http://localhost:5157/api`.
Override the backend URL via `VITE_API_BASE_URL` in `.env`.

### Docker (Backend Only)

```bash
docker build -f Backend/dockerfile -t ai-workspace-api .
docker run -p 8080:80 ai-workspace-api
```

## 📡 API Summary

| Area      | Method | Endpoint                                          | Auth Required |
| --------- | ------ | ------------------------------------------------- | ------------- |
| Auth      | POST   | `/api/auth/login`                                 | No            |
| Auth      | POST   | `/api/auth/register`                              | No            |
| Auth      | POST   | `/api/auth/refresh`                               | No            |
| Auth      | POST   | `/api/auth/logout`                                | No            |
| Auth      | GET    | `/api/auth/me`                                    | Yes           |
| Auth      | GET    | `/api/auth/sessions`                              | Yes           |
| Auth      | DELETE | `/api/auth/sessions`                              | Yes           |
| Auth      | DELETE | `/api/auth/sessions/{deviceId}`                   | Yes           |
| Teams     | GET    | `/api/teams`                                      | Yes           |
| Teams     | POST   | `/api/teams`                                      | Yes           |
| Teams     | GET    | `/api/teams/{id}`                                 | Yes           |
| Teams     | PUT    | `/api/teams/{id}`                                 | Yes           |
| Teams     | DELETE | `/api/teams/{id}`                                 | Yes           |
| Teams     | GET    | `/api/teams/{id}/members`                         | Yes           |
| Teams     | POST   | `/api/teams/{id}/members`                         | Yes           |
| Teams     | PUT    | `/api/teams/{id}/members/{memberId}`              | Yes           |
| Teams     | DELETE | `/api/teams/{id}/members/{memberId}`              | Yes           |
| Teams     | GET    | `/api/teams/{id}/available-members`               | Yes           |
| Teams     | GET    | `/api/teams/{teamId}/projects`                    | Yes           |
| Projects  | GET    | `/api/projects`                                   | Yes           |
| Projects  | POST   | `/api/projects`                                   | Yes           |
| Projects  | GET    | `/api/projects/{id}`                              | Yes           |
| Projects  | PUT    | `/api/projects/{id}`                              | Yes           |
| Projects  | GET    | `/api/projects/{slug}`                            | Yes           |
| Projects  | GET    | `/api/projects/{projectId}/members`               | Yes           |
| Tasks     | POST   | `/api/projects/{projectId}/tasks`                 | Yes           |
| Tasks     | GET    | `/api/projects/{projectId}/tasks`                 | Yes           |
| Tasks     | GET    | `/api/projects/{projectId}/tasks/me`              | Yes           |
| Tasks     | PATCH  | `/api/projects/{projectId}/tasks/{taskId}/status` | Yes           |
| Users     | PUT    | `/api/users`                                      | Yes           |
| Users     | PATCH  | `/api/users/password`                             | Yes           |
| Upload    | POST   | `/api/uploads/picture`                            | Yes           |
| Dashboard | GET    | `/api/summaries`                                  | Yes           |

> See `Backend/README.md` for full endpoint descriptions and `Frontend/README.md` for the client-side routing table.

## 🔒 Security

- **Passwords**: Hashed with PBKDF2 via ASP.NET Core Identity `PasswordHasher` (wrapped in `PasswordHasherHelper`)
- **JWT**: Access tokens (15 min) + opaque refresh tokens with **absolute expiry** (7 days from login)
- **Refresh Token Rotation**: Old token revoked on each refresh — prevents reuse attacks
- **Cookies**: HttpOnly + Secure + SameSite=None (cross-origin) — not accessible via JavaScript
- **Rate Limiting**: Login endpoint — 5 requests per minute
- **Authorization**: MediatR pipeline behaviors (`TeamRoleBehavior`, `ProjectRoleBehavior`)
- **Sensitive Data Logging**: Disabled in production; passwords/tokens masked before logging
- **Background Cleanup**: Expired tokens cleaned every hour, orphaned images every 24 hours

## 🧪 Scripts

### Frontend

| Command           | Description                   |
| ----------------- | ----------------------------- |
| `npm run dev`     | Start Vite dev server         |
| `npm run build`   | TypeScript check + Vite build |
| `npm run preview` | Preview production build      |
| `npm run lint`    | Run oxlint linter             |

### Backend

| Command                                       | Description             |
| --------------------------------------------- | ----------------------- |
| `dotnet build Backend/AIWorkspace.sln`        | Build solution          |
| `dotnet run --project Backend/src/Api`        | Run API server          |
| `dotnet run --project Backend/src/Api --seed` | Run with seed data      |
| `bash Backend/script/run.sh`                  | Quick run script        |
| `bash Backend/script/run-with-seed.sh`        | Run with seed data      |
| `bash Backend/script/add-migration.sh <Name>` | Add EF Core migration   |
| `bash Backend/script/update-database.sh`      | Apply DB migrations     |
| `bash Backend/script/list-migration.sh`       | List applied migrations |
| `bash Backend/script/clean-and-build.sh`      | Clean & build solution  |

## 📄 License

This project is for demonstration/educational purposes.
