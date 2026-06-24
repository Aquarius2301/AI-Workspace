import { Card, Avatar, Typography, Space, theme } from "antd";
import { UserOutlined } from "@ant-design/icons";
import { useTranslation } from "react-i18next";

const { Text, Title } = Typography;

interface UserGreetingProps {
  name: string;
  email: string;
  avatar?: string;
}

export function UserGreeting({ name, email, avatar }: UserGreetingProps) {
  const { t } = useTranslation();
  const { token } = theme.useToken();

  return (
    <Card
      style={{
        borderRadius: token.borderRadius * 2,
        border: `1px solid ${token.colorBorder}`,
        background: `linear-gradient(135deg, ${token.colorPrimary}15 0%, ${token.colorBgContainer} 60%)`,
        width: "100%",
      }}
      styles={{
        body: { padding: 24 },
      }}
    >
      <Space
        size={20}
        style={{
          display: "flex",
          alignItems: "center",
          flexWrap: "wrap",
        }}
      >
        <Avatar
          size={64}
          src={avatar}
          icon={avatar || <UserOutlined />}
          style={{
            border: `3px solid ${token.colorPrimary}`,
            boxShadow: `0 4px 12px ${token.colorPrimary}30`,
            flexShrink: 0,
          }}
        />
        <div style={{ flex: 1, minWidth: 200 }}>
          <Title
            level={4}
            style={{
              margin: 0,
              color: token.colorTextBase,
              fontWeight: 700,
            }}
          >
            {t("home.greeting")}, {name || t("home.userFallback")}! 👋
          </Title>
          <Text
            type="secondary"
            style={{
              fontSize: 14,
              display: "block",
              marginTop: 4,
            }}
          >
            {email || t("home.noEmail")}
          </Text>
          <Text
            style={{
              fontSize: 13,
              color: token.colorTextDescription,
              display: "block",
              marginTop: 2,
            }}
          >
            {t("home.welcomeBack")}
          </Text>
        </div>
      </Space>
    </Card>
  );
}
