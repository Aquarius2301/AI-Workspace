import { Card, Avatar, Typography, Space, theme } from "antd";
import { UserOutlined } from "@ant-design/icons";

const { Text, Title } = Typography;

interface UserGreetingProps {
  name: string;
  email: string;
  avatar?: string;
}

export function UserGreeting({ name, email, avatar }: UserGreetingProps) {
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
          icon={!avatar ? <UserOutlined /> : undefined}
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
            Xin chào, {name || "Người dùng"}! 👋
          </Title>
          <Text
            type="secondary"
            style={{
              fontSize: 14,
              display: "block",
              marginTop: 4,
            }}
          >
            {email || "Chưa có email"}
          </Text>
          <Text
            style={{
              fontSize: 13,
              color: token.colorTextDescription,
              display: "block",
              marginTop: 2,
            }}
          >
            Chào mừng bạn quay trở lại với không gian làm việc
          </Text>
        </div>
      </Space>
    </Card>
  );
}
