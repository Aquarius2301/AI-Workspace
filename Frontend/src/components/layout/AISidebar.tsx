import { useState } from "react";
import { useTranslation } from "react-i18next";
import {
  Layout,
  Menu,
  Button,
  Flex,
  Space,
  Typography,
  theme,
  Drawer,
} from "antd";
import { useNavigate, useLocation } from "react-router-dom";
import {
  AppstoreOutlined,
  SettingOutlined,
  FolderOpenOutlined,
  LogoutOutlined,
  TeamOutlined,
} from "@ant-design/icons";
import type { AuthResponse } from "@/types";
import { AUTH_ME_QUERY_KEY, useAuth, useGetCacheData, useTheme } from "@/hooks";
import { AIModal, AIThemeSwitch, UserAvatar } from "../ui";

const { Sider } = Layout;
const { Text, Title } = Typography;

export interface AISidebarProps {
  isMobile?: boolean;
  open?: boolean;
  onClose?: () => void;
}

export function AISidebar({ isMobile, open, onClose }: AISidebarProps) {
  const { t } = useTranslation();
  const { token } = theme.useToken();
  const { theme: currentTheme } = useTheme();
  const navigate = useNavigate();
  const location = useLocation();
  const [collapsed, setCollapsed] = useState(false);
  const [logOutModal, setLogOutModal] = useState(false);

  const me = useGetCacheData<AuthResponse>(AUTH_ME_QUERY_KEY);

  const { logout } = useAuth();

  const menuItems = [
    {
      key: "/overview",
      icon: <AppstoreOutlined style={{ fontSize: "18px" }} />,
      label: t("sidebar.menu.overview"),
    },
    {
      key: "/teams",
      icon: <TeamOutlined style={{ fontSize: "18px" }} />,
      label: t("sidebar.menu.teams"),
    },
    {
      key: "/projects",
      icon: <FolderOpenOutlined style={{ fontSize: "18px" }} />,
      label: t("sidebar.menu.projects"),
    },
    {
      key: "/settings",
      icon: <SettingOutlined style={{ fontSize: "18px" }} />,
      label: t("sidebar.menu.settings"),
    },
  ];

  const handleMenuClick = (e: { key: string }) => {
    navigate(e.key);
  };

  const getSelectedKey = (pathname: string) => {
    if (pathname.startsWith("/overview")) return "/overview";
    if (pathname.startsWith("/teams")) return "/teams";
    if (pathname.startsWith("/projects")) return "/projects";
    if (pathname.startsWith("/settings")) return "/settings";

    return "/";
  };

  const sidebarContent = (
    <div
      style={{
        display: "flex",
        flexDirection: "column",
        height: "100%",
        justifyContent: "space-between",
      }}
    >
      {/*  LOGO & MENU ITEMS */}
      <div>
        {/* LOGO BRANDING */}
        <div
          style={{
            padding: collapsed ? "24px 0" : "24px 20px",
            textAlign: "center",
            transition: "all 0.2s",
          }}
        >
          {collapsed ? (
            <Flex justify="center">
              <AIThemeSwitch />
            </Flex>
          ) : (
            <Space size={16}>
              <Title
                level={4}
                style={{
                  margin: 0,
                  color: token.colorPrimary,
                  fontWeight: 800,
                  letterSpacing: "-0.03em",
                }}
              >
                AIWorkspace
              </Title>

              <AIThemeSwitch />
            </Space>
          )}
        </div>

        {/* MENU CHÍNH */}
        <Menu
          mode="inline"
          selectedKeys={[getSelectedKey(location.pathname)]}
          items={menuItems}
          onClick={(e) => {
            handleMenuClick(e);
            isMobile && onClose?.();
          }}
          style={{
            borderRight: 0,
            backgroundColor: "transparent",
          }}
        />
      </div>

      {/*AVATAR & LOGOUT */}
      <div
        style={{
          padding: "16px",
          borderTop: `1px solid ${token.colorBorder}`,
          backgroundColor: isDarkMode(token) ? "#141416" : "#F8FAFC",
          transition: "all 0.2s",
        }}
      >
        {isMobile || collapsed ? (
          <Space vertical size="small" style={{ width: "100%" }}>
            <Space>
              <UserAvatar src={me?.avatar} userName={me?.name} size={44} />
              <Space vertical align="start" size={0} style={{ width: "100%" }}>
                <Text
                  strong
                  style={{
                    color: token.colorTextBase,
                    whiteSpace: "nowrap",
                    overflow: "hidden",
                    textOverflow: "ellipsis",
                  }}
                >
                  {me?.name}
                </Text>
                <Text
                  type="secondary"
                  style={{
                    fontSize: 12,
                    whiteSpace: "nowrap",
                    overflow: "hidden",
                    textOverflow: "ellipsis",
                  }}
                >
                  {me?.email}
                </Text>
              </Space>
            </Space>
            <Flex justify="center" style={{ width: "100%" }}>
              <Button
                type="text"
                danger
                icon={<LogoutOutlined style={{ fontSize: 18 }} />}
                onClick={() => setLogOutModal(true)}
              />
            </Flex>
          </Space>
        ) : (
          <div
            style={{
              display: "flex",
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Space size="middle" style={{ overflow: "hidden" }}>
              <UserAvatar src={me?.avatar} userName={me?.name} size={44} />
              <div
                style={{
                  display: "flex",
                  flexDirection: "column",
                  overflow: "hidden",
                }}
              >
                <Text
                  strong
                  style={{
                    color: token.colorTextBase,
                    whiteSpace: "nowrap",
                    overflow: "hidden",
                    textOverflow: "ellipsis",
                  }}
                >
                  {me?.name}
                </Text>
                <Text
                  type="secondary"
                  style={{
                    fontSize: 12,
                    whiteSpace: "nowrap",
                    overflow: "hidden",
                    textOverflow: "ellipsis",
                  }}
                >
                  {me?.email}
                </Text>
              </div>
            </Space>

            <Button
              type="text"
              danger
              shape="circle"
              icon={<LogoutOutlined style={{ fontSize: 16 }} />}
              onClick={() => setLogOutModal(true)}
              style={{ flexShrink: 0 }}
            />
          </div>
        )}
      </div>
    </div>
  );

  const logoutModal = (
    <AIModal
      open={logOutModal}
      onCancel={() => setLogOutModal(false)}
      title={t("sidebar.logout.title")}
      onOk={() => {
        logout.mutate();
        navigate("/login");
      }}
      footer={[
        { type: "cancel" },
        { type: "delete", text: t("sidebar.logout.ok") },
      ]}
    >
      {t("sidebar.logout.content")}
    </AIModal>
  );

  const sidebar = isMobile ? (
    <Drawer
      placement="left"
      open={open}
      onClose={onClose}
      size={260}
      styles={{
        body: { padding: 0 },
      }}
    >
      {sidebarContent}
    </Drawer>
  ) : (
    <Sider
      collapsible
      breakpoint="lg"
      collapsedWidth={72}
      width={260}
      collapsed={collapsed}
      onCollapse={(value) => setCollapsed(value)}
      trigger={null}
      style={{
        height: "100vh",
        position: "sticky",
        top: 0,
        left: 0,
        borderRight: `1px solid ${token.colorBorder}`,
        backgroundColor: token.colorBgContainer,
        transition: "all 0.2s ease",
      }}
    >
      {sidebarContent}
    </Sider>
  );

  return (
    <div>
      {sidebar}
      {logoutModal}
    </div>
  );
}

const isDarkMode = (token: any) => token.colorTextBase === "#FAFAFA";
