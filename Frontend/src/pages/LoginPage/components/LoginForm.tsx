import { Button, Form, Input, Typography, App } from "antd";
import {
  MailOutlined,
  LockOutlined,
  SunOutlined,
  MoonOutlined,
} from "@ant-design/icons";
import { useAuth, useTheme } from "@/hooks";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import type { LoginRequest } from "@/types";

const { Title, Text } = Typography;

export default function LoginForm() {
  const { t } = useTranslation();
  const { theme, toggleTheme } = useTheme();
  const { login } = useAuth();
  const navigate = useNavigate();
  const [form] = Form.useForm();
  const { message } = App.useApp();

  const handleLogin = async (values: LoginRequest) => {
    try {
      await login.mutateAsync(values);
      message.success(t("login.loginSuccess"));
      navigate("/");
    } catch (error: any) {
      const errorMessage =
        error?.response?.data?.message || t("login.loginFailed");
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
        <Text className="login-mobile-slogan">{t("login.bannerSlogan1")}</Text>
      </div>

      <Title level={3} className="login-title">
        {t("login.title")}
      </Title>
      <Text className="login-subtitle">{t("login.subtitle")}</Text>

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
          label={t("login.emailLabel")}
          rules={[
            { required: true, message: t("login.emailRequired") },
            { type: "email", message: t("login.emailInvalid") },
          ]}
        >
          <Input
            prefix={<MailOutlined />}
            placeholder={t("login.emailPlaceholder")}
            size="large"
          />
        </Form.Item>

        <Form.Item
          name="password"
          label={t("login.passwordLabel")}
          rules={[{ required: true, message: t("login.passwordRequired") }]}
        >
          <Input.Password
            prefix={<LockOutlined />}
            placeholder={t("login.passwordPlaceholder")}
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
            {t("login.loginButton")}
          </Button>
        </Form.Item>
      </Form>

      <div className="login-footer">
        <Text className="login-footer-text">
          {t("login.noAccount")}{" "}
          <Button type="link" className="register-link">
            {t("login.createAccount")}
          </Button>
        </Text>
      </div>
    </div>
  );
}
