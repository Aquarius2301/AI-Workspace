import {
  App,
  Button,
  Flex,
  Form,
  Grid,
  Input,
  Radio,
  theme,
  Typography,
} from "antd";
import { useTranslation } from "react-i18next";
import { AICard, AIThemeSwitch } from "@/components";
import { useLanguage, useLogin } from "@/hooks";
import { useNavigate } from "react-router-dom";
import type { CheckboxGroupProps } from "antd/es/checkbox";
import { getErrorMessage, getFormFieldErrors } from "@/utils";
import type { LoginRequest } from "@/types";
import { ROUTE } from "@/constants";

export default function LoginPage() {
  const { t } = useTranslation();
  const { language, changeLanguage } = useLanguage();
  const { token } = theme.useToken();
  const { message } = App.useApp();
  const navigate = useNavigate();
  const screens = Grid.useBreakpoint();
  const isMobile = !screens.md;
  const [form] = Form.useForm();
  const login = useLogin();
  const options: CheckboxGroupProps<string>["options"] = [
    { value: "vi", label: t("loginPage.language.vietnamese") },
    { value: "en", label: t("loginPage.language.english") },
  ];

  const handleSubmit = async (values: { email: string; password: string }) => {
    try {
      await login.mutateAsync(values);
      message.success(t("loginPage.success"));
      navigate(ROUTE.OVERVIEW, { replace: true });
    } catch (errors) {
      const fieldErrors = getFormFieldErrors<LoginRequest>(errors);
      if (fieldErrors.length > 0) {
        form.setFields(fieldErrors);
      } else {
        message.error(getErrorMessage(errors));
      }
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
                {t("loginPage.mobile.slogan")}
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
                {t("loginPage.banner.slogan")}
              </Typography.Title>
              <Typography.Text
                style={{
                  color: "rgba(255,255,255,0.7)",
                  fontSize: 14,
                }}
              >
                {t("loginPage.banner.subslogan")}
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
            <Flex justify="space-between" style={{ marginBottom: 16 }}>
              <Radio.Group
                vertical
                block
                options={options}
                defaultValue={language}
                onChange={(e) => changeLanguage(e.target.value)}
                optionType="button"
              />

              <AIThemeSwitch />
            </Flex>

            {/* Heading */}
            <Typography.Title
              level={3}
              style={{ marginBottom: 4, fontWeight: 600 }}
            >
              {t("loginPage.title")}
            </Typography.Title>
            <Typography.Text
              type="secondary"
              style={{ marginBottom: 32, fontSize: 15 }}
            >
              {t("loginPage.subtitle")}
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
                label={t("loginPage.email.title")}
                rules={[
                  { required: true, message: t("loginPage.email.required") },
                ]}
              >
                <Input placeholder={t("loginPage.email.placeholder")} />
              </Form.Item>

              <Form.Item
                name="password"
                label={t("loginPage.password.title")}
                rules={[
                  { required: true, message: t("loginPage.password.required") },
                ]}
              >
                <Input.Password
                  placeholder={t("loginPage.password.placeholder")}
                />
              </Form.Item>

              <Form.Item style={{ marginBottom: 20 }}>
                <Button
                  type="primary"
                  htmlType="submit"
                  block
                  loading={login.isPending}
                >
                  {t("loginPage.submit")}
                </Button>
              </Form.Item>
            </Form>

            {/* Register link */}
            <Flex gap={4} justify="center" style={{ marginTop: "auto" }}>
              <Typography.Text type="secondary">
                {t("loginPage.register.title")}
              </Typography.Text>
              <Typography.Link>{t("loginPage.register.link")}</Typography.Link>
            </Flex>
          </Flex>
        </Flex>
      </AICard>
    </Flex>
  );
}
