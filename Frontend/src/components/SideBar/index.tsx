import { useState } from "react";
import {
  Layout,
  Menu,
  Avatar,
  Button,
  Space,
  Typography,
  theme,
  Modal,
} from "antd";
import { useNavigate, useLocation } from "react-router-dom";
import {
  AppstoreOutlined,
  SettingOutlined,
  FolderOpenOutlined,
  LogoutOutlined,
  BulbOutlined,
  TeamOutlined,
} from "@ant-design/icons";
import type { UserResponse } from "@/types";
import { MeQueryKey, useAuth, useGetCacheData } from "@/hooks";

const { Sider } = Layout;
const { Text, Title } = Typography;

// interface SidebarNavProps {
//   avatarUrl?: string;
//   name: string;
//   email: string;
// }

export function SidebarNav() {
  const { token } = theme.useToken();
  const navigate = useNavigate();
  const location = useLocation();
  const [collapsed, setCollapsed] = useState(false);

  const me = useGetCacheData<UserResponse>(MeQueryKey);

  const { logout } = useAuth();

  const onLogout = () => {
    Modal.confirm({
      title: "Xác nhận đăng xuất",
      content: "Bạn có chắc chắn muốn đăng xuất khỏi hệ thống không?",
      okText: "Đăng xuất",
      cancelText: "Hủy",
      onOk: () => {
        logout.mutate(); // Gọi API logout để xóa token và thông tin người dùng
        navigate("/login");
      },
    });
  };
  // Danh sách các Item Menu điều hướng
  const menuItems = [
    {
      key: "/",
      icon: <AppstoreOutlined style={{ fontSize: "18px" }} />,
      label: "Tổng quan",
    },
    {
      key: "/teams",
      icon: <TeamOutlined style={{ fontSize: "18px" }} />,
      label: "Nhóm của tôi",
    },
    {
      key: "/projects",
      icon: <FolderOpenOutlined style={{ fontSize: "18px" }} />,
      label: "Dự án của tôi",
    },
    {
      key: "/settings",
      icon: <SettingOutlined style={{ fontSize: "18px" }} />,
      label: "Cài đặt hệ thống",
    },
  ];

  // Xử lý khi click vào menu item
  const handleMenuClick = (e: { key: string }) => {
    navigate(e.key);
  };

  // Lấy key hiện tại từ pathname
  const getSelectedKey = (pathname: string) => {
    if (pathname.startsWith("/teams")) return "/teams";
    if (pathname.startsWith("/projects")) return "/projects";
    if (pathname.startsWith("/settings")) return "/settings";

    return "/";
  };

  return (
    <Sider
      collapsible
      breakpoint="lg" // Tự động thu nhỏ khi màn hình dưới độ phân giải lg (992px)
      collapsedWidth={72} // Chiều rộng khi thu nhỏ (chỉ hiện icon)
      width={260} // Chiều rộng khi mở rộng hoàn toàn
      collapsed={collapsed}
      onCollapse={(value) => setCollapsed(value)}
      trigger={null} // Ẩn nút trigger mặc định ở dưới cùng để giao diện sạch hơn
      style={{
        height: "100vh",
        position: "sticky",
        top: 0,
        left: 0,
        borderRight: `1px solid ${token.colorBorder}`,
        backgroundColor: token.colorBgContainer, // Đồng bộ màu nền Sidebar theo theme
        transition: "all 0.2s ease",
      }}
    >
      {/* CONTAINER CHÍNH: Dùng Flexbox dọc để đẩy phần User xuống đáy */}
      <div
        style={{
          display: "flex",
          flexDirection: "column",
          height: "100%",
          justifyContent: "space-between",
        }}
      >
        {/* PHẦN TRÊN: LOGO & MENU ITEMS */}
        <div>
          {/* LOGO BRANDING */}
          <div
            style={{
              padding: collapsed ? "24px 0" : "24px 20px",
              textAlign: collapsed ? "center" : "left",
              transition: "all 0.2s",
            }}
          >
            {collapsed ? (
              <BulbOutlined
                style={{ fontSize: 26, color: token.colorPrimary }}
              />
            ) : (
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
            )}
          </div>

          {/* MENU CHÍNH */}
          <Menu
            mode="inline"
            selectedKeys={[getSelectedKey(location.pathname)]}
            items={menuItems}
            onClick={handleMenuClick}
            style={{
              borderRight: 0,
              backgroundColor: "transparent",
            }}
          />
        </div>

        {/* PHẦN DƯỚI: AVATAR & LOGOUT (Tự động thay đổi UI khi co/giãn) */}
        <div
          style={{
            padding: "16px",
            borderTop: `1px solid ${token.colorBorder}`,
            backgroundColor: isDarkMode(token) ? "#141416" : "#F8FAFC", // Tạo mảng màu nền khối user nổi bật nhẹ
            transition: "all 0.2s",
          }}
        >
          {collapsed ? (
            /* KHI THU NHỎ: Chỉ hiển thị Avatar và nút Logout xếp dọc */
            <Space
              vertical
              align="center"
              size="middle"
              style={{ width: "100%" }}
            >
              <Avatar
                src={me.avatar}
                style={{
                  cursor: "pointer",
                  border: `2px solid ${token.colorBorder}`,
                }}
              />
              <Button
                type="text"
                danger
                icon={<LogoutOutlined style={{ fontSize: 18 }} />}
                onClick={onLogout}
              />
            </Space>
          ) : (
            /* KHI MỞ RỘNG: Hiển thị đầy đủ Avatar, Tên, Email và nút Logout bên phải */
            <div
              style={{
                display: "flex",
                alignItems: "center",
                justifyContent: "space-between",
              }}
            >
              <Space size="middle" style={{ overflow: "hidden" }}>
                <Avatar
                  size="large"
                  src={me.avatar}
                  style={{
                    border: `2px solid ${token.colorBorder}`,
                    flexShrink: 0,
                  }}
                />
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
                    {me.name}
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
                    {me.email}
                  </Text>
                </div>
              </Space>

              <Button
                type="text"
                danger
                shape="circle"
                icon={<LogoutOutlined style={{ fontSize: 16 }} />}
                onClick={onLogout}
                style={{ flexShrink: 0 }}
              />
            </div>
          )}
        </div>
      </div>
    </Sider>
  );
}

// Hàm bổ trợ kiểm tra xem Token hiện tại có phải là Dark Mode hay không dựa vào màu chữ chính
const isDarkMode = (token: any) => token.colorTextBase === "#FAFAFA";
