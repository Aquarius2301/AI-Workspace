import { App, Button, Flex, Form, Grid, Input, theme, Typography } from "antd";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import { AICard, AIThemeSwitch } from "@/components";
import { useAuth } from "@/hooks";
import { getTranslatedErrorMessage } from "@/utils";

export default function LoginPage() {
  const { t } = useTranslation();
  const { token } = theme.useToken();
  const { message } = App.useApp();
  const navigate = useNavigate();
  const { login } = useAuth();
  const screens = Grid.useBreakpoint();
  const isMobile = !screens.md;
  const [form] = Form.useForm();

  const handleSubmit = async (values: { email: string; password: string }) => {
    try {
      await login.mutateAsync(values);
      message.success(t("login.success"));
      navigate("/overview");
    } catch (error) {
      message.error(getTranslatedErrorMessage(error));
    }
  };

  const bannerGradient = `linear-gradient(135deg, ${token.colorPrimary} 0%, ${token.colorPrimary}DD 50%, ${token.colorPrimary}99 100%)`;

  return (
    <Flex
      align="center"
      justify="center"
      style={{
        minHeight: "100dvh",
        background: token.colorBgLayout,
        padding: 16,
      }}
    >
      <AICard
        isHovering={false}
        style={{
          width: "100%",
          maxWidth: 900,
          borderRadius: token.borderRadiusLG,
          overflow: "hidden",
          padding: 0,
        }}
      >
        <Flex
          vertical={isMobile}
          style={{ minHeight: isMobile ? undefined : 500 }}
        >
          {/* ── Banner ── */}
          {isMobile ? (
            <Flex
              vertical
              align="center"
              style={{
                background: bannerGradient,
                padding: "32px 24px 24px",
                textAlign: "center",
              }}
            >
              <Typography.Title level={3} style={{ color: "#fff", margin: 0 }}>
                AI Workspace
              </Typography.Title>
              <Typography.Text style={{ color: "rgba(255,255,255,0.85)" }}>
                {t("login.mobile.slogan")}
              </Typography.Text>
            </Flex>
          ) : (
            <Flex
              vertical
              align="center"
              justify="center"
              style={{
                width: "50%",
                background: bannerGradient,
                padding: 48,
                textAlign: "center",
                position: "relative",
              }}
            >
              <Typography.Title
                level={2}
                style={{ color: "#fff", margin: 0, fontWeight: 700 }}
              >
                AI Workspace
              </Typography.Title>
              <Typography.Title
                level={5}
                style={{
                  color: "rgba(255,255,255,0.9)",
                  fontWeight: 400,
                  marginTop: 12,
                  marginBottom: 8,
                }}
              >
                {t("login.banner.slogan")}
              </Typography.Title>
              <Typography.Text
                style={{
                  color: "rgba(255,255,255,0.7)",
                  fontSize: 14,
                }}
              >
                {t("login.banner.subslogan")}
              </Typography.Text>
            </Flex>
          )}

          {/* ── Form Section ── */}
          <Flex
            vertical
            flex={1}
            style={{
              padding: isMobile ? 24 : 40,
              position: "relative",
            }}
          >
            {/* Theme toggle */}
            <Flex justify="end" style={{ marginBottom: 16 }}>
              <AIThemeSwitch />
            </Flex>

            {/* Heading */}
            <Typography.Title
              level={3}
              style={{ marginBottom: 4, fontWeight: 600 }}
            >
              {t("login.title")}
            </Typography.Title>
            <Typography.Text
              type="secondary"
              style={{ marginBottom: 32, fontSize: 15 }}
            >
              {t("login.subtitle")}
            </Typography.Text>

            {/* Form */}
            <Form
              form={form}
              layout="vertical"
              onFinish={handleSubmit}
              size="large"
              requiredMark={false}
            >
              <Form.Item
                name="email"
                label={t("login.email.default")}
                rules={[
                  { required: true, message: t("login.email.required") },
                  { type: "email", message: t("login.email.invalid") },
                ]}
              >
                <Input placeholder={t("login.email.placeholder")} />
              </Form.Item>

              <Form.Item
                name="password"
                label={t("login.password.default")}
                rules={[
                  { required: true, message: t("login.password.required") },
                ]}
              >
                <Input.Password placeholder={t("login.password.placeholder")} />
              </Form.Item>

              <Form.Item style={{ marginBottom: 20 }}>
                <Button
                  type="primary"
                  htmlType="submit"
                  block
                  loading={login.isPending}
                >
                  {t("login.submit")}
                </Button>
              </Form.Item>
            </Form>

            {/* Register link */}
            <Flex gap={4} justify="center" style={{ marginTop: "auto" }}>
              <Typography.Text type="secondary">
                {t("login.register.default")}
              </Typography.Text>
              <Typography.Link>{t("login.register.link")}</Typography.Link>
            </Flex>
          </Flex>
        </Flex>
      </AICard>
    </Flex>
  );
}
