import { useState, useEffect, type ReactNode } from "react";
import { Layout, Breadcrumb, Button, Typography, theme } from "antd";
import { MenuOutlined } from "@ant-design/icons";
import { useLocation } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { AIFullscreenLoading, AISidebar } from "@/components";
import type { BreadcrumbItemType } from "antd/es/breadcrumb/Breadcrumb";

const { Content, Header } = Layout;
const { Text } = Typography;

/**
 * AppLayout — the app shell for all authenticated pages.
 *
 *  Desktop ──────────────────────────────────
 *  ┌──────────┬──────────────────────────────┐
 *  │          │  Breadcrumb                  │
 *  │ Sidebar  │                              │
 *  │ (sticky) │  Content (Outlet)            │
 *  │          │                              │
 *  └──────────┴──────────────────────────────┘
 *
 *  Mobile ───────────────────────────────────
 *  ┌─────────────────────────────────────────┐
 *  │ [☰]  AIWorkspace             (sticky)   │
 *  ├─────────────────────────────────────────┤
 *  │  Breadcrumb                             │
 *  │  Content (Outlet)                       │
 *  └─────────────────────────────────────────┘
 *  Sidebar slides in as a Drawer via [☰]
 */

interface AppLayoutProps {
  isLoading?: boolean;
  children?: ReactNode;
  breadcrumbItems?: BreadcrumbItemType[];
}
export function AppLayout({
  isLoading = false,
  children,
  breadcrumbItems = [],
}: AppLayoutProps) {
  const { t } = useTranslation();
  const { token } = theme.useToken();
  const location = useLocation();
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const [isMobile, setIsMobile] = useState(false);

  // ── Responsive detection ─────────────────────────────────
  useEffect(() => {
    const checkMobile = () => setIsMobile(window.innerWidth < 1024);
    checkMobile();
    window.addEventListener("resize", checkMobile);
    return () => window.removeEventListener("resize", checkMobile);
  }, []);

  // Close drawer on route change (mobile)
  useEffect(() => {
    if (isMobile) setSidebarOpen(false);
  }, [location.pathname, isMobile]);

  // ── Render ───────────────────────────────────────────────
  return (
    <Layout style={{ minHeight: "100vh", background: token.colorBgLayout }}>
      {isLoading && (
        <AIFullscreenLoading description={t("common.authenticating")} />
      )}
      {/* ── Sidebar (Desktop → Sider, Mobile → Drawer) ── */}
      <AISidebar
        isMobile={isMobile}
        open={sidebarOpen}
        onClose={() => setSidebarOpen(false)}
      />

      <Layout style={{ background: token.colorBgLayout }}>
        {/* ── Mobile top bar ── */}
        {isMobile && (
          <Header
            style={{
              padding: "0 16px",
              display: "flex",
              alignItems: "center",
              gap: 12,
              height: 56,
              lineHeight: "56px",
              backgroundColor: token.colorBgContainer,
              borderBottom: `1px solid ${token.colorBorderSecondary}`,
              position: "sticky",
              top: 0,
              zIndex: 100,
            }}
          >
            <Button
              type="text"
              icon={<MenuOutlined style={{ fontSize: 20 }} />}
              onClick={() => setSidebarOpen(true)}
              aria-label="Open menu"
            />
            <Text
              strong
              style={{
                fontSize: 17,
                color: token.colorPrimary,
                letterSpacing: "-0.02em",
              }}
            >
              AIWorkspace
            </Text>
          </Header>
        )}

        {/* ── Content area ── */}
        <Content
          style={{
            padding: isMobile ? "16px" : "24px 32px",
            minHeight: 280,
          }}
        >
          {/* Breadcrumb */}
          <Breadcrumb
            items={breadcrumbItems}
            style={{
              padding: 10,
              fontWeight: "bold",
              marginBottom: isMobile ? 16 : 24,
              borderBottom: `2px solid ${token.colorBorder}`,
            }}
          />

          {/* Page content injected via <Outlet /> */}
          {children}
        </Content>
      </Layout>
    </Layout>
  );
}
