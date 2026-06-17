import { FullscreenLoading } from "@/components";
import { useAuth } from "@/hooks";
import { HomePage, LoginPage, TeamPage, TeamPageDetail } from "@/pages";
import React from "react";
import {
  BrowserRouter,
  Routes,
  Route,
  Navigate,
  Outlet,
} from "react-router-dom";

export function ProtectedRoute() {
  const { me } = useAuth();

  const { data: user, isLoading, isError } = me;

  if (isLoading) {
    return <FullscreenLoading description="Đang xác thực" />;
  }

  if (!isError && user) {
    return <Outlet />;
  }

  return <Navigate to="/login" replace />;
}

type AppRoute = {
  name: string;
  path: string;
  component: React.ComponentType<any>;
  isProtected?: boolean;
};

const routes: AppRoute[] = [
  {
    name: "Team",
    path: "/teams",
    component: TeamPage,
    isProtected: true,
  },
  {
    name: "TeamPageDetail",
    path: "/teams/:id",
    component: TeamPageDetail,
    isProtected: true,
  },
  { name: "Home", path: "/", component: HomePage, isProtected: true },
  { name: "Login", path: "/login", component: LoginPage, isProtected: false },
];

export default function AppRouter() {
  // Tách riêng route cần bảo vệ và không cần bảo vệ từ mảng config cũ của bạn
  const protectedRoutes = routes.filter((r) => r.isProtected);
  const publicRoutes = routes.filter((r) => !r.isProtected);

  return (
    <BrowserRouter>
      <Routes>
        {/* Nhóm các ROUTES CÔNG KHAI (Không cần đăng nhập) */}
        {publicRoutes.map((r) => {
          const Component = r.component;
          return <Route key={r.path} path={r.path} element={<Component />} />;
        })}

        {/* Nhóm các ROUTES BẢO VỆ (Bắt buộc phải check dùng useAuth qua ProtectedRoute) */}
        <Route element={<ProtectedRoute />}>
          {protectedRoutes.map((r) => {
            const Component = r.component;
            return <Route key={r.path} path={r.path} element={<Component />} />;
          })}
        </Route>
      </Routes>
    </BrowserRouter>
  );
}
