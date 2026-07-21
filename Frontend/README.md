# AI Workspace — Frontend

The frontend for AI Workspace is a modern **React 19** single-page application built with **TypeScript**, **Vite**, and **Ant Design 6**. It provides a responsive dashboard for managing teams, projects, and tasks with multi-language support and theming.

## 🖥 Tech Stack

| Technology              | Version       | Purpose                         |
| ----------------------- | ------------- | ------------------------------- |
| React                   | ^19.2         | UI library                      |
| TypeScript              | ~6.0          | Type-safe development           |
| Vite                    | ^8.1          | Build tool and dev server       |
| Ant Design              | ^6.5          | UI component library            |
| TanStack React Query    | ^5.101        | Server state fetching & caching |
| React Router            | ^7.18         | Client-side routing             |
| Axios                   | ^1.18         | HTTP client for API calls       |
| i18next + react-i18next | ^26.3 / ^17.0 | Internationalization (EN / VI)  |
| oxlint                  | ^1.69         | Linter                          |

## 🗺 Routing

| Path              | Page                       | Auth Required |
| ----------------- | -------------------------- | ------------- |
| `/`               | HomePage (landing)         | No            |
| `/login`          | LoginPage                  | No            |
| `/overview`       | OverviewPage (dashboard)   | Yes           |
| `/teams`          | TeamPage (team list)       | Yes           |
| `/teams/:slug`    | TeamDetailPage             | Yes           |
| `/projects`       | ProjectPage (project list) | Yes           |
| `/projects/:slug` | ProjectDetailPage          | Yes           |
| `/profile/me`     | ProfilePage (user profile) | Yes           |

The `ProtectedRoute` wrapper checks authentication via `GET /api/auth/me` on mount and redirects unauthenticated users to `/login`. It also syncs the user's language preference to i18next.

## 📁 Project Structure

```
Frontend/
├── public/                     # Static assets
│   └── icon.png
├── src/
│   ├── api/                   # Axios-based API modules
│   │   ├── auth.api.ts       #   Login, register, refresh, logout, sessions
│   │   ├── team.api.ts       #   Teams CRUD, members
│   │   ├── project.api.ts    #   Project CRUD, tasks
│   │   ├── user.api.ts       #   Update profile, change password
│   │   ├── summary.api.ts    #   Dashboard summary
│   │   ├── config.api.ts     #   Axios instance with interceptors (auto-refresh, 401 queue)
│   │   └── index.ts          #   Re-exports
│   │
│   ├── components/            # Reusable components
│   │   ├── ui/               #   Generic UI
│   │   │   ├── AICard.tsx
│   │   │   ├── AICardItem.tsx
│   │   │   ├── AIFullLoading.tsx
│   │   │   ├── AIList.tsx
│   │   │   ├── AIModal.tsx
│   │   │   ├── AIPagination.tsx
│   │   │   ├── AIThemeSwitch.tsx
│   │   │   ├── AIUserAvatar.tsx
│   │   │   ├── NotFound.tsx
│   │   │   └── index.ts
│   │   ├── business/         #   Domain-specific components
│   │   │   ├── task/         #   AITaskStatusTag/Select, AITaskPriorityTag/Select
│   │   │   ├── team/         #   AITeamRoleTag/Select
│   │   │   ├── visibility/   #   AIVisibilityTag/Select
│   │   │   ├── project/      #   AIProjectRoleTag/Select
│   │   │   └── index.ts
│   │   └── layout/           #   App shell
│   │       ├── AISidebar.tsx
│   │       └── index.ts
│   │
│   ├── constants/             # App-wide constants
│   │   ├── endpoints.ts      #   API endpoint paths
│   │   ├── routes.ts         #   Route paths
│   │   ├── theme.ts         #   Ant Design theme tokens (light/dark)
│   │   └── index.ts
│   │
│   ├── contexts/              # React contexts
│   │   ├── ThemeContext.tsx   #   Dark/light theme management (persisted to localStorage)
│   │   └── index.ts
│   │
│   ├── hooks/                 # Custom React hooks
│   │   ├── api/              #   TanStack Query hooks
│   │   │   ├── useAuth.hook.ts
│   │   │   ├── useTeam.hook.ts
│   │   │   ├── useProject.hook.ts
│   │   │   ├── useSummary.hook.ts
│   │   │   ├── useUser.hook.ts
│   │   │   └── index.ts
│   │   ├── useDebounce.hook.ts
│   │   ├── useGetCacheData.hook.ts  # Reactive cache reader
│   │   ├── useLanguage.hook.ts
│   │   ├── useSearch.hook.ts
│   │   ├── useTheme.hook.ts
│   │   └── index.ts
│   │
│   ├── i18n/                  # Internationalization
│   │   ├── locales/en/       #   English translations
│   │   ├── locales/vi/       #   Vietnamese translations
│   │   └── index.ts          #   i18next config (LanguageDetector, localStorage)
│   │
│   ├── layouts/               # App layout wrapper
│   │   └── index.tsx         #   AppLayout: Sidebar + Header (mobile) + Breadcrumb + Content
│   │
│   ├── pages/                 # Route-level page components (lazy-loaded)
│   │   ├── HomePage/         #   Landing / root page with quick actions
│   │   ├── LoginPage/        #   Login screen with banner + form + language/theme toggle
│   │   ├── OverviewPage/     #   Dashboard with stats, task breakdown, recent tasks, team summaries
│   │   ├── TeamPage/         #   Team listing with search + create modal
│   │   ├── TeamDetailPage/   #   Single team: info, members, projects
│   │   ├── ProjectPage/      #   Project listing with search + filters
│   │   ├── ProjectDetailPage/#   Single project: info, members, tasks
│   │   ├── ProfilePage/      #   User profile: update info, change password
│   │   └── index.ts          #   Lazy-loaded exports
│   │
│   ├── router/                # Route configuration
│   │   └── index.tsx         #   Public + protected routes with ProtectedRoute guard
│   │
│   ├── types/                 # TypeScript type definitions
│   │   ├── auth.type.ts      #   Auth request/response types
│   │   ├── team.type.ts      #   Team & member types
│   │   ├── project.type.ts   #   Project types
│   │   ├── task.type.ts      #   Task item types
│   │   ├── summary.type.ts   #   Dashboard summary types
│   │   ├── user.type.ts      #   User types
│   │   ├── common.type.ts    #   Shared types (ApiResponse, Pagination)
│   │   └── index.ts          #   Re-exports
│   │
│   ├── utils/                 # Utility functions
│   │   ├── date.util.ts      #   Date formatting
│   │   ├── error.util.ts     #   Error handling + i18n translation
│   │   ├── common.util.ts    #   Common utilities
│   │   ├── image.util.ts     #   Image helpers
│   │   ├── userAgent.util.ts #   User-agent parsing (sessions)
│   │   └── index.ts
│   │
│   ├── App.tsx                # Root component (QueryClientProvider, ConfigProvider, Router)
│   ├── main.tsx               # Application entry point (ThemeProvider, StrictMode)
│   └── index.css             # Global styles (body reset)
│
├── index.html                 # HTML entry point
├── vite.config.ts             # Vite config (React plugin, @ path alias)
├── tsconfig.json              # TypeScript configuration (references)
├── tsconfig.app.json          # App-specific TS config (path aliases, strict mode)
├── tsconfig.node.json         # Node-specific TS config
├── .env                       # Environment variables (VITE_API_BASE_URL)
├── .env.development.local     # Local overrides (gitignored)
├── package.json
└── README.md
```

## 🚀 Getting Started

### Prerequisites

- [Node.js](https://nodejs.org/) v20+
- npm v10+

### Installation

```bash
cd Frontend
npm install
```

### Development

```bash
npm run dev
```

Starts the Vite dev server at `http://localhost:5173`. API requests are sent to the backend at the URL specified in your `.env` file:

```
VITE_API_BASE_URL=http://localhost:5157/api
```

### Production Build

```bash
npm run build
```

Outputs optimized assets to `dist/`. Preview the build with:

```bash
npm run preview
```

### Linting

```bash
npm run lint
```

Uses [oxlint](https://oxc.dev/docs/guide/usage/lint) for fast, Rust-powered linting.

> **Note**: `package.json` currently includes `@types/axios` in devDependencies. This package is deprecated since axios 1.x ships its own types — it can be removed in a future cleanup.

## 🎨 Theme

The app supports **light** and **dark** themes using Ant Design's `ConfigProvider` with `ThemeContext`. Theme tokens are defined in `src/constants/theme.ts`. The active theme is persisted to `localStorage` via the `ThemeContext`.

| Token           | Light                  | Dark                   |
| --------------- | ---------------------- | ---------------------- |
| `colorPrimary`  | `#4F46E5` (Indigo 600) | `#6366F1` (Indigo 500) |
| `colorBgLayout` | `#F8FAFC` (Slate 50)   | `#09090B` (Zinc 950)   |
| `colorTextBase` | `#0F172A` (Slate 900)  | `#FAFAFA` (Zinc 50)    |
| `colorBorder`   | `#E2E8F0` (Slate 200)  | `#27272A` (Zinc 800)   |

## 🌐 Internationalization

Two locales are available:

- **English** (`src/i18n/locales/en/translation.json`)
- **Vietnamese** (`src/i18n/locales/vi/translation.json`)

The user's language preference is stored server-side and synced on login via the `GET /api/auth/me` response. The `ProtectedRoute` component automatically syncs the user's language setting to i18next.

## 🔐 Authentication Flow

1. **Login**: User submits email/password → backend sets `access_token` + `refresh_token` HttpOnly cookies
2. **Protected Routes**: `ProtectedRoute` calls `GET /api/auth/me` on mount to verify authentication
3. **Auto Refresh**: Axios interceptor detects 401 responses, automatically calls `POST /api/auth/refresh`, and retries the original request
4. **Queue Mechanism**: Only 1 refresh request runs at a time; concurrent 401s are queued and retried after refresh succeeds
5. **Redirect**: If refresh fails, user is redirected to `/login`

## 📦 Key Dependencies & Rationale

| Package                 | Why                                                                                        |
| ----------------------- | ------------------------------------------------------------------------------------------ |
| `@tanstack/react-query` | Automatic caching, deduplication, stale-while-revalidate for API calls (stale time: 5 min) |
| `antd`                  | Rich set of production-ready UI components (Table, Modal, Form, Card, Tag, Select)         |
| `react-router-dom`      | Declarative routing with protected route guard pattern                                     |
| `axios`                 | Interceptors for auto-refresh token, 401 queue mechanism, centralized error handling       |
| `i18next`               | Proven i18n framework with browser language detection and localStorage caching             |
| `oxlint`                | Fast Rust-based linter                                                                     |
