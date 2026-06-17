import React from "react";
import { Layout, Button, theme } from "antd";
import { SunOutlined, MoonOutlined } from "@ant-design/icons";
import { useTheme } from "@/hooks";
import { Breadcrumb, SidebarNav } from "@/components";

const { Header, Content } = Layout;

interface MainLayoutProps {
  isSidebarOpen?: boolean; // Cho phép truyền prop isSidebarOpen để điều khiển trạng thái mở/đóng của sidebar nếu cần
  children?: React.ReactNode; // Cho phép truyền bất kỳ trang nào vào làm ruột (Dashboard, Chat, Settings...)
  breadcrumbItems?: { title: string; href?: string }[]; // Cho phép truyền breadcrumb items từ trang con nếu cần, nếu không sẽ dùng default "Tổng quan"
}

export default function MainLayout({
  isSidebarOpen = true,
  children,
  breadcrumbItems = [],
}: MainLayoutProps) {
  const { token } = theme.useToken();
  const { theme: currentTheme, toggleTheme } = useTheme();

  return (
    <Layout style={{ minHeight: "100vh" }}>
      {/* 1. SIDEBAR BÊN TRÁI (Giữ nguyên từ phần trước) */}
      {isSidebarOpen && <SidebarNav />}

      {/* 2. KHỐI NỘI DUNG BÊN PHẢI */}
      <Layout
        style={{
          backgroundColor: token.colorBgLayout,
          transition: "all 0.3s ease",
        }}
      >
        {/* HEADER: Chứa nút Toggle Theme căn phải */}
        <Header
          style={{
            background: "transparent", // Để tiệp màu với nền Layout tạo cảm giác thoáng đãng
            padding: "0 24px",
            display: "flex",
            alignItems: "center",
            justifyContent: "flex-start", // Đẩy tất cả element bên trong về góc phải
            height: 64,
          }}
        >
          <Button
            type="text"
            shape="circle"
            icon={
              currentTheme === "dark" ? (
                <SunOutlined style={{ fontSize: 18 }} />
              ) : (
                <MoonOutlined style={{ fontSize: 18 }} />
              )
            }
            onClick={toggleTheme}
            style={{
              marginRight: 16,
              color: token.colorTextBase,
              backgroundColor:
                currentTheme === "dark"
                  ? "rgba(255,255,255,0.05)"
                  : "rgba(0,0,0,0.03)", // Tạo background mờ nhẹ cho nút
            }}
          />
          <Breadcrumb items={breadcrumbItems} />
        </Header>

        {/* CONTENT: Vùng hiển thị nội dung chính */}
        <Content
          style={{
            padding: "24px 48px 48px 48px", // Không padding top vì đã có khoảng trống của Header
            margin: 0,
            minHeight: "calc(100vh - 64px - 24px)",
            transition: "all 0.3s ease",
            color: token.colorTextBase,
          }}
        >
          {children}
        </Content>
      </Layout>
    </Layout>
  );
}
