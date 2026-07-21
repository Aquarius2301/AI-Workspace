import React from "react";
import {
  BrowserRouter,
  Routes,
  Route,
  Outlet,
  Navigate,
} from "react-router-dom";
import { useAuthMe } from "@/hooks";
import i18n from "@/i18n";
import {
  LoginPage,
  HomePage,
  OverviewPage,
  TeamPage,
  TeamDetailPage,
  ProfilePage,
  ProjectPage,
  ProjectDetailPage,
} from "@/pages";
import { AppLayout } from "@/layouts";
import { ROUTE } from "@/constants";

export function ProtectedRoute() {
  const { data, isLoading, isError } = useAuthMe();

  if (isLoading) {
    return <AppLayout isLoading={true} />;
  }

  if (isError || !data) {
    return <Navigate to={ROUTE.LOGIN} replace />;
  }

  // Sync user language preference from server
  const userLang = data.language || "en";
  if (localStorage.i18nextLng !== userLang) {
    localStorage.i18nextLng = userLang;
    i18n.changeLanguage(userLang);
  }

  return <Outlet />;
}

interface AppRoute {
  name: string;
  path: string;
  component: React.ComponentType<any>;
  isProtected?: boolean;
}

const protectedRoutes: AppRoute[] = [
  {
    name: "Overview",
    path: ROUTE.OVERVIEW,
    component: OverviewPage,
  },
  {
    name: "Teams",
    path: ROUTE.TEAM,
    component: TeamPage,
  },
  {
    name: "TeamDetail",
    path: `${ROUTE.TEAM}/:slug`,
    component: TeamDetailPage,
  },
  {
    name: "Profile",
    path: ROUTE.PROFILE,
    component: ProfilePage,
  },
  {
    name: "Projects",
    path: ROUTE.PROJECT,
    component: ProjectPage,
  },
  {
    name: "ProjectDetail",
    path: `${ROUTE.PROJECT}/:slug`,
    component: ProjectDetailPage,
  },
];

const publicRoutes: AppRoute[] = [
  {
    name: "Login",
    path: ROUTE.LOGIN,
    component: LoginPage,
  },
  {
    name: "Home",
    path: ROUTE.HOME,
    component: HomePage,
  },
];

export default function AppRouter() {
  return (
    <BrowserRouter>
      <React.Suspense fallback={<AppLayout isLoading={true} />}>
        <Routes>
          {/* Public Routes */}
          {publicRoutes.map((r) => {
            const Component = r.component;
            return <Route key={r.path} path={r.path} element={<Component />} />;
          })}

          {/* Protected Routes (Need to login) */}
          <Route element={<ProtectedRoute />}>
            {protectedRoutes.map((r) => {
              const Component = r.component;
              return (
                <Route key={r.path} path={r.path} element={<Component />} />
              );
            })}
          </Route>
        </Routes>
      </React.Suspense>
    </BrowserRouter>
  );
}
