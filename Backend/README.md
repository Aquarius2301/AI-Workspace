# AI Workspace — Backend

The backend for AI Workspace is a **.NET 8** Web API built with a clean architecture using CQRS (via MediatR), Entity Framework Core, and JWT-based authentication. It provides a RESTful API for team collaboration, project management, task tracking, documents, and AI interaction logging.

## 🏗 Architecture

The solution is split across four projects, each in its own folder:

```
Backend/
├── BusinessObject/              # Domain layer
│   ├── Entities/               # Domain models (User, Team, Project, TaskItem, etc.)
│   ├── Enums/                  # Enum types (TeamMemberRole, TaskItemStatus, etc.)
│   ├── Migrations/             # EF Core migrations
│   └── AIWorkspaceContext.cs   # DbContext
│
├── DataAccess/                  # Data layer
│   ├── Interceptors/           # EF Core interceptors (SQL capture for logging)
│   ├── Repositories/           # Generic repository pattern
│   ├── Services/               # SQL capture service
│   └── UnitOfWork/             # Unit of Work pattern
│
├── Infrastructure/              # Application / CQRS layer
│   ├── Common/                 # Shared models (Pagination, behaviors)
│   ├── Exceptions/             # Custom exceptions & error codes
│   ├── Functions/              # MediatR handlers grouped by domain:
│   │   ├── Auth/               #   Login, Register, Refresh, Logout, Me
│   │   ├── Projects/           #   CreateProject
│   │   ├── Summaries/          #   GetSummary (dashboard overview)
│   │   ├── Teams/              #   CRUD teams, members, roles, transfer admin
│   │   └── Users/              #   GetUsers, UpdateProfile, ChangePassword
│   ├── Helpers/                # JWT & password utilities
│   └── Settings/               # Strongly-typed config (AuthSetting)
│
└── WebApi/                      # Presentation / Host layer
    ├── Controllers/             # API controllers (Auth, Team, Project, User, Summary)
    ├── Middlewares/             # Request logging, exception handling, refresh cleanup, active tracking
    ├── Services/                # DI registration, auth, CORS, rate limiting, Swagger
    ├── Common/                  # API response wrapper & filter
    ├── Helpers/                 # Claim & cookie helpers
    ├── Seeds/                   # Development seed data
    └── Program.cs               # Application entry point
```

## 🧩 Domain Entities

| Entity | Description |
|---|---|
| `User` | User account with name, email, password hash, avatar, language preference |
| `Team` | Collaboration group with a name and optional description |
| `TeamMember` | User membership in a team with a role (Admin / CoAdmin / Leader / Member) |
| `Project` | Work unit scoped to a team, with visibility (Public / Private) |
| `ProjectMember` | Explicit user access to a project |
| `TaskItem` | Trackable work item with title, description, priority (1-3), status, assignee, due date |
| `Document` | Rich-text or markdown document within a project |
| `Comment` | Polymorphic comment on documents or tasks |
| `Attachment` | File attachment linked to documents or tasks |
| `AiInteraction` | Log of AI model prompts and responses, optionally tied to a user and project |
| `RefreshToken` | JWT refresh token with expiry and device info |

## 🔌 API Endpoints

### Authentication

| Method | Path | Description |
|---|---|---|
| POST | `/api/auth/login` | Login with email/password; sets HTTP-only cookies |
| POST | `/api/auth/register` | Create a new account |
| POST | `/api/auth/refresh` | Refresh access token via refresh token cookie |
| POST | `/api/auth/logout` | Clear tokens and expire session |
| POST | `/api/auth/revoke-all-refresh` | Revoke every refresh token for the user |
| GET | `/api/auth/me` | Get the current user profile |
| POST | `/api/auth/me/active` | Update the user's last-active timestamp |

### Teams

| Method | Path | Description |
|---|---|---|
| GET | `/api/teams` | List teams (paginated, filterable by membership & search) |
| POST | `/api/teams` | Create a team (creator becomes Admin) |
| GET | `/api/teams/{id}` | Get team details |
| PUT | `/api/teams/{id}` | Update team (Admin only) |
| DELETE | `/api/teams/{id}` | Delete team and all related data (Admin only) |
| GET | `/api/teams/{id}/members` | List team members (paginated, filterable) |
| GET | `/api/teams/{id}/me` | Current user's membership in the team |
| POST | `/api/teams/{id}/members` | Add members (Admin only) |
| PUT | `/api/teams/{id}/members/{memberId}` | Update member role (Admin only) |
| DELETE | `/api/teams/{id}/members/{memberId}` | Remove a member (Admin only) |
| GET | `/api/teams/{id}/available-members` | Users not yet in the team |
| POST | `/api/teams/{id}/transfer-admin/{targetUserId}` | Transfer Admin role to a CoAdmin |
| POST | `/api/teams/{id}/leave` | Current user leaves the team |
| GET | `/api/teams/{id}/projects` | List projects within a team |

### Projects

| Method | Path | Description |
|---|---|---|
| POST | `/api/projects` | Create a project (Admin/Leader only) |

### Users

| Method | Path | Description |
|---|---|---|
| GET | `/api/users` | List all users (paginated) |
| PUT | `/api/users` | Update own profile (name, avatar, language) |
| PATCH | `/api/users/{id}/password` | Change password |

### Dashboard

| Method | Path | Description |
|---|---|---|
| GET | `/api/summaries` | Aggregated overview stats, recent items, and task breakdown |

## ⚙️ Configuration

Key settings in `appsettings.json` / `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(local);Database=AIWorkspace;..."
  },
  "AuthSetting": {
    "JwtKey": "your-256-bit-key",
    "Issuer": "AiWorkspace",
    "Audience": "AiWorkspaceClients",
    "AccessTokenMinutes": 15,
    "RefreshTokenDays": 30,
    "AllowInsecureCookies": true,
    "CookieDomain": null
  }
}
```

## 🔒 Security

- Passwords are hashed with SHA256 via `PasswordHelper`
- JWT access tokens (short-lived) + opaque refresh tokens (long-lived) stored as HTTP-only cookies
- Rate limiting on the login endpoint (`LoginPolicy`) to mitigate brute-force attacks
- Team role-based authorization via MediatR `TeamRoleBehavior` pipeline
- Middleware pipeline: Request logging → Exception handling → Refresh token cleanup → CORS → Auth → Active tracking
- SQL capture interceptor for logging all queries during a request scope

## 🛠 Running

```bash
# Restore and run
dotnet restore
dotnet run --project WebApi

# Run with watch (hot reload)
dotnet watch run --project WebApi
```

- Dev server: `http://localhost:5157`
- Swagger UI: `http://localhost:5157/swagger`
- Seed data is applied automatically in Development mode
