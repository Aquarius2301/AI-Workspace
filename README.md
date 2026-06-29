# AI Workspace

A collaborative project management platform integrated with AI capabilities, built with a modern **.NET 8 + React** stack. Teams can manage projects, tasks, documents, and engage with AI-powered interactions — all in one workspace.

## ✨ Features

- **Team Collaboration** — Create teams, manage members with role-based access (Admin, CoAdmin, Leader, Member)
- **Project Management** — Organize work into projects with public/private visibility
- **Task Tracking** — Full task lifecycle (Open → In Progress → Done / Blocked) with priority and assignment
- **Document Management** — Create and store documents with markdown/rich-text content
- **AI Interactions** — Track AI model prompts and responses tied to users and projects
- **Commenting System** — Poloymorphic comments on documents and tasks
- **File Attachments** — Attach files to documents or tasks
- **JWT Authentication** — Secure login with access + refresh token rotation (HTTP-only cookies)
- **Role-Based Authorization** — Granular permission control at the team level
- **Multi-Language UI** — English and Vietnamese (i18n) with per-user language preference
- **Dark / Light Theme** — Theme toggle persisted across sessions
- **Dashboard** — At-a-glance overview with stats, recent activity, task breakdown, and team summaries

## 🧱 Tech Stack

### Backend

| Technology | Purpose |
|---|---|
| **.NET 8** (C#) | Runtime / Language |
| **ASP.NET Core Web API** | REST API layer |
| **Entity Framework Core 8** | ORM (SQL Server) |
| **MediatR** | CQRS / mediator pattern |
| **JWT Bearer Authentication** | Token-based auth |
| **Swashbuckle** | Swagger / OpenAPI docs |
| **System.Threading.RateLimiting** | Rate limiting (login endpoint) |

### Frontend

| Technology | Purpose |
|---|---|
| **React 19** | UI library |
| **TypeScript 6** | Type-safe JavaScript |
| **Vite 8** | Build tool / dev server |
| **Ant Design 6** | UI component library |
| **TanStack React Query 5** | Server state / caching |
| **React Router 7** | Client-side routing |
| **Axios** | HTTP client |
| **i18next** | Internationalization |
| **React Context** | Theming (dark/light) |

## 📁 Project Structure

```
AI-Workspace/
├── Backend/                     # .NET 8 solution
│   ├── AIWorkspace.sln
│   ├── BusinessObject/          # Domain models, DbContext, migrations
│   ├── DataAccess/             # Repositories, Unit of Work, SQL interceptors
│   ├── Infrastructure/         # CQRS handlers, helpers, settings
│   ├── WebApi/                 # Controllers, middleware, DI registration
│   └── dockerfile
├── Frontend/                    # React + TypeScript + Vite app
│   ├── src/
│   │   ├── api/               # Axios API clients
│   │   ├── components/        # Shared UI & business components
│   │   ├── hooks/             # Custom React hooks
│   │   ├── pages/             # Route pages (Overview, Teams, Login, etc.)
│   │   ├── router/            # Route definitions + auth guards
│   │   ├── contexts/          # React contexts (theme)
│   │   ├── i18n/              # Locale files (en, vi)
│   │   ├── types/             # TypeScript type definitions
│   │   └── utils/             # Utility functions
│   ├── dist/                  # Production build output
│   └── vite.config.ts
├── .gitignore
└── README.md
```

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) (v20+)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/) (local or remote)

### 1. Clone & Configure

```bash
git clone <repository-url>
cd AI-Workspace
```

Configure the database connection string in `Backend/WebApi/appsettings.json`:

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
    "RefreshTokenDays": 30
  }
}
```

### 2. Run the Backend

```bash
cd Backend
dotnet restore
dotnet run --project WebApi
```

The API starts on `http://localhost:5157` by default. Swagger UI is available at `/swagger` in development mode. A seed-data routine runs on first startup, populating sample teams, members, projects, tasks, and documents.

### 3. Run the Frontend

```bash
cd Frontend
npm install
npm run dev
```

The dev server starts on `http://localhost:5173` and proxies API calls to the backend (default: `http://localhost:5157`).

### Docker (Backend Only)

```bash
docker build -f Backend/dockerfile -t ai-workspace-api .
docker run -p 8080:80 ai-workspace-api
```

## 📡 API Overview

| Prefix | Description | Auth |
|---|---|---|
| `POST /api/auth/login` | Login | No |
| `POST /api/auth/register` | Register | No |
| `POST /api/auth/refresh` | Refresh tokens | Cookie |
| `POST /api/auth/logout` | Logout | Cookie |
| `POST /api/auth/revoke-all-refresh` | Revoke all tokens | JWT |
| `GET /api/auth/me` | Current user profile | JWT |
| `POST /api/auth/me/active` | Update activity timestamp | JWT |
| `GET /api/teams` | List teams (paginated) | JWT |
| `POST /api/teams` | Create team | JWT |
| `GET/PUT/DELETE /api/teams/{id}` | Team CRUD | JWT |
| `GET /api/teams/{id}/members` | Team members | JWT |
| `POST /api/teams/{id}/members` | Add members | JWT |
| `GET /api/teams/{id}/projects` | Team projects | JWT |
| `POST /api/projects` | Create project | JWT |
| `GET /api/users` | List users | JWT |
| `PUT /api/users` | Update profile | JWT |
| `PATCH /api/users/{id}/password` | Change password | JWT |
| `GET /api/summaries` | Dashboard summary | JWT |

## 🧪 Scripts

### Frontend

| Command | Description |
|---|---|
| `npm run dev` | Start Vite dev server |
| `npm run build` | TypeScript check + production build |
| `npm run preview` | Preview production build |
| `npm run lint` | Run oxlint linter |

## 📄 License

This project is for demonstration/educational purposes.
