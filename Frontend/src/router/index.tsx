import { useAuthMe } from "@/hooks";
import React from "react";
import {
  BrowserRouter,
  Routes,
  Route,
  Navigate,
  Outlet,
} from "react-router-dom";
import i18n from "@/i18n";
import {
  LoginPage,
  ProfilePage,
  TeamPage,
  TeamDetailPage,
  OverviewPage,
  HomePage,
} from "@/pages";
import { AppLayout } from "@/layouts";

export function ProtectedRoute() {
  const { data: me, isLoading, isError } = useAuthMe();

  if (isLoading) {
    return <AppLayout isLoading={true} />;
  }

  if (!isError && me) {
    const userLang = me.language || "en";
    if (localStorage.i18nextLng !== userLang) {
      localStorage.i18nextLng = userLang;
      i18n.changeLanguage(userLang);
    }
    return <Outlet />;
  }

  return <Navigate to="/login" replace />;
}

interface AppRoute {
  name: string;
  path: string;
  component: React.ComponentType<any>;
  isProtected?: boolean;
}

const protectedRoutes: AppRoute[] = [
  {
    name: "Profile",
    path: "/profile/me",
    component: ProfilePage,
  },
  {
    name: "Overview",
    path: "/overview",
    component: OverviewPage,
  },
  {
    name: "Teams",
    path: "/teams",
    component: TeamPage,
  },
  {
    name: "TeamDetail",
    path: "/teams/:id",
    component: TeamDetailPage,
  },
];

const publicRoutes: AppRoute[] = [
  {
    name: "Login",
    path: "/login",
    component: LoginPage,
  },
  {
    name: "Home",
    path: "/",
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
