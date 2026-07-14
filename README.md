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

| Technology                 | Purpose                    |
| -------------------------- | -------------------------- |
| **React 19**               | UI library                 |
| **TypeScript 6**           | Type-safe JavaScript       |
| **Vite 8**                 | Build tool / dev server    |
| **Ant Design 6**           | UI component library       |
| **TanStack React Query 5** | Server state / caching     |
| **React Router 7**         | Client-side routing        |
| **Axios**                  | HTTP client (auto-refresh) |
| **i18next**                | Internationalization       |
| **React Context**          | Theming (dark/light)       |
| **oxlint**                 | Rust-based linter          |

## 📁 Project Structure

```
AI-Workspace/
├── Backend/                     # .NET 8 solution (Clean Architecture)
│   ├── AIWorkspace.sln
│   ├── src/
│   │   ├── Domain/             # Entities, Enums (pure business logic)
│   │   ├── Application/        # CQRS commands/queries, interfaces, behaviors
│   │   ├── Infrastructure/     # EF Core DbContext, migrations, services, settings
│   │   └── Api/                # Controllers, middleware, filters, DI registration
│   ├── dockerfile
│   └── script/                 # Helper scripts (migration, build, run)
│
├── Frontend/                    # React + TypeScript + Vite app
│   ├── src/
│   │   ├── api/                # Axios API clients (auth, team, project, summary)
│   │   ├── components/
│   │   │   ├── ui/             # Generic UI (AICard, AIList, AIModal, AIPagination...)
│   │   │   ├── business/       # Task status/priority, team role, visibility, project role
│   │   │   └── layout/         # AISidebar (desktop sider / mobile drawer)
│   │   ├── constants/          # Routes, endpoints, theme tokens
│   │   ├── contexts/           # ThemeContext (dark/light)
│   │   ├── hooks/
│   │   │   ├── api/            # TanStack Query hooks (useAuth, useTeam, useProject, useSummary)
│   │   │   └── *.hook.ts       # useSearch, useDebounce, useGetCacheData, useTheme, useLanguage
│   │   ├── i18n/               # Locale files (en, vi)
│   │   ├── layouts/            # AppLayout (sidebar + header + breadcrumb + content)
│   │   ├── pages/              # HomePage, LoginPage, OverviewPage, TeamPage, TeamDetailPage
│   │   ├── router/             # Route definitions + ProtectedRoute guard
│   │   ├── types/              # TypeScript definitions (auth, team, project, task, summary, common)
│   │   ├── utils/              # Date formatting, error handling, common utilities
│   │   ├── App.tsx             # Root component (providers, theme, query client)
│   │   ├── main.tsx            # Application entry point
│   │   └── index.css           # Global styles
│   ├── dist/                   # Production build output
│   └── vite.config.ts
│
├── .clinerules/
│   ├── beRules.md              # Backend coding standards
│   └── feRules.md              # Frontend coding standards
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

### Docker (Backend Only)

```bash
docker build -f Backend/dockerfile -t ai-workspace-api .
docker run -p 8080:80 ai-workspace-api
```

## 📡 API Summary

| Area      | Key Endpoints                                                                   | Auth Required |
| --------- | ------------------------------------------------------------------------------- | ------------- |
| Auth      | `POST /api/auth/login`, `/register`, `/refresh`, `/logout`                      | Mixed         |
| Auth      | `GET /api/auth/me`, `GET/DELETE /api/auth/sessions`                             | Yes           |
| Teams     | `GET/POST /api/teams`, `GET/PUT/DELETE /api/teams/{id}`                         | Yes           |
| Teams     | `GET/POST /api/teams/{id}/members`, `PUT /api/teams/{id}/members/{memberId}`    | Yes           |
| Teams     | `GET /api/teams/{id}/available-members`, `GET /api/teams/{teamId}/projects`     | Yes           |
| Projects  | `GET/POST /api/projects`, `GET /api/projects/{id}`                              | Yes           |
| Projects  | `GET /api/projects/{slug}`, `GET /api/projects/{projectId}/members`             | Yes           |
| Tasks     | `GET /api/projects/{projectId}/tasks`, `GET /api/projects/{projectId}/tasks/me` | Yes           |
| Users     | `PUT /api/users`, `PATCH /api/users/password`                                   | Yes           |
| Upload    | `POST /api/uploads/picture`                                                     | Yes           |
| Dashboard | `GET /api/summaries`                                                            | Yes           |

## 🔒 Security

- **Passwords**: Hashed with PBKDF2 via ASP.NET Core Identity `PasswordHasher`
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

| Command                                        | Description        |
| ---------------------------------------------- | ------------------ |
| `dotnet build Backend/AIWorkspace.sln`         | Build solution     |
| `dotnet run --project Backend/src/Api`         | Run API server     |
| `dotnet run --project Backend/src/Api  --seed` | Run with seed data |
| `bash Backend/script/run.sh`                   | Quick run script   |

## 📄 License

This project is for demonstration/educational purposes.
