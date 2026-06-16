import React from "react";
import { Layout, Button, theme } from "antd";
import { SunOutlined, MoonOutlined } from "@ant-design/icons";
import { useTheme } from "@/hooks";
import { Breadcrumb, FullscreenLoading, SidebarNav } from "@/components";

const { Header, Content } = Layout;

interface MainLayoutProps {
  isSidebarOpen?: boolean; // Cho phép truyền prop isSidebarOpen để điều khiển trạng thái mở/đóng của sidebar nếu cần
  isLoading?: boolean; // Cho phép truyền prop isLoading để hiển thị loading state nếu cần
  children?: React.ReactNode; // Cho phép truyền bất kỳ trang nào vào làm ruột (Dashboard, Chat, Settings...)
  breadcrumbItems?: { title: string; href?: string }[]; // Cho phép truyền breadcrumb items từ trang con nếu cần, nếu không sẽ dùng default "Tổng quan"
}

export default function MainLayout({
  isSidebarOpen = true,
  isLoading = false,
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
            padding: "0 24px 24px 24px", // Không padding top vì đã có khoảng trống của Header
            margin: 0,
          }}
        >
          {/* Box ruột màu trắng/tối bo góc hiện đại bọc lấy nội dung trang con */}
          <div
            style={{
              background: token.colorBgContainer, // Trắng ở Light Mode / Zinc 900 ở Dark Mode
              padding: "24px",
              borderRadius: token.borderRadius * 2, // Bo góc x2 để tạo độ mềm mại cho khung làm việc
              minHeight: "calc(100vh - 64px - 24px)", // Tự động căng full chiều cao còn lại của màn hình
              color: token.colorTextBase,
              border: `1px solid ${token.colorBorder}`,
              boxShadow:
                currentTheme === "dark"
                  ? "none"
                  : "0 1px 3px 0 rgba(0, 0, 0, 0.05), 0 1px 2px -1px rgba(0, 0, 0, 0.05)",
              transition: "all 0.3s ease",
            }}
          >
            {/* Nếu có trang con truyền vào thì render trang con, nếu không thì hiện text mặc định */}
            {isLoading && <FullscreenLoading />}{" "}
            {/* Hiển thị loading nếu isLoading là true */}
            {children || (
              <div>Bắt đầu thiết kế không gian làm việc của bạn tại đây...</div>
            )}
          </div>
        </Content>
      </Layout>
    </Layout>
  );
}
