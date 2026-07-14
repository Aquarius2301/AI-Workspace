# AI Workspace — Backend

The backend for AI Workspace is a **.NET 8** Web API built with **Clean Architecture** using **CQRS (MediatR)**, **Entity Framework Core**, **FluentValidation**, and **JWT-based authentication (HttpOnly Cookies)**. It provides a RESTful API for team collaboration, project management, and task tracking.

## 🏗 Architecture

The solution follows **Clean Architecture** with 4 projects:

```
Backend/
├── src/
│   ├── Domain/                 # Innermost layer — Enterprise business rules
│   │   ├── Entities/           # Domain models (User, Team, Project, TaskItem, ...)
│   │   └── Enums/              # Enum types (TeamMemberRole, TaskItemStatus, TaskPriority, ProjectVisibility, ...)
│   │
│   ├── Application/            # Use case / CQRS layer
│   │   ├── Common/             # Shared abstractions
│   │   │   ├── IAppDbContext   # DbContext interface (abstraction for Persistence)
│   │   │   ├── Behaviors/      # MediatR pipeline behaviors (Validation, TeamRole, ProjectRole)
│   │   │   ├── Exceptions/     # Custom exceptions (BadRequest, NotFound, Unauthorized, Forbidden...) + ErrorCodes
│   │   │   ├── Models/         # Pagination models
│   │   │   └── Services/       # Service interfaces (IJwtService, IImageKitService)
│   │   ├── Features/           # CQRS grouped by domain
│   │   │   ├── Auth/           #   Login, Register, Refresh, Logout, Sessions, Me
│   │   │   ├── Projects/       #   CreateProject, GetMyProjects, GetProject, GetProjectMembers
│   │   │   ├── Summaries/      #   GetSummary (dashboard overview)
│   │   │   ├── Tasks/          #   Task queries
│   │   │   ├── Teams/          #   Create/Update/Delete team, members, roles
│   │   │   ├── Uploads/        #   Upload picture
│   │   │   └── Users/          #   UpdateProfile, ChangePassword
│   │   └── Helpers/            # Utilities (PasswordHasher, SlugHelper, CollationSearch)
│   │
│   ├── Infrastructure/         # External concerns implementation
│   │   ├── Persistence/        # EF Core DbContext + Entity configurations
│   │   │   └── Configurations/ #   Fluent API entity configs
│   │   ├── Migrations/         # EF Core migrations
│   │   ├── Services/           # Concrete implementations (JwtService, ImageKitService)
│   │   └── Settings/           # Strongly-typed config (AuthSetting, UploadSetting, RateLimitSetting, FrontendSetting)
│   │
│   └── Api/                    # Presentation / Host layer
│       ├── Controllers/        # API controllers (Auth, Team, Project, User, Summary, Upload)
│       ├── Middleware/          # Request pipeline (Exception, RequestLogging, ActiveTracking)
│       ├── Filters/            # API response wrapper & filter
│       ├── Extensions/         # DI registration extensions (Auth, CORS, Database, RateLimit, Swagger...)
│       ├── Helpers/            # ClaimHelper, CookieHelper
│       ├── Backgrounds/        # Background services (token cleanup, picture cleanup)
│       ├── Seeds/              # Development seed data
│       └── Program.cs          # Application entry point
```

### Layer Dependencies

```
Api → Application → Domain
Infrastructure → Application
```

- **Domain**: No dependencies — pure business logic
- **Application**: Depends only on Domain. **Never references Infrastructure**
- **Infrastructure**: Implements interfaces from Application
- **Api**: Orchestrator — registers DI, middleware, controllers

## 🧩 Domain Entities

| Entity          | Description                                                                       |
| --------------- | --------------------------------------------------------------------------------- |
| `User`          | Account with name, email, password hash, avatar, language                         |
| `Team`          | Collaboration group with name, slug, description                                  |
| `TeamMember`    | User membership in team with role (Admin / CoAdmin / Member)                      |
| `Project`       | Work unit scoped to team, with visibility (Public / Private)                      |
| `ProjectMember` | Explicit user access to project with role                                         |
| `TaskItem`      | Trackable work item with title, description, priority, status, assignee, due date |
| `Picture`       | Uploaded image record with file ID, URL, active status, user link                 |
| `RefreshToken`  | JWT refresh token with absolute expiry and device info                            |

## 🔌 API Endpoints

### Authentication

| Method | Path                            | Auth Required | Description                            |
| ------ | ------------------------------- | ------------- | -------------------------------------- |
| POST   | `/api/auth/login`               | No            | Login, sets HttpOnly cookies           |
| POST   | `/api/auth/register`            | No            | Create account                         |
| POST   | `/api/auth/refresh`             | No            | Rotate refresh token (absolute expiry) |
| POST   | `/api/auth/logout`              | No            | Revoke refresh token, clear cookies    |
| DELETE | `/api/auth/sessions`            | Yes           | Revoke all sessions                    |
| GET    | `/api/auth/sessions`            | Yes           | List active sessions                   |
| DELETE | `/api/auth/sessions/{deviceId}` | Yes           | Revoke specific device session         |
| GET    | `/api/auth/me`                  | Yes           | Get current user profile               |

### Teams

| Method | Path                                 | Auth Required | Description                 |
| ------ | ------------------------------------ | ------------- | --------------------------- |
| GET    | `/api/teams`                         | Yes           | List my teams (paginated)   |
| POST   | `/api/teams`                         | Yes           | Create team                 |
| GET    | `/api/teams/{id}`                    | Yes           | Get team details            |
| PUT    | `/api/teams/{id}`                    | Yes           | Update team (Admin)         |
| DELETE | `/api/teams/{id}`                    | Yes           | Delete team (Admin)         |
| GET    | `/api/teams/{id}/members`            | Yes           | List members (paginated)    |
| POST   | `/api/teams/{id}/members`            | Yes           | Add members (Admin/CoAdmin) |
| PUT    | `/api/teams/{id}/members/{memberId}` | Yes           | Update member role (Admin)  |
| GET    | `/api/teams/{id}/available-members`  | Yes           | Users not yet in team       |
| GET    | `/api/teams/{teamId}/projects`       | Yes           | List projects in team       |

### Projects

| Method | Path                                 | Auth Required | Description                      |
| ------ | ------------------------------------ | ------------- | -------------------------------- |
| GET    | `/api/projects`                      | Yes           | List my projects (paginated)     |
| POST   | `/api/projects`                      | Yes           | Create project (Admin/CoAdmin)   |
| GET    | `/api/projects/{id}`                 | Yes           | Get project by ID                |
| GET    | `/api/projects/{slug}`               | Yes           | Get project ID by slug           |
| GET    | `/api/projects/{projectId}/members`  | Yes           | List project members (paginated) |
| GET    | `/api/projects/{projectId}/tasks/me` | Yes           | My tasks in project (paginated)  |
| GET    | `/api/projects/{projectId}/tasks`    | Yes           | All project tasks (filterable)   |

### Users

| Method | Path                  | Auth Required | Description     |
| ------ | --------------------- | ------------- | --------------- |
| PUT    | `/api/users`          | Yes           | Update profile  |
| PATCH  | `/api/users/password` | Yes           | Change password |

### Upload

| Method | Path                   | Auth Required | Description                                  |
| ------ | ---------------------- | ------------- | -------------------------------------------- |
| POST   | `/api/uploads/picture` | Yes           | Upload image to ImageKit (JPEG/PNG/GIF/WebP) |

### Dashboard

| Method | Path             | Auth Required | Description                                  |
| ------ | ---------------- | ------------- | -------------------------------------------- |
| GET    | `/api/summaries` | Yes           | Aggregated overview (teams, projects, tasks) |

## ⚙️ Configuration

Key settings in `appsettings.json`:

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
    "RefreshTokenDays": 7
  },
  "UploadSetting": {
    "PublicKey": "your-imagekit-public-key",
    "PrivateKey": "your-imagekit-private-key"
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

## 🔒 Security

- **Passwords**: Hashed with ASP.NET Core Identity `PasswordHasher` (PBKDF2)
- **JWT**: Access tokens (15 min) + opaque refresh tokens (7 days absolute expiry)
- **Refresh Token Rotation**: Old token revoked on each refresh → prevents reuse attacks
- **Absolute Expiry**: Refresh token expiry is based on original `CreatedAt` — user must re-login after 7 days even if they refresh frequently
- **Cookies**: HttpOnly + Secure + SameSite=None (for cross-origin dev)
- **Rate Limiting**: `LoginPolicy` — 5 requests per minute on login endpoint
- **Authorization**: `TeamRoleBehavior` / `ProjectRoleBehavior` MediatR pipeline
- **Background Cleanup**: Expired refresh tokens (every 1h), orphaned pictures (every 24h)
- **Sensitive Data Logging**: Disabled in production; only enabled in Development
- **Request Logging**: Passwords and tokens are masked before logging

## 🛠 Running

```bash
# Restore and build
dotnet restore Backend/AIWorkspace.sln
dotnet build Backend/AIWorkspace.sln

# Run
dotnet run --project Backend/src/Api

# Run with seed data
dotnet run --project Backend/src/Api --seed
```

- Dev server: `http://localhost:5157`
- Swagger UI: `http://localhost:5157/swagger`

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB or Docker)
- ImageKit account (for upload feature)

### Database

```bash
# Add migration (from Backend directory)
dotnet ef migrations add MigrationName --project src/Infrastructure --startup-project src/Api

# Update database
dotnet ef database update --project src/Infrastructure --startup-project src/Api
```

## 🧪 Seed Data

Running with `--seed` flag in Development mode:

- Deletes existing data
- Creates 10 users, 4 teams, 20 team members, 8 projects, 24 project members, 48 tasks
- Default password: `Password123@`
