# AI Workspace — Frontend

The frontend for AI Workspace is a modern **React 19** single-page application built with **TypeScript**, **Vite**, and **Ant Design 6**. It provides a responsive dashboard for managing teams, projects, and tasks with AI-powered capabilities.

## 🖥 Tech Stack

| Technology | Version | Purpose |
|---|---|---|
| React | 19 | UI library |
| TypeScript | 6 | Type-safe development |
| Vite | 8 | Build tool and dev server |
| Ant Design | 6 | UI component library |
| TanStack React Query | 5 | Server state fetching & caching |
| React Router | 7 | Client-side routing |
| Axios | 1.18 | HTTP client for API calls |
| i18next + react-i18next | latest | Internationalization (EN / VI) |
| oxlint | 1.69 | Linter |

## 📁 Project Structure

```
Frontend/
├── public/                     # Static assets
│   ├── favicon.svg
│   └── icons.svg
├── src/
│   ├── api/                   # Axios-based API modules
│   │   ├── auth.api.ts        #   Login, register, refresh, logout
│   │   ├── account.api.ts     #   Profile update, password change
│   │   ├── team.api.ts        #   Teams CRUD, members, transfer admin
│   │   ├── project.api.ts     #   Project creation
│   │   ├── summary.api.ts     #   Dashboard summary
│   │   └── index.ts           #   Axios instance with interceptors
│   │
│   ├── components/            # Reusable components
│   │   ├── ui/               #   Generic UI (Card, Modal, List, Loading, ThemeSwitch, Avatar)
│   │   ├── business/         #   Domain-specific (Role tags, Status tags, Visibility selectors)
│   │   └── layout/           #   App shell (Sidebar)
│   │
│   ├── contexts/              # React contexts
│   │   └── ThemeContext.tsx   #   Dark/light theme management
│   │
│   ├── hooks/                 # Custom React hooks
│   │   ├── useAuth.hook.ts   #   Authentication state
│   │   ├── useTeam.hook.ts   #   Team data queries/mutations
│   │   ├── useProject.hook.ts #   Project data queries
│   │   ├── useUser.hook.ts   #   User data queries
│   │   ├── useSummary.hook.ts #   Dashboard summary
│   │   ├── useTheme.hook.ts  #   Theme accessor
│   │   ├── useSearch.hook.ts #   Debounced search
│   │   ├── useDebounce.hook.ts#   Generic debounce utility
│   │   └── useGetCacheData.hook.ts #   Cache-first data retrieval
│   │
│   ├── pages/                 # Route-level page components
│   │   ├── LoginPage/        #   Login / register screen
│   │   ├── HomePage/         #   Landing / root page
│   │   ├── OverviewPage/     #   Dashboard with stats, tasks, activity
│   │   ├── TeamPage/         #   Team listing with create modal
│   │   └── TeamDetailPage/   #   Single team: info, members, projects
│   │
│   ├── router/                # Route configuration
│   │   └── index.tsx          #   Public + protected routes with auth guard
│   │
│   ├── types/                 # TypeScript type definitions
│   │   ├── auth.type.ts      #   Auth request/response types
│   │   ├── account.type.ts   #   Profile types
│   │   ├── team.type.ts      #   Team & member types
│   │   ├── project.type.ts   #   Project types
│   │   ├── task.type.ts      #   Task item types
│   │   ├── summary.type.ts   #   Dashboard summary types
│   │   ├── common.type.ts    #   Shared types (pagination, etc.)
│   │   └── index.ts          #   Re-exports
│   │
│   ├── i18n/                  # Internationalization
│   │   ├── locales/en/       #   English translations
│   │   ├── locales/vi/       #   Vietnamese translations
│   │   └── index.ts          #   i18next configuration
│   │
│   ├── constants/             # App-wide constants (theme tokens)
│   ├── utils/                 # Utility functions (date formatting, error handling)
│   ├── layouts/               # App layout wrapper
│   ├── App.tsx                # Root component (providers, theme, query client)
│   ├── main.tsx               # Application entry point
│   └── index.css              # Global styles
│
├── dist/                      # Production build output
├── index.html                 # HTML entry point
├── vite.config.ts             # Vite config (React plugin, @ path alias)
├── tsconfig.json              # TypeScript configuration (base)
├── tsconfig.app.json          # App-specific TS config
├── tsconfig.node.json         # Node-specific TS config
├── .oxlintrc.json             # Linter configuration
├── .env                       # Environment variables (VITE_API_BASE_URL)
├── .env.development           # Dev-specific env overrides
└── package.json
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

Starts the Vite dev server at `http://localhost:5173`. API requests are proxied to the backend at the URL specified in your `.env` file:

```
VITE_API_BASE_URL=http://localhost:5157
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

## 🧭 Routing

| Path | Page | Auth Required |
|---|---|---|
| `/` | HomePage (landing) | No |
| `/login` | LoginPage | No |
| `/overview` | OverviewPage (dashboard) | Yes |
| `/teams` | TeamPage (team list) | Yes |
| `/teams/:id` | TeamDetailPage | Yes |

The `ProtectedRoute` wrapper checks authentication via `GET /api/auth/me` on mount and redirects unauthenticated users to `/login`. It also syncs the user's language preference to i18next.

## 🎨 Theme

The app supports **light** and **dark** themes using Ant Design's `ConfigProvider`. The active theme is persisted via the `ThemeContext`. Theme tokens are defined in `src/constants/index.ts`.

## 🌐 Internationalization

Two locales are available:

- **English** (`src/i18n/locales/en/translation.json`)
- **Vietnamese** (`src/i18n/locales/vi/translation.json`)

The user's language preference is stored server-side and synced on login via the `GET /api/auth/me` response.

## 📦 Key Dependencies & Rationale

| Package | Why |
|---|---|
| `@tanstack/react-query` | Automatic caching, deduplication, stale-while-revalidate for API calls (stale time: 5 min) |
| `antd` | Rich set of production-ready UI components (Table, Modal, Form, Card, Tag, Select) |
| `react-router-dom` | Declarative routing with protected route guard pattern |
| `axios` | Interceptors for attaching auth cookies, centralized error handling |
| `i18next` | Proven i18n framework with lazy-loading and browser language detection |
| `oxlint` | Fast Rust-based linter (drop-in replacement for ESLint) |
