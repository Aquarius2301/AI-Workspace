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
  FolderOpenOutlined,
  TeamOutlined,
  LogoutOutlined,
} from "@ant-design/icons";
import { AUTH_ME_QUERY_KEY, useGetCacheData, useLogout } from "@/hooks";
import { AIModal, AIThemeSwitch, UserAvatar } from "../ui";
import type { AuthResponse } from "@/types";
import { ROUTE } from "@/constants";

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
  const navigate = useNavigate();
  const location = useLocation();
  const [logOutModal, setLogOutModal] = useState(false);

  const me = useGetCacheData<AuthResponse>(AUTH_ME_QUERY_KEY);

  const { mutate } = useLogout();

  const menuItems = [
    {
      key: ROUTE.OVERVIEW,
      icon: <AppstoreOutlined style={{ fontSize: "18px" }} />,
      label: t("overviewPage.title"),
    },
    {
      key: ROUTE.TEAM,
      icon: <TeamOutlined style={{ fontSize: "18px" }} />,
      label: t("teamPage.title"),
    },
    {
      key: ROUTE.PROJECT,
      icon: <FolderOpenOutlined style={{ fontSize: "18px" }} />,
      label: t("projectPage.title"),
    },
    // {
    //   key: "/settings",
    //   icon: <SettingOutlined style={{ fontSize: "18px" }} />,
    //   label: t("sidebar.menu.settings"),
    // },
  ];

  const handleMenuClick = (e: { key: string }) => {
    navigate(e.key);
  };

  const getSelectedKey = (pathname: string) => {
    if (pathname.startsWith(ROUTE.OVERVIEW)) return "/overview";
    if (pathname.startsWith(ROUTE.TEAM)) return "/teams";
    if (pathname.startsWith(ROUTE.PROJECT)) return "/projects";
    // if (pathname.startsWith("/settings")) return "/settings";

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
            padding: "24px 20px",
            textAlign: "center",
          }}
        >
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
        </div>

        {/* MENU */}
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
          padding: "8px",
          borderTop: `1px solid ${token.colorBorder}`,
          backgroundColor: isDarkMode(token) ? "#141416" : "#F8FAFC",
          transition: "all 0.2s",
        }}
      >
        <Space vertical size="small" style={{ width: "100%" }} align="center">
          {me && (
            <Button
              type="text"
              onClick={() => navigate("/profile/me")}
              style={{
                padding: 8,
                width: "100%",
                height: "100%",
              }}
            >
              <Space>
                <UserAvatar src={me.avatarUrl} userName={me.name} size={44} />

                <Space
                  vertical
                  align="start"
                  size={0}
                  style={{ width: "100%" }}
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
                </Space>
              </Space>
            </Button>
          )}
          <Flex justify="center" style={{ width: "100%" }}>
            <Button type="text" danger onClick={() => setLogOutModal(true)}>
              {t("sidebar.logout.default")}{" "}
              <LogoutOutlined style={{ fontSize: 18 }} />
            </Button>
          </Flex>
        </Space>
      </div>
    </div>
  );

  const logoutModal = (
    <AIModal
      open={logOutModal}
      onCancel={() => setLogOutModal(false)}
      title={t("sidebar.logout.title")}
      onOk={() => {
        mutate();
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
      width={260}
      style={{
        height: "100vh",
        position: "sticky",
        top: 0,
        left: 0,
        borderRight: `1px solid ${token.colorBorder}`,
        backgroundColor: token.colorBgContainer,
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
