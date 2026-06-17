import { Button, Form, Input, Typography, App } from "antd";
import {
  MailOutlined,
  LockOutlined,
  SunOutlined,
  MoonOutlined,
} from "@ant-design/icons";
import { useAuth, useTheme } from "@/hooks";
import { useNavigate } from "react-router-dom";
import type { LoginRequest } from "@/types";

const { Title, Text } = Typography;

export default function LoginForm() {
  const { theme, toggleTheme } = useTheme();
  const { login } = useAuth();
  const navigate = useNavigate();
  const [form] = Form.useForm();
  const { message } = App.useApp();

  const handleLogin = async (values: LoginRequest) => {
    try {
      await login.mutateAsync(values);
      message.success("Đăng nhập thành công!");
      navigate("/");
    } catch (error: any) {
      const errorMessage =
        error?.response?.data?.message ||
        "Đăng nhập thất bại. Vui lòng thử lại!";
      message.error(errorMessage);
    }
  };

  return (
    <div className="login-form-wrapper">
      {/* Theme toggle */}
      <div className="login-theme-toggle">
        <Button
          type="text"
          shape="circle"
          icon={
            theme === "dark" ? (
              <SunOutlined style={{ fontSize: 18 }} />
            ) : (
              <MoonOutlined style={{ fontSize: 18 }} />
            )
          }
          onClick={toggleTheme}
          style={{
            // color: token.colorTextBase,
            backgroundColor:
              theme === "dark" ? "rgba(255,255,255,0.05)" : "rgba(0,0,0,0.03)", // Tạo background mờ nhẹ cho nút
          }}
        />
      </div>
      {/* Mobile header - shows only on small screens */}
      <div className="login-mobile-header">
        <Title level={2} className="login-mobile-title">
          AI Workspace
        </Title>
        <Text className="login-mobile-slogan">
          Nơi trí tuệ nhân tạo và sáng tạo hội tụ
        </Text>
      </div>

      <Title level={3} className="login-title">
        Đăng nhập
      </Title>
      <Text className="login-subtitle">
        Chào mừng bạn trở lại! Vui lòng đăng nhập để tiếp tục.
      </Text>

      <Form
        form={form}
        layout="vertical"
        onFinish={handleLogin}
        autoComplete="off"
        requiredMark={false}
        className="login-form"
      >
        <Form.Item
          name="email"
          label="Email"
          rules={[
            { required: true, message: "Vui lòng nhập email!" },
            { type: "email", message: "Email không hợp lệ!" },
          ]}
        >
          <Input
            prefix={<MailOutlined />}
            placeholder="Nhập email của bạn"
            size="large"
          />
        </Form.Item>

        <Form.Item
          name="password"
          label="Mật khẩu"
          rules={[
            { required: true, message: "Vui lòng nhập mật khẩu!" },
            { min: 6, message: "Mật khẩu phải có ít nhất 6 ký tự!" },
          ]}
        >
          <Input.Password
            prefix={<LockOutlined />}
            placeholder="Nhập mật khẩu"
            size="large"
          />
        </Form.Item>

        <Form.Item>
          <Button
            type="primary"
            htmlType="submit"
            block
            size="large"
            loading={login.isPending}
            className="login-button"
          >
            Đăng nhập
          </Button>
        </Form.Item>
      </Form>

      <div className="login-footer">
        <Text className="login-footer-text">
          Chưa có tài khoản?{" "}
          <Button type="link" className="register-link">
            Tạo tài khoản
          </Button>
        </Text>
      </div>
    </div>
  );
}
